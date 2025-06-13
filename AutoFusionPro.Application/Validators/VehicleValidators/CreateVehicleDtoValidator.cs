using AutoFusionPro.Application.DTOs.VehicleAsset;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.VehicleValidators
{
    public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleAssetDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        // Injecting the service allows using helper methods like MakeModelYearExistsAsync
        // Alternatively, inject IUnitOfWork if you prefer direct repo checks here
        //private readonly IVehicleService _vehicleService; // Or IUnitOfWork _unitOfWork; // Resulted in Circular Dependency

        //public CreateVehicleDtoValidator(IUnitOfWork unitOfWork) // Inject service
        //{
        //    _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        //    RuleFor(v => v.Make)
        //        .NotEmpty().WithMessage("Make is required.")
        //        .MaximumLength(50).WithMessage("Make cannot exceed 50 characters.");

        //    RuleFor(v => v.Model)
        //        .NotEmpty().WithMessage("Model is required.")
        //        .MaximumLength(50).WithMessage("Model cannot exceed 50 characters.");

        //    RuleFor(v => v.Year)
        //        .InclusiveBetween(1900, DateTime.Now.Year + 2).WithMessage($"Year must be between 1900 and {DateTime.Now.Year + 2}."); // Sensible year range

        //    // --- Uniqueness Check ---
        //    // Use RuleSet or MustAsync for database checks
        //    RuleFor(v => v) // Rule for the whole DTO
        //        .MustAsync(async (dto, cancellationToken) =>
        //        {
        //            // Only check if key fields are provided
        //            if (string.IsNullOrWhiteSpace(dto.Make) || string.IsNullOrWhiteSpace(dto.Model) || dto.Year < 1900)
        //                return true; // Let other rules handle missing required fields

        //            // Use the service method for the check
        //            return !await _unitOfWork.Vehicles.MakeModelYearExistsAsync(dto.Make, dto.Model, dto.Year);
        //        })
        //        .WithMessage(dto => $"A vehicle with Make '{dto.Make}', Model '{dto.Model}', and Year '{dto.Year}' already exists.")
        //        .When(dto => !string.IsNullOrWhiteSpace(dto.Make) && !string.IsNullOrWhiteSpace(dto.Model) && dto.Year >= 1900); // Only run when key fields are valid

        //    // --- Optional VIN Validation ---
        //    When(v => !string.IsNullOrWhiteSpace(v.VIN), () => // Only validate VIN if provided
        //    {
        //        RuleFor(v => v.VIN)
        //            .Length(17).WithMessage("VIN must be exactly 17 characters if provided.")
        //            // Add Regex for valid VIN characters if needed: .Matches("^[A-HJ-NPR-Z0-9]{17}$")
        //            .MustAsync(async (vin, cancellationToken) =>
        //            {
        //                if (string.IsNullOrWhiteSpace(vin)) return true;
        //                // Use service method or direct repo access via UoW
        //                return !await _unitOfWork.Vehicles.VinExistsAsync(vin); // Assuming VinExistsAsync checks *all* vehicles
        //            })
        //            .WithMessage(v => $"VIN '{v.VIN}' already exists.");
        //    });


        //    // --- Optional Other Fields ---
        //    RuleFor(v => v.Engine)
        //        .MaximumLength(50).WithMessage("Engine description cannot exceed 50 characters.");

        //    RuleFor(v => v.Transmission)
        //        .MaximumLength(50).WithMessage("Transmission description cannot exceed 50 characters.");

        //    RuleFor(v => v.TrimLevel)
        //        .MaximumLength(50).WithMessage("Trim Level cannot exceed 50 characters.");

        //    RuleFor(v => v.BodyType)
        //        .MaximumLength(50).WithMessage("Body Type cannot exceed 50 characters.");
        //}
    }
}

