using AutoFusionPro.Application.DTOs.PartImage;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.PartImage
{
    public class UpdatePartImageDtoValidator : AbstractValidator<UpdatePartImageDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePartImageDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Part Image ID must be valid.")
                .MustAsync(async (id, token) => await _unitOfWork.ExistsAsync<Domain.Models.PartImage>(pi => pi.Id == id, token))
                .WithMessage(x => $"Part Image with ID '{x.Id}' not found.");

            RuleFor(x => x.Caption)
                .MaximumLength(255).WithMessage("Caption cannot exceed 255 characters.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be non-negative.");

            // IsPrimary is a boolean, no specific validation needed here beyond type.
            // The service layer will handle logic related to ensuring only one primary.
        }
    }
}
