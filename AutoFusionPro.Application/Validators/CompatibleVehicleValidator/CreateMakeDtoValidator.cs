using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class CreateMakeDtoValidator : AbstractValidator<CreateMakeDto>
    {
        public CreateMakeDtoValidator() // IUnitOfWork can be injected if NameExistsAsync is not on repo
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Make name is required.")
                .MaximumLength(100).WithMessage("Make name cannot exceed 100 characters.");
            // Uniqueness check is handled in the service layer before DB call
            // or could be added here if IUnitOfWork (or IMakeRepository) is injected.
        }
    }


}
