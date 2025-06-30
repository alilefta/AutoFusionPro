using AutoFusionPro.Application.DTOs.PartImage;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.PartImage
{
    public class CreatePartImageDtoValidator : AbstractValidator<CreatePartImageDto>
    {
        public CreatePartImageDtoValidator()
        {
            RuleFor(x => x.Caption)
                .MaximumLength(255).WithMessage("Caption cannot exceed 255 characters.");
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be non-negative.");
            // PartId is validated in the service method.
            // IsPrimary is a boolean, no specific validation needed here beyond type.
        }
    }
}
