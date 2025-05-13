using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class TransmissionTypesManagementViewModel : BaseViewModel<TransmissionTypesManagementViewModel>, ITabViewModel
    {
        [ObservableProperty]
        private bool _isVisible = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _displayName = "Transmission Types";

        [ObservableProperty]
        private string _icon = "";

        public TransmissionTypesManagementViewModel(ILocalizationService localizationService,
                ILogger<TransmissionTypesManagementViewModel> logger) : base(localizationService, logger)
        {

        }
    }
}
