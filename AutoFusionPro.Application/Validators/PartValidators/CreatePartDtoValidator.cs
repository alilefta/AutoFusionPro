using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.DTOs.Vehicle;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Application.Validators.PartValidators
{
    public class CreatePartDtoValidator : AbstractValidator<CreatePartDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreatePartDtoValidator> _logger;

        // Inject IUnitOfWork to access repositories for database checks
        public CreatePartDtoValidator(ILogger<CreatePartDtoValidator> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            RuleFor(p => p.PartNumber)
                .NotEmpty().WithMessage("Part Number is required.")
                .MaximumLength(50).WithMessage("Part Number cannot exceed 50 characters.")
                // Custom async rule to check for uniqueness
                .MustAsync(async (partNumber, cancellationToken) =>
                {
                    // Only check if partNumber is not empty to avoid unnecessary DB call
                    if (string.IsNullOrWhiteSpace(partNumber)) return true;
                    return !await _unitOfWork.Parts.PartNumberExistsAsync(partNumber);
                })
                .WithMessage(p => $"Part Number '{p.PartNumber}' already exists."); // Dynamic error message

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Part Name is required.")
                .MaximumLength(100).WithMessage("Part Name cannot exceed 100 characters.");

            RuleFor(p => p.CategoryId)
                .GreaterThan(0).WithMessage("A valid Category must be selected.")
                // Check if the category actually exists
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    if (categoryId <= 0) return true; // Handled by GreaterThan(0)
                    // Assuming ICategoryRepository is added to IUnitOfWork or you use context directly
                    // Option 1: Using Repository (preferred if exists)
                    // return await _unitOfWork.Categories.ExistsAsync(c => c.Id == categoryId); // Requires ICategoryRepository
                    // Option 2: If no CategoryRepository, check via DbContext (less ideal abstraction-wise)
                    // return await _unitOfWork.Context.Set<Domain.Models.Category>().AnyAsync(c => c.Id == categoryId, cancellationToken);
                    // For now, let's assume you'll add ICategoryRepository
                    return await CategoryExists(categoryId); // Use helper method
                })
                .WithMessage("Selected Category does not exist.");


            RuleFor(p => p.CostPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Cost Price cannot be negative.");

            RuleFor(p => p.SellingPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Selling Price cannot be negative.");
            // Optional: RuleFor(p => p.SellingPrice).GreaterThanOrEqualTo(p => p.CostPrice).WithMessage("Selling Price should not be less than Cost Price.");

            RuleFor(p => p.ReorderLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Reorder Level cannot be negative.");

            RuleFor(p => p.MinimumStock)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum Stock cannot be negative.");

            RuleFor(p => p.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(p => p.Manufacturer)
                .MaximumLength(100).WithMessage("Manufacturer cannot exceed 100 characters.");

            RuleFor(p => p.Location)
                .MaximumLength(50).WithMessage("Location cannot exceed 50 characters.");

            RuleFor(p => p.ImagePath)
                .MaximumLength(255).WithMessage("Image Path cannot exceed 255 characters.");

            RuleFor(p => p.Notes)
               .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");

            // Add rules for InitialCompatibleVehicles, InitialSuppliers if needed
            // e.g., RuleForEach(p => p.InitialSuppliers).SetValidator(new PartSupplierCreateDtoValidator());

        }

        // Helper method to check category existence (assuming ICategoryRepository will be added to UoW)
        private async Task<bool> CategoryExists(int categoryId)
        {
            // Placeholder: Implement using _unitOfWork.Categories.ExistsAsync or similar
            // Example: return await _unitOfWork.Context.Set<Domain.Models.Category>().AnyAsync(c => c.Id == categoryId);
            // Replace with your actual repository call
            var categoryRepo = _unitOfWork.Categories; // Example if using generic repo access
            if (categoryRepo != null)
            {
                return await categoryRepo.ExistsAsync(c => c.Id == categoryId);
            }
            // Fallback or throw if repo not available - depends on your UoW setup
            _logger.LogWarning("Warning: ICategoryRepository not found in UnitOfWork for validation.");
            return false; // Or true, depending on desired behaviour if repo missing
        }

        // Helper to access generic repository if needed for Category check
        // Replace this with direct ICategoryRepository property if added to IUnitOfWork
        public interface IUnitOfWorkWithGenericRepo : IUnitOfWork
        {
            IBaseRepository<T> Repository<T>() where T : class;
        }
    }
}
