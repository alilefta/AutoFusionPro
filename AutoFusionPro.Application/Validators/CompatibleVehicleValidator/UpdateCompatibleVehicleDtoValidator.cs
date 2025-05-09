using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class UpdateCompatibleVehicleDtoValidator : AbstractValidator<UpdateCompatibleVehicleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCompatibleVehicleDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Compatible Vehicle ID.");

            RuleFor(x => x.ModelId)
                .GreaterThan(0).WithMessage("A valid Model must be selected.");
            // .MustAsync(async (id, token) => await _unitOfWork.Models.ExistsAsync(m => m.Id == id))
            // .WithMessage("Selected Model does not exist.");

            RuleFor(x => x.YearStart)
                .InclusiveBetween(1900, DateTime.Now.Year + 5).WithMessage($"Start year must be between 1900 and {DateTime.Now.Year + 5}.");

            RuleFor(x => x.YearEnd)
                .InclusiveBetween(1900, DateTime.Now.Year + 10).WithMessage($"End year must be between 1900 and {DateTime.Now.Year + 10}.")
                .GreaterThanOrEqualTo(x => x.YearStart).WithMessage("End year must be greater than or equal to start year.");

            // Optional: Check existence of FKs if IDs are provided (similar to Create validator)
            When(x => x.TrimLevelId.HasValue, () => { /* ... */ });
            // ...

            RuleFor(x => x.VIN)
                .MaximumLength(20).WithMessage("VIN cannot exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9]*$").When(x => !string.IsNullOrEmpty(x.VIN)).WithMessage("VIN can only contain letters and numbers.");


            // Uniqueness check for the specification, excluding itself
            RuleFor(x => x)
                .MustAsync(async (dto, token) =>
                {
                    return !await _unitOfWork.CompatibleVehicles.SpecificationExistsAsync(
                        dto.ModelId, dto.YearStart, dto.YearEnd,
                        dto.TrimLevelId, dto.TransmissionTypeId, dto.EngineTypeId, dto.BodyTypeId,
                        dto.Id // exclude its own ID during update
                    );
                })
                .WithMessage("This exact vehicle specification (Model, Years, Trim, etc.) already exists for another record.")
                .When(x => x.ModelId > 0 && x.Id > 0);
        }
    }
}
