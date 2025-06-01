using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.Category
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Category ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.ImagePath)
                .MaximumLength(500).WithMessage("Image path cannot exceed 500 characters.");

            // Rule for ParentCategoryId
            RuleFor(x => x.ParentCategoryId)
                // Check existence only if a value is provided
                .MustAsync(async (parentId, token) =>
                {
                    if (!parentId.HasValue || parentId.Value == 0) return true; // No parent or "0" means top-level / no check needed here for existence
                    return await ParentCategoryExists(parentId.Value, token);
                })
                .WithMessage("Selected parent category does not exist.")
                .When(x => x.ParentCategoryId.HasValue && x.ParentCategoryId.Value > 0) // Apply only when a valid parent ID is given

                // Prevent setting self as parent
                .Must((dto, parentId) => !parentId.HasValue || parentId.Value != dto.Id)
                .WithMessage("Category cannot be its own parent.")

                // Prevent setting a descendant as parent (circular reference)
                .MustAsync(async (dto, parentId, token) =>
                {
                    if (!parentId.HasValue || parentId.Value == 0) return true; // No parent or "0" means no circular check needed
                    return await IsNotDescendant(dto.Id, parentId.Value, token);
                })
                .WithMessage("Cannot set parent category to one of its own subcategories (circular reference).")
                .When(x => x.ParentCategoryId.HasValue && x.ParentCategoryId.Value > 0); // Apply only when a valid parent ID is given


            // Rule for uniqueness: Name must be unique under the same parent (or globally if no parent), excluding self
            RuleFor(x => x) // Validating the whole object to access Name and ParentCategoryId
                .MustAsync(async (dto, token) => await IsNameUniqueUnderParent(dto.Name, dto.ParentCategoryId, dto.Id, token))
                .WithMessage(dto => $"A category named '{dto.Name}' already exists " +
                                    (dto.ParentCategoryId.HasValue ? "under the selected parent." : "at the top level."))
                .When(dto => !string.IsNullOrWhiteSpace(dto.Name)); // Only run if name is provided
        }

        private async Task<bool> ParentCategoryExists(int parentCategoryId, CancellationToken cancellationToken = default)
        {
            // Assuming ParentCategoryId 0 is not a valid ID but might be used by UI for "no parent"
            if (parentCategoryId <= 0) return false; // Or true if 0 means "no parent selected" and thus valid
            return await _unitOfWork.Categories.ExistsAsync(c => c.Id == parentCategoryId);
        }

        private async Task<bool> IsNameUniqueUnderParent(string name, int? parentCategoryId, int? excludeCategoryId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return true; // Handled by NotEmpty rule
            return !await _unitOfWork.Categories.NameExistsAsync(name, parentCategoryId, excludeCategoryId);
        }

        /// <summary>
        /// Checks if the proposed parentId is not a descendant of the categoryId (being updated).
        /// This prevents creating circular dependencies in the category hierarchy.
        /// </summary>
        private async Task<bool> IsNotDescendant(int categoryIdToUpdate, int proposedParentId, CancellationToken cancellationToken = default)
        {
            if (categoryIdToUpdate == proposedParentId) return false; // Explicitly cannot be its own parent

            var currentAncestorId = proposedParentId; // Start with the proposed parent
            int loopGuard = 0; // Prevent infinite loops in case of unexpected data issues
            const int maxDepth = 20; // Max hierarchy depth to check

            while (loopGuard++ < maxDepth)
            {
                // Fetch the current ancestor to check its ParentCategoryId
                // A repository method to get just the ParentCategoryId would be more efficient:
                // var parentOfCurrentAncestorId = await _unitOfWork.Categories.GetParentIdAsync(currentAncestorId);

                // Using GetByIdAsync for now:
                var currentAncestor = await _unitOfWork.Categories.GetByIdAsync(currentAncestorId);
                if (cancellationToken.IsCancellationRequested) return false; // Or true, depending on how you want to handle cancellation

                if (currentAncestor == null)
                {
                    // Proposed parent (or one of its ancestors) doesn't exist, which is an issue
                    // This should ideally be caught by ParentCategoryExists rule first.
                    // Or, it means we've reached the top of a valid chain without finding categoryIdToUpdate.
                    return true;
                }

                if (!currentAncestor.ParentCategoryId.HasValue)
                {
                    // Reached a top-level category in the ancestry of proposedParentId
                    // without finding categoryIdToUpdate. So, it's a valid parent.
                    return true;
                }

                if (currentAncestor.ParentCategoryId.Value == categoryIdToUpdate)
                {
                    // Found categoryIdToUpdate in the ancestry of proposedParentId,
                    // meaning proposedParentId is a descendant. This is invalid.
                    return false;
                }
                currentAncestorId = currentAncestor.ParentCategoryId.Value; // Move up the tree
            }

            // Exceeded max depth, assume potential issue or very deep valid tree.
            // For safety, you might return false or log a warning.
            // If the tree is legitimately deeper than maxDepth and valid, this would be a false negative.
            // This loop guard is mainly for safety against bad data or runaway recursion.
            return true; // Or false, depending on how strict you want to be on depth.
        }
    }
}
