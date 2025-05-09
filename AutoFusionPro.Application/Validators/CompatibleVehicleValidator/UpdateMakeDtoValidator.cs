using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class UpdateMakeDtoValidator : AbstractValidator<UpdateMakeDto>
    {
        public UpdateMakeDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Make ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Make name is required.")
                .MaximumLength(100).WithMessage("Make name cannot exceed 100 characters.");
            // Uniqueness check (excluding self) handled in service layer
        }
    }
}
