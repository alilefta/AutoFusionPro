using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Parts.Filters
{
    public partial class FilterItemViewModel : BaseViewModel<FilterItemViewModel>   
    {
        [ObservableProperty]
        private string _name = string.Empty;

        public FilterItemViewModel(ILocalizationService localizationService, ILogger<FilterItemViewModel> logger) : base(localizationService, logger)
        { 

        }

        //public FilterItemViewModel(string name)
        //{
        //    Name = name;
        //}
    }
}
