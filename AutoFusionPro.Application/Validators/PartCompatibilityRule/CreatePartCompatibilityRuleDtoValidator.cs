using AutoFusionPro.Application.DTOs.PartCompatibilityDtos;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models.CompatibleVehicleModels;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.PartCompatibilityRule
{
    public class CreatePartCompatibilityRuleDtoValidator : AbstractValidator<CreatePartCompatibilityRuleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePartCompatibilityRuleDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Rule name is required.")
                .MaximumLength(150).WithMessage("Rule name cannot exceed 150 characters.");
            // Optional: Add uniqueness check for rule name *per part* if desired, or globally if templates.
            // This would require PartId to be passed to the validator or validated in the service.

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Rule description cannot exceed 500 characters.");

            // Validate MakeId if provided
            When(x => x.MakeId.HasValue && x.MakeId.Value > 0, () => {
                RuleFor(x => x.MakeId)
                    .MustAsync(async (id, token) => await _unitOfWork.ExistsAsync<Make>(m => m.Id == id!.Value, token))
                    .WithMessage("Selected Make does not exist.");
            });

            // Validate ModelId if provided (and MakeId must also be provided)
            When(x => x.ModelId.HasValue && x.ModelId.Value > 0, () => {
                RuleFor(x => x.ModelId)
                    .NotEmpty().When(x => x.ApplicableTrimLevels != null && x.ApplicableTrimLevels.Any(), ApplyConditionTo.CurrentValidator)
                        .WithMessage("Model must be selected if specific trim levels are chosen.")
                    .MustAsync(async (dto, modelId, token) => // Check if model belongs to make
                    {
                        if (!dto.MakeId.HasValue || dto.MakeId.Value == 0) return false; // Make must be selected for a model
                        var model = await _unitOfWork.Models.GetByIdAsync(modelId!.Value);
                        return model != null && model.MakeId == dto.MakeId.Value;
                    })
                    .WithMessage("Selected Model does not exist or does not belong to the selected Make.");
            }).Otherwise(() => { // If ModelId is null or 0
                RuleFor(x => x.ApplicableTrimLevels)
                    .Must(trims => trims == null || !trims.Any())
                    .WithMessage("Specific trim levels cannot be selected if no Model is chosen.");
            });


            // Validate Year Range
            When(x => x.YearStart.HasValue || x.YearEnd.HasValue, () => {
                RuleFor(x => x.YearStart)
                    .LessThanOrEqualTo(x => x.YearEnd)
                    .When(x => x.YearStart.HasValue && x.YearEnd.HasValue)
                    .WithMessage("Year Start must be less than or equal to Year End.");
                RuleFor(x => x.YearStart.Value)
                    .InclusiveBetween(1900, DateTime.Now.Year + 10) // Example range
                    .When(x => x.YearStart.HasValue)
                    .WithMessage($"Year Start must be between 1900 and {DateTime.Now.Year + 10}.");
                RuleFor(x => x.YearEnd.Value)
                    .InclusiveBetween(1900, DateTime.Now.Year + 10)
                    .When(x => x.YearEnd.HasValue)
                    .WithMessage($"Year End must be between 1900 and {DateTime.Now.Year + 10}.");
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

        //private void ValidateApplicableAttributeList<T TaxonomyEntity>(
        //    System.Linq.Expressions.Expression<Func<CreatePartCompatibilityRuleDto, List<PartCompatibilityRuleApplicableAttributeDto>?>> propertyExpression,
        //    string entityName) where TTaxonomyEntity : BaseEntity
        //{
        //    RuleForEach(propertyExpression)
        //        .ChildRules(item =>
        //        {
        //            item.RuleFor(x => x.AttributeId)
        //                .GreaterThan(0).WithMessage($"{entityName} ID must be valid.")
        //                .MustAsync(async (id, token) => await _unitOfWork.ExistsAsync<TTaxonomyEntity>(e => e.Id == id, token))
        //                .WithMessage(x => $"Selected {entityName.ToLower()} with ID '{x.AttributeId}' does not exist.");
        //            // AttributeName is for display, not typically validated here
        //            // IsExclusion is a boolean, no specific validation needed beyond type
        //        })
        //        .When(dto => propertyExpression.Compile()(dto) != null); // Only validate items if the list itself is not null
        //}
    }
}
