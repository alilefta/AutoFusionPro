using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class EngineTypesManagementViewModel : BaseViewModel<EngineTypesManagementViewModel>, ITabViewModel
    {
        [ObservableProperty]
        private bool _isVisible = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _displayName = "Engine Types";

        [ObservableProperty]
        private string _icon = "";


        public EngineTypesManagementViewModel(ILocalizationService localizationService,
                ILogger<EngineTypesManagementViewModel> logger) : base(localizationService, logger)
        {

        }
    }
}
