using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class UpdateLookupDtoValidator : AbstractValidator<UpdateLookupDto>
    {
        public UpdateLookupDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
        }
    }
}
