using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoFusionPro.UI.Helpers.Filtration
{
    public partial class ActiveFilterDisplayItem : ObservableObject
    {
        [ObservableProperty]
        private string _filterType; // e.g., "Make", "Model", "Year"

        [ObservableProperty]
        private string _filterValueDisplay; // e.g., "Nissan", "Altima", "2020"

        public object OriginalCriterion { get; } // To know what to remove (e.g., the MakeId)
        public string CriterionKey { get; } // e.g., "MakeId", "ModelId"

        public ActiveFilterDisplayItem(string filterType, string filterValueDisplay, string criterionKey, object originalCriterion)
        {
            _filterType = filterType;
            _filterValueDisplay = filterValueDisplay;
            CriterionKey = criterionKey;
            OriginalCriterion = originalCriterion;
        }
    }
}
