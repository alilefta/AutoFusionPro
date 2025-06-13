using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Application.Validators.PartValidators
{
    public class CreatePartDtoValidator : AbstractValidator<CreatePartDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreatePartDtoValidator> _logger;

        // Inject IUnitOfWork to access repositories for database checks
        public CreatePartDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.PartNumber)
                .NotEmpty().WithMessage("Part Number is required.")
                .MaximumLength(50).WithMessage("Part Number cannot exceed 50 characters.")
                .MustAsync(async (partNumber, token) => !await _unitOfWork.Parts.PartNumberExistsAsync(partNumber))
                .WithMessage(dto => $"Part Number '{dto.PartNumber}' already exists.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Part Name is required.")
                .MaximumLength(100).WithMessage("Part Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Manufacturer)
                .MaximumLength(100).WithMessage("Manufacturer cannot exceed 100 characters.");

            RuleFor(x => x.CostPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Cost Price must be zero or greater.");

            RuleFor(x => x.SellingPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Selling Price must be zero or greater.");

            RuleFor(x => x.StockQuantity) // Initial stock
                .GreaterThanOrEqualTo(0).WithMessage("Initial Stock Quantity must be zero or greater.");

            RuleFor(x => x.ReorderLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Reorder Level must be zero or greater.");

            RuleFor(x => x.MinimumStock)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum Stock must be zero or greater.");

            RuleFor(x => x.Location)
                .MaximumLength(50).WithMessage("Location cannot exceed 50 characters.");

            RuleFor(x => x.ImagePath)
                .MaximumLength(255).WithMessage("Image Path cannot exceed 255 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");

            RuleFor(x => x.Barcode)
                .MaximumLength(100).WithMessage("Barcode cannot exceed 100 characters.") // Adjust length as needed
                .MustAsync(async (barcode, token) =>
                    string.IsNullOrWhiteSpace(barcode) || !await _unitOfWork.Parts.BarcodeExistsAsync(barcode))
                .WithMessage(dto => $"Barcode '{dto.Barcode}' already exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Barcode)); // Only validate if barcode is provided

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("A valid Category must be selected.")
                .MustAsync(async (categoryId, token) => await _unitOfWork.Categories.ExistsAsync(c => c.Id == categoryId))
                .WithMessage("Selected Category does not exist.");

            RuleFor(x => x.StockingUnitOfMeasureId)
                .GreaterThan(0).WithMessage("A valid stocking unit of measure must be selected.")
                .MustAsync(async (id, token) => await _unitOfWork.UnitOfMeasures.ExistsAsync(uom => uom.Id == id)) // Assuming generic Exists or UoM repo
                .WithMessage("Selected stocking unit of measure does not exist.");

            // Sales UoM
            When(x => x.SalesUnitOfMeasureId.HasValue && x.SalesUnitOfMeasureId.Value > 0, () => {
                RuleFor(x => x.SalesUnitOfMeasureId)
                    .MustAsync(async (id, token) => await _unitOfWork.UnitOfMeasures.ExistsAsync(uom => uom.Id == id!.Value))
                    .WithMessage("Selected sales unit of measure does not exist.");

                // If SalesUoM is different from StockingUoM, SalesConversionFactor is required and must be > 0
                RuleFor(x => x.SalesConversionFactor)
                    .NotNull().WithMessage("Sales conversion factor is required when Sales Unit is different from Stocking Unit.")
                    .GreaterThan(0).WithMessage("Sales conversion factor must be greater than zero.")
                    .When(x => x.SalesUnitOfMeasureId.HasValue && x.SalesUnitOfMeasureId.Value != x.StockingUnitOfMeasureId);
            });
            // If SalesUoM is NOT set OR is THE SAME as StockingUoM, SalesConversionFactor should NOT be set (or be 1 and hidden in UI)
            When(x => !x.SalesUnitOfMeasureId.HasValue || x.SalesUnitOfMeasureId.Value == 0 || x.SalesUnitOfMeasureId.Value == x.StockingUnitOfMeasureId, () => {
                RuleFor(x => x.SalesConversionFactor)
                    .Null().WithMessage("Sales conversion factor should not be set if Sales Unit is the same as Stocking Unit or not specified.");
            });


            // Purchase UoM (similar logic to Sales UoM)
            When(x => x.PurchaseUnitOfMeasureId.HasValue && x.PurchaseUnitOfMeasureId.Value > 0, () => {
                RuleFor(x => x.PurchaseUnitOfMeasureId)
                    .MustAsync(async (id, token) => await _unitOfWork.UnitOfMeasures.ExistsAsync(uom => uom.Id == id!.Value))
                    .WithMessage("Selected purchase unit of measure does not exist.");

                RuleFor(x => x.PurchaseConversionFactor)
                    .NotNull().WithMessage("Purchase conversion factor is required when Purchase Unit is different from Stocking Unit.")
                    .GreaterThan(0).WithMessage("Purchase conversion factor must be greater than zero.")
                    .When(x => x.PurchaseUnitOfMeasureId.HasValue && x.PurchaseUnitOfMeasureId.Value != x.StockingUnitOfMeasureId);
            });
            When(x => !x.PurchaseUnitOfMeasureId.HasValue || x.PurchaseUnitOfMeasureId.Value == 0 || x.PurchaseUnitOfMeasureId.Value == x.StockingUnitOfMeasureId, () => {
                RuleFor(x => x.PurchaseConversionFactor)
                    .Null().WithMessage("Purchase conversion factor should not be set if Purchase Unit is the same as Stocking Unit or not specified.");
            });

            // Optional: Validation for InitialSuppliers and InitialCompatibleVehicles
            // This can get complex if you need to validate each item in the list
            // or check existence of SupplierId/CompatibleVehicleId.
            // For now, we assume the service layer might handle this or they are simpler.
            // Example for InitialSuppliers:
            // RuleForEach(x => x.InitialSuppliers).SetValidator(new PartSupplierCreateDtoValidator(unitOfWork));
            // (You would need a PartSupplierCreateDtoValidator)
        }
    

        //// Helper to access generic repository if needed for Category check
        //// Replace this with direct ICategoryRepository property if added to IUnitOfWork
        //public interface IUnitOfWorkWithGenericRepo : IUnitOfWork
        //{
        //    IBaseRepository<T> Repository<T>() where T : class;
        //}
    }
}
