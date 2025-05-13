using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.CompatibleVehicleValidator
{
    public class UpdateModelDtoValidator : AbstractValidator<UpdateModelDto>
    {
        public UpdateModelDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Model ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Model name is required.")
                .MaximumLength(100).WithMessage("Model name cannot exceed 100 characters.");

            RuleFor(x => x.MakeId)
                .GreaterThan(0).WithMessage("A valid Make must be selected.")
                .MustAsync(async (id, token) => await unitOfWork.Makes.ExistsAsync(m => m.Id == id))
                .WithMessage("Selected Make does not exist.");

            //// Uniqueness check for Name within the Make (excluding self)
            //RuleFor(x => x)
            //    .MustAsync(async (dto, token) =>
            //    {
            //        if (string.IsNullOrWhiteSpace(dto.Name) || dto.MakeId <= 0) return true;
            //        return !await unitOfWork.Models.NameExistsForMakeAsync(dto.Name, dto.MakeId, dto.Id);
            //    })
            //    .WithMessage(dto => $"Model name '{dto.Name}' already exists for the selected Make.")
            //    .When(x => !string.IsNullOrWhiteSpace(x.Name) && x.MakeId > 0 && x.Id > 0);
        }
    }
}
