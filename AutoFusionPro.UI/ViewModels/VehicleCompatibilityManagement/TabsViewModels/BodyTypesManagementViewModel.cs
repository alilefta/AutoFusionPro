using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class BodyTypesManagementViewModel : BaseViewModel<BodyTypesManagementViewModel>, ITabViewModel
    {
        [ObservableProperty]
        private bool _isVisible = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _displayName = "Body Types";        
        
        [ObservableProperty]
        private string _icon = "";


        public BodyTypesManagementViewModel(ILocalizationService localizationService,
                ILogger<BodyTypesManagementViewModel> logger) : base(localizationService, logger)
        {
            
        }

        #region Loading Data

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Commands

        #endregion


    }
}
