using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement
{
    public partial class VehicleCompatibilityShellViewModel : BaseViewModel<VehicleCompatibilityShellViewModel>
    {

        #region General Properties

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private int _selectedTabIndex = -1;

        #endregion

        #region View Models 

        [ObservableProperty]
        private ObservableCollection<ITabViewModel> _tabViewModels; // ITabViewModel defines common props like DisplayName

        [ObservableProperty]
        private ITabViewModel _selectedTabViewModel;

        #endregion


        #region Constructor

        public VehicleCompatibilityShellViewModel(
                MakesModelsTrimsManagementViewModel makesModelsTrimsManagementVM,
                TransmissionTypesManagementViewModel transmissionTypesVM,
                EngineTypesManagementViewModel engineTypesVM,
                BodyTypesManagementViewModel bodyTypesVM,
                CompatibleVehiclesViewModel compatibleVehiclesVM,
                ILocalizationService localizationService,
                ILogger<VehicleCompatibilityShellViewModel> logger
            ) : base(localizationService, logger)
        {
           
            TabViewModels = new ObservableCollection<ITabViewModel>
            {
                makesModelsTrimsManagementVM,
                transmissionTypesVM,
                engineTypesVM,
                bodyTypesVM,
                compatibleVehiclesVM
            };

            SelectedTabViewModel = TabViewModels.FirstOrDefault(); // Select the first tab initially

            if(SelectedTabViewModel != null)
                SelectedTabViewModel.InitializeAsync();
        }

        #endregion

        partial void OnSelectedTabViewModelChanged(ITabViewModel value)
        {
            if (value != null)
            {

                value.InitializeAsync();

            }
        }

    }
}
