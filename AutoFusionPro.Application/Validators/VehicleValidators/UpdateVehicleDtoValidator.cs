using AutoFusionPro.Application.DTOs.VehicleAsset;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.VehicleValidators
{
    public class UpdateVehicleDtoValidator : AbstractValidator<CreateVehicleAssetDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        //public UpdateVehicleDtoValidator(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        //    RuleFor(v => v.Id)
        //        .GreaterThan(0).WithMessage("A valid Vehicle ID is required for updates.");

        //    RuleFor(v => v.Make)
        //        .NotEmpty().WithMessage("Make is required.")
        //        .MaximumLength(50).WithMessage("Make cannot exceed 50 characters.");

        //    RuleFor(v => v.Model)
        //        .NotEmpty().WithMessage("Model is required.")
        //        .MaximumLength(50).WithMessage("Model cannot exceed 50 characters.");

        //    RuleFor(v => v.Year)
        //        .InclusiveBetween(1900, DateTime.Now.Year + 2).WithMessage($"Year must be between 1900 and {DateTime.Now.Year + 2}.");

        //    // --- Uniqueness Check (excluding self) ---
        //    RuleFor(v => v)
        //        .MustAsync(async (dto, cancellationToken) =>
        //        {
        //            if (string.IsNullOrWhiteSpace(dto.Make) || string.IsNullOrWhiteSpace(dto.Model) || dto.Year < 1900 || dto.Id <= 0)
        //                return true; // Let other rules handle validation

        //            // Use the service method, passing the ID to exclude
        //            return !await _unitOfWork.Vehicles.MakeModelYearExistsAsync(dto.Make, dto.Model, dto.Year, dto.Id);
        //        })
        //        .WithMessage(dto => $"Another vehicle with Make '{dto.Make}', Model '{dto.Model}', and Year '{dto.Year}' already exists.")
        //        .When(dto => !string.IsNullOrWhiteSpace(dto.Make) && !string.IsNullOrWhiteSpace(dto.Model) && dto.Year >= 1900 && dto.Id > 0);


        //    // --- Optional VIN Validation (excluding self) ---
        //    When(v => !string.IsNullOrWhiteSpace(v.VIN), () =>
        //    {
        //        RuleFor(v => v.VIN)
        //            .Length(17).WithMessage("VIN must be exactly 17 characters if provided.")
        //            // .Matches("^[A-HJ-NPR-Z0-9]{17}$") // Optional Regex
        //            .MustAsync(async (dto, vin, cancellationToken) => // Get dto via overload
        //            {
        //                if (string.IsNullOrWhiteSpace(vin) || dto.Id <= 0) return true;
        //                // Use service method, passing the ID to exclude
        //                return !await _unitOfWork.Vehicles.VinExistsAsync(vin, dto.Id);
        //            })
        //            .WithMessage(v => $"VIN '{v.VIN}' already exists.");
        //    });

        //    // --- Optional Other Fields ---
        //    RuleFor(v => v.Engine)
        //       .MaximumLength(50).WithMessage("Engine description cannot exceed 50 characters.");
        //    RuleFor(v => v.Transmission)
        //       .MaximumLength(50).WithMessage("Transmission description cannot exceed 50 characters.");
        //    RuleFor(v => v.TrimLevel)
        //       .MaximumLength(50).WithMessage("Trim Level cannot exceed 50 characters.");
        //    RuleFor(v => v.BodyType)
        //       .MaximumLength(50).WithMessage("Body Type cannot exceed 50 characters.");
        //}
    }
}
