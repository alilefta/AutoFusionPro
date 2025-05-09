using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class CreateEngineTypeDtoValidator : AbstractValidator<CreateEngineTypeDto>
    {
        public CreateEngineTypeDtoValidator(IUnitOfWork unitOfWork) // Inject UoW for DB checks
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Engine Type name is required.")
                .MaximumLength(150).WithMessage("Engine Type name cannot exceed 150 characters.")
                // Uniqueness for Name
                .MustAsync(async (name, token) =>
                    !await unitOfWork.EngineTypes.NameExistsAsync(name))
                .WithMessage(dto => $"Engine Type name '{dto.Name}' already exists.");

            RuleFor(x => x.Code)
                .MaximumLength(50).WithMessage("Engine Code cannot exceed 50 characters.")
                // Uniqueness for Code (only if provided)
                .MustAsync(async (code, token) =>
                    string.IsNullOrWhiteSpace(code) || !await unitOfWork.EngineTypes.CodeExistsAsync(code))
                .WithMessage(dto => $"Engine Code '{dto.Code}' already exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Code));
        }
    }
}
