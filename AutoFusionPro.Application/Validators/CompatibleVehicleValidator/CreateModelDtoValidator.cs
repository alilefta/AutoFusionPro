using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class CreateModelDtoValidator : AbstractValidator<CreateModelDto>
    {
        public CreateModelDtoValidator(IUnitOfWork unitOfWork) // Inject IUnitOfWork
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Model name is required.")
                .MaximumLength(100).WithMessage("Model name cannot exceed 100 characters.");

            RuleFor(x => x.MakeId)
                .GreaterThan(0).WithMessage("A valid Make must be selected.")
                // Check if MakeId exists
                .MustAsync(async (id, token) => await unitOfWork.Makes.ExistsAsync(m => m.Id == id))
                .WithMessage("Selected Make does not exist.");

            // Uniqueness check for Name within the Make
            RuleFor(x => x)
                .MustAsync(async (dto, token) =>
                {
                    if (string.IsNullOrWhiteSpace(dto.Name) || dto.MakeId <= 0) return true; // Handled by other rules
                    return !await unitOfWork.Models.NameExistsForMakeAsync(dto.Name, dto.MakeId);
                })
                .WithMessage(dto => $"Model name '{dto.Name}' already exists for the selected Make.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name) && x.MakeId > 0);
        }
    }
}
