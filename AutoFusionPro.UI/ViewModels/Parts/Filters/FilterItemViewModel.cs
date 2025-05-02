using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoFusionPro.UI.ViewModels.Parts.Filters
{
    public partial class FilterItemViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _name = string.Empty;

        public FilterItemViewModel() 
        { 

        }

        public FilterItemViewModel(string name)
        {
            Name = name;
        }
    }
}
