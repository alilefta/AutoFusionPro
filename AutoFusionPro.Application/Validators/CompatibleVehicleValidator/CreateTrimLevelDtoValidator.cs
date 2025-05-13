using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class CreateTrimLevelDtoValidator : AbstractValidator<CreateTrimLevelDto>
    {
        public CreateTrimLevelDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Trim Level name is required.")
                .MaximumLength(100).WithMessage("Trim Level name cannot exceed 100 characters.");

            RuleFor(x => x.ModelId)
                .GreaterThan(0).WithMessage("A valid Model must be selected.")
                .MustAsync(async (id, token) => await unitOfWork.Models.ExistsAsync(m => m.Id == id))
                .WithMessage("Selected Model does not exist.");

            //RuleFor(x => x)
            //    .MustAsync(async (dto, token) =>
            //    {
            //        if (string.IsNullOrWhiteSpace(dto.Name) || dto.ModelId <= 0) return true;
            //        return !await unitOfWork.TrimLevels.NameExistsForModelAsync(dto.Name, dto.ModelId);
            //    })
            //    .WithMessage(dto => $"Trim Level name '{dto.Name}' already exists for the selected Model.")
            //    .When(x => !string.IsNullOrWhiteSpace(x.Name) && x.ModelId > 0);
        }
    }
}
