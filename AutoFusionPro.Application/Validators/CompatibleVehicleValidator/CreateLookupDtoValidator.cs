using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class CreateLookupDtoValidator : AbstractValidator<CreateLookupDto>
    {
        public CreateLookupDtoValidator() // IUnitOfWork can be injected for more complex generic lookup validation
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            // Generic uniqueness check would require knowing the entity type;
            // often best handled in the specific service method or a more specific validator.
        }
    }
}
