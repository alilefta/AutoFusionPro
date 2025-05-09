using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class CreateCompatibleVehicleDtoValidator : AbstractValidator<CreateCompatibleVehicleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompatibleVehicleDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; // For DB checks

            RuleFor(x => x.ModelId)
                .GreaterThan(0).WithMessage("A valid Model must be selected.");
            // Optional: Check if ModelId exists in the database
            // .MustAsync(async (id, token) => await _unitOfWork.Models.ExistsAsync(m => m.Id == id))
            // .WithMessage("Selected Model does not exist.");

            RuleFor(x => x.YearStart)
                .InclusiveBetween(1900, DateTime.Now.Year + 5).WithMessage($"Start year must be between 1900 and {DateTime.Now.Year + 5}.");

            RuleFor(x => x.YearEnd)
                .InclusiveBetween(1900, DateTime.Now.Year + 10).WithMessage($"End year must be between 1900 and {DateTime.Now.Year + 10}.")
                .GreaterThanOrEqualTo(x => x.YearStart).WithMessage("End year must be greater than or equal to start year.");

            // Optional: Check existence of FKs if IDs are provided
            When(x => x.TrimLevelId.HasValue, () => {
                RuleFor(x => x.TrimLevelId)
                    .MustAsync(async (id, token) => id.HasValue && await _unitOfWork.TrimLevels.ExistsAsync(t => t.Id == id.Value))
                    .WithMessage("Selected Trim Level does not exist.");
            });
            When(x => x.TransmissionTypeId.HasValue, () => {
                RuleFor(x => x.TransmissionTypeId)
                   .MustAsync(async (id, token) => id.HasValue && await _unitOfWork.TransmissionTypes.ExistsAsync(t => t.Id == id.Value))
                   .WithMessage("Selected Transmission Type does not exist.");
            });
            // ... Add similar rules for EngineTypeId, BodyTypeId ...

            RuleFor(x => x.VIN)
                .MaximumLength(20).WithMessage("VIN cannot exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9]*$").When(x => !string.IsNullOrEmpty(x.VIN)).WithMessage("VIN can only contain letters and numbers.");
            // Note: True VIN validation is complex; this is a basic check.

            // Uniqueness for the entire specification (handled in service or here via MustAsync)
            // Example of how it could be done here (more complex due to many parameters)
            RuleFor(x => x) // Rule for the whole object
                .MustAsync(async (dto, token) =>
                {
                    // Call repository method directly as service isn't available here
                    // This assumes ICompatibleVehicleRepository is available on IUnitOfWork
                    return !await _unitOfWork.CompatibleVehicles.SpecificationExistsAsync(
                        dto.ModelId, dto.YearStart, dto.YearEnd,
                        dto.TrimLevelId, dto.TransmissionTypeId, dto.EngineTypeId, dto.BodyTypeId,
                        null // excludeCompatibleVehicleId is null for creation
                    );
                })
                .WithMessage("This exact vehicle specification (Model, Years, Trim, etc.) already exists.")
                .When(x => x.ModelId > 0); // Only run if ModelId is valid
        }
    }


}
