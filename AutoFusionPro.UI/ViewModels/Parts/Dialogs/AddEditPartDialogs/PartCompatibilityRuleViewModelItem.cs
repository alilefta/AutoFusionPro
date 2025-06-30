using AutoFusionPro.Application.DTOs.PartCompatibilityDtos;
using AutoFusionPro.Application.Interfaces.DataServices;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEditPartDialogs
{
    public partial class PartCompatibilityRuleViewModelItem : ObservableObject
    {
        // --- State Properties ---
        public bool IsNew { get; }
        public PartCompatibilityRuleSummaryDto? ExistingRule { get; }
        public CreatePartCompatibilityRuleDto? NewRuleData { get; }

        // --- Display Properties for DataGrid Binding ---
        public string Name => IsNew ? NewRuleData!.Name : ExistingRule!.Name;
        public bool IsActive => IsNew ? NewRuleData!.IsActive : ExistingRule!.IsActive;

        [ObservableProperty]
        private string _makeNameDisplay = "Loading...";
        [ObservableProperty]
        private string _modelNameDisplay = "Loading...";
        [ObservableProperty]
        private string _yearRangeDisplay = "Loading...";
        [ObservableProperty]
        private string _trimsDisplay = "Loading...";
        [ObservableProperty]
        private string _enginesDisplay = "Loading...";
        [ObservableProperty]
        private string _transmissionsDisplay = "Loading...";
        [ObservableProperty]
        private string _bodyTypesDisplay = "Loading...";

        // --- Constructor for EXISTING rules ---
        public PartCompatibilityRuleViewModelItem(PartCompatibilityRuleSummaryDto existingRule)
        {
            IsNew = false;
            ExistingRule = existingRule;
            NewRuleData = null;

            // Populate display properties directly from the summary DTO
            MakeNameDisplay = existingRule.MakeNameDisplay;
            ModelNameDisplay = existingRule.ModelNameDisplay;
            YearRangeDisplay = existingRule.YearRangeDisplay;
            TrimsDisplay = existingRule.TrimsDisplay;
            EnginesDisplay = existingRule.EnginesDisplay;
            TransmissionsDisplay = existingRule.TransmissionsDisplay;
            BodyTypesDisplay = existingRule.BodyTypesDisplay;
        }

        // --- Constructor for NEW (pending) rules ---
        public PartCompatibilityRuleViewModelItem(CreatePartCompatibilityRuleDto newRule, IVehicleTaxonomyService taxonomyService)
        {
            IsNew = true;
            ExistingRule = null;
            NewRuleData = newRule;

            // Asynchronously build the display strings by fetching names for IDs
            _ = BuildDisplayStringsAsync(taxonomyService);
        }

        // This helper method is called for NEW rules to translate their IDs into displayable names.
        private async Task BuildDisplayStringsAsync(IVehicleTaxonomyService taxonomyService)
        {
            var res = System.Windows.Application.Current.Resources;

            var anyMakeStr = res["AnyMakeStr"] as string ?? "Any Make";
            var anyModelStr = res["AnyModelStr"] as string ?? "Any Model";
            var anyStr = res["AnyStr"] as string ?? "Any";
            var fromStr = res["FromStr"] as string ?? "From";
            var upToStr = res["UpToStr"] as string ?? "Up To";
            var anyYearStr = res["AnyYearStr"] as string ?? "Any Year";

            // Make & Model
            var make = NewRuleData!.MakeId.HasValue ? await taxonomyService.GetMakeByIdAsync(NewRuleData.MakeId.Value) : null;
            var model = NewRuleData.ModelId.HasValue ? await taxonomyService.GetModelByIdAsync(NewRuleData.ModelId.Value) : null;
            MakeNameDisplay = make?.Name ?? "Any Make";
            ModelNameDisplay = model?.Name ?? (make != null ? anyModelStr : anyStr);

            // Year Range
            if (NewRuleData.YearStart.HasValue && NewRuleData.YearEnd.HasValue)
            {
                YearRangeDisplay = NewRuleData.YearStart == NewRuleData.YearEnd
                    ? NewRuleData.YearStart.Value.ToString()
                    : $"{NewRuleData.YearStart.Value} - {NewRuleData.YearEnd.Value}";
            }
            else if (NewRuleData.YearStart.HasValue) YearRangeDisplay = $"{fromStr} {NewRuleData.YearStart.Value}";
            else if (NewRuleData.YearEnd.HasValue) YearRangeDisplay = $"{upToStr} {NewRuleData.YearEnd.Value}";
            else YearRangeDisplay = anyYearStr;

            // Attributes (Trims, Engines, etc.)

            var trimStr = res["TrimStr"] as string ?? "Trim";
            var engineStr = res["EngineStr"] as string ?? "Engine";
            var BodyStr = res["BodyStr"] as string ?? "Body";
            var transmissionStr = res["TransmissionStr"] as string ?? "Transmission";



            TrimsDisplay = FormatAttributesForDisplay(NewRuleData.ApplicableTrimLevels, trimStr);
            EnginesDisplay = FormatAttributesForDisplay(NewRuleData.ApplicableEngineTypes, engineStr);
            TransmissionsDisplay = FormatAttributesForDisplay(NewRuleData.ApplicableTransmissionTypes, transmissionStr);
            BodyTypesDisplay = FormatAttributesForDisplay(NewRuleData.ApplicableBodyTypes, BodyStr);

            // Notify that properties have been updated after async operation
            OnPropertyChanged(nameof(MakeNameDisplay));
            OnPropertyChanged(nameof(ModelNameDisplay));
            OnPropertyChanged(nameof(YearRangeDisplay));
            OnPropertyChanged(nameof(TrimsDisplay));
            OnPropertyChanged(nameof(EnginesDisplay));
            OnPropertyChanged(nameof(TransmissionsDisplay));
            OnPropertyChanged(nameof(BodyTypesDisplay));
        }

        // Helper to format attributes for display, mirroring logic from the service's summary mapping.
        private string FormatAttributesForDisplay(List<PartCompatibilityRuleApplicableAttributeDto>? attributes, string attributeTypeSingular)
        {
            if (attributes == null || !attributes.Any(a => a.AttributeId > 0))
            {
                var anyStr = System.Windows.Application.Current.Resources["AnyStr"] as string ?? "Any";

                return $"{anyStr} {attributeTypeSingular}";
            }

            var included = attributes.Where(a => !a.IsExclusion).Select(a => a.AttributeName).ToList();
            var excluded = attributes.Where(a => a.IsExclusion).Select(a => a.AttributeName).ToList();

            string result = "";
            if (included.Any())
            {
                result = string.Join(", ", included.OrderBy(n => n));
            }
            else // Only exclusions implies "All except..."
            {
                var allStr = System.Windows.Application.Current.Resources["AllStr"] as string ?? "All";

                result = $"{allStr} {attributeTypeSingular}s";
            }

            if (excluded.Any())
            {
                var excludeStr = System.Windows.Application.Current.Resources["ExcludesStr"] as string ?? "Exclude";
                result += $" ({excludeStr}: {string.Join(", ", excluded.OrderBy(n => n))})";
            }
            return result.Trim();
        }
    }
}
