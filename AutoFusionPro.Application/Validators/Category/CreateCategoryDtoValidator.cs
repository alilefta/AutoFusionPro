using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.Category
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.ImagePath)
                .MaximumLength(500).WithMessage("Image path cannot exceed 500 characters."); // Assuming path length

            When(x => x.ParentCategoryId.HasValue && x.ParentCategoryId.Value > 0, () =>
            {
                RuleFor(x => x.ParentCategoryId)
                    .MustAsync(async (id, token) => await ParentCategoryExists(id!.Value))
                    .WithMessage("Selected parent category does not exist.");
            });

            // Rule for uniqueness: Name must be unique under the same parent (or globally if no parent)
            RuleFor(x => x) // Validating the whole object to access Name and ParentCategoryId
                .MustAsync(async (dto, token) => await IsNameUniqueUnderParent(dto.Name, dto.ParentCategoryId, null))
                .WithMessage(dto => $"A category named '{dto.Name}' already exists " +
                                    (dto.ParentCategoryId.HasValue ? "under the selected parent." : "at the top level."))
                .When(dto => !string.IsNullOrWhiteSpace(dto.Name)); // Only run if name is provided
        }

        private async Task<bool> ParentCategoryExists(int parentCategoryId)
        {
            return await _unitOfWork.Categories.ExistsAsync(c => c.Id == parentCategoryId);
        }

        private async Task<bool> IsNameUniqueUnderParent(string name, int? parentCategoryId, int? excludeCategoryId)
        {
            if (string.IsNullOrWhiteSpace(name)) return true; // Handled by NotEmpty rule
            return !await _unitOfWork.Categories.NameExistsAsync(name, parentCategoryId, excludeCategoryId);
        }
    }
}
