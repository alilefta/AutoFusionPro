using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class UpdateEngineTypeDtoValidator : AbstractValidator<UpdateEngineTypeDto>
    {
        public UpdateEngineTypeDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Engine Type ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Engine Type name is required.")
                .MaximumLength(150).WithMessage("Engine Type name cannot exceed 150 characters.")
                // Uniqueness for Name (excluding self)
                .MustAsync(async (dto, name, token) => // context overload for dto.Id
                    !await unitOfWork.EngineTypes.NameExistsAsync(name, dto.Id))
                .WithMessage(dto => $"Engine Type name '{dto.Name}' already exists.");


            RuleFor(x => x.Code)
                .MaximumLength(50).WithMessage("Engine Code cannot exceed 50 characters.")
                // Uniqueness for Code (excluding self, only if provided)
                .MustAsync(async (dto, code, token) =>
                    string.IsNullOrWhiteSpace(code) || !await unitOfWork.EngineTypes.CodeExistsAsync(code, dto.Id))
                .WithMessage(dto => $"Engine Code '{dto.Code}' already exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Code));
        }
    }
}
