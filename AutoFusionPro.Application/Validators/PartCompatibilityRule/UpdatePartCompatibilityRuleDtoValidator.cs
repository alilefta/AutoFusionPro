using AutoFusionPro.Application.DTOs.PartCompatibilityDtos;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.PartCompatibilityRule
{
    public class UpdatePartCompatibilityRuleDtoValidator : AbstractValidator<UpdatePartCompatibilityRuleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePartCompatibilityRuleDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Rule ID is invalid.")
                .MustAsync(async (id, token) => await _unitOfWork.ExistsAsync<Domain.Models.PartCompatibilityRule>(r => r.Id == id, token)) // Assuming UoW.PartCompatibilityRules
                .WithMessage("Compatibility Rule to update was not found.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Rule name is required.")
                .MaximumLength(150).WithMessage("Rule name cannot exceed 150 characters.");
            // Optional: Uniqueness check for rule name *per part*, excluding self.
            // This would need PartId and excludeRuleId (dto.Id). Service layer is a good place for this.

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Rule description cannot exceed 500 characters.");

            // Validate MakeId if provided
            When(x => x.MakeId.HasValue && x.MakeId.Value > 0, () => {
                RuleFor(x => x.MakeId)
                    .MustAsync(async (id, token) => await _unitOfWork.ExistsAsync<Make>(m => m.Id == id!.Value, token))
                    .WithMessage("Selected Make does not exist.");
            });

            // Validate ModelId if provided
            When(x => x.ModelId.HasValue && x.ModelId.Value > 0, () => {
                RuleFor(x => x.ModelId)
                    .MustAsync(async (dto, modelId, token) =>
                    {
                        if (!dto.MakeId.HasValue || dto.MakeId.Value == 0) return false;
                        var model = await _unitOfWork.Models.GetByIdAsync(modelId!.Value);
                        return model != null && model.MakeId == dto.MakeId.Value;
                    })
                    .WithMessage("Selected Model does not exist or does not belong to the selected Make.");
            });

            RuleFor(x => x.ModelId)
                .NotEmpty().WithMessage("Model must be selected if specific trim levels are chosen.")
                .When(x => x.ApplicableTrimLevels != null && x.ApplicableTrimLevels.Any(tl => tl.AttributeId > 0));


            // Validate Year Range
            When(x => x.YearStart.HasValue || x.YearEnd.HasValue, () => {
                RuleFor(x => x.YearStart)
                    .NotNull().When(x => x.YearEnd.HasValue).WithMessage("Year Start is required if Year End is specified.")
                    .LessThanOrEqualTo(x => x.YearEnd)
                    .When(x => x.YearStart.HasValue && x.YearEnd.HasValue)
                    .WithMessage("Year Start must be less than or equal to Year End.");

                RuleFor(x => x.YearEnd)
                    .NotNull().When(x => x.YearStart.HasValue).WithMessage("Year End is required if Year Start is specified.");

                RuleFor(x => x.YearStart.Value)
                    .InclusiveBetween(1900, DateTime.Now.Year + 15)
                    .When(x => x.YearStart.HasValue)
                    .WithMessage($"Year Start must be between 1900 and {DateTime.Now.Year + 15}.");

                RuleFor(x => x.YearEnd.Value)
                    .InclusiveBetween(1900, DateTime.Now.Year + 15)
                    .When(x => x.YearEnd.HasValue)
                    .WithMessage($"Year End must be between 1900 and {DateTime.Now.Year + 15}.");
            });

            // Validate ApplicableTrimLevels
            RuleFor(dto => dto) // Validate the root DTO to access both ModelId and ApplicableTrimLevels
                .CustomAsync(async (dto, context, token) => {
                    if (dto.ApplicableTrimLevels != null && dto.ApplicableTrimLevels.Any(tl => tl.AttributeId > 0))
                    {
                        if (!dto.ModelId.HasValue || dto.ModelId.Value == 0)
                        {
                            context.AddFailure(nameof(dto.ApplicableTrimLevels), "A Model must be selected to specify Trim Levels.");
                            return;
                        }
                        foreach (var trimAttr in dto.ApplicableTrimLevels.Where(ta => ta.AttributeId > 0))
                        {
                            var trimLevel = await _unitOfWork.TrimLevels.GetByIdAsync(trimAttr.AttributeId); // Use specific repo
                            if (trimLevel == null)
                            {
                                context.AddFailure($"{nameof(dto.ApplicableTrimLevels)}[{dto.ApplicableTrimLevels.IndexOf(trimAttr)}].{nameof(trimAttr.AttributeId)}", $"Trim Level with ID '{trimAttr.AttributeId}' does not exist.");
                            }
                            else if (trimLevel.ModelId != dto.ModelId.Value)
                            {
                                context.AddFailure($"{nameof(dto.ApplicableTrimLevels)}[{dto.ApplicableTrimLevels.IndexOf(trimAttr)}].{nameof(trimAttr.AttributeId)}", $"Trim Level '{trimLevel.Name}' (ID: {trimAttr.AttributeId}) does not belong to the selected Model.");
                            }
                        }
                    }
                });

            // Validate ApplicableEngineTypes
            RuleForEach(x => x.ApplicableEngineTypes)
                .ChildRules(item => {
                    item.RuleFor(attr => attr.AttributeId)
                        .GreaterThan(0).WithMessage("Engine Type ID must be valid.")
                        .MustAsync(async (id, token) => await _unitOfWork.ExistsAsync<EngineType>(et => et.Id == id, token)) // Use specific repo
                        .WithMessage(attr => $"Selected Engine Type with ID '{attr.AttributeId}' does not exist.");
                })
                .When(dto => dto.ApplicableEngineTypes != null && dto.ApplicableEngineTypes.Any(et => et.AttributeId > 0));

            // Validate ApplicableTransmissionTypes
            RuleForEach(x => x.ApplicableTransmissionTypes)
                .ChildRules(item => {
                    item.RuleFor(attr => attr.AttributeId)
                        .GreaterThan(0).WithMessage("Transmission Type ID must be valid.")
                        .MustAsync(async (id, token) => await _unitOfWork.ExistsAsync<TransmissionType>(tt => tt.Id == id, token)) // Use specific repo
                        .WithMessage(attr => $"Selected Transmission Type with ID '{attr.AttributeId}' does not exist.");
                })
                .When(dto => dto.ApplicableTransmissionTypes != null && dto.ApplicableTransmissionTypes.Any(tt => tt.AttributeId > 0));

            // Validate ApplicableBodyTypes
            RuleForEach(x => x.ApplicableBodyTypes)
                .ChildRules(item => {
                    item.RuleFor(attr => attr.AttributeId)
                        .GreaterThan(0).WithMessage("Body Type ID must be valid.")
                        .MustAsync(async (id, token) => await _unitOfWork.ExistsAsync<BodyType>(bt => bt.Id == id, token)) // Use specific repo
                        .WithMessage(attr => $"Selected Body Type with ID '{attr.AttributeId}' does not exist.");
                })
                .When(dto => dto.ApplicableBodyTypes != null && dto.ApplicableBodyTypes.Any(bt => bt.AttributeId > 0));

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Rule notes cannot exceed 1000 characters.");
        }
    }
}
