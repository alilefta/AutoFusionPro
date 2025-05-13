using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class CompatibleVehiclesViewModel : BaseViewModel<CompatibleVehiclesViewModel>, ITabViewModel
    {

        #region Private Fields

        private readonly ICompatibleVehicleService _compatibleVehicleService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWpfToastNotificationService _toastNotificationService;
        private readonly IDialogService _dialogService;

        #endregion


        [ObservableProperty]
        private bool _isVisible = false;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _displayName = "Compatible Vehicle Types";

        [ObservableProperty]
        private string _icon = "";


        public CompatibleVehiclesViewModel(
            ICompatibleVehicleService compatibleVehicleService,
            IWpfToastNotificationService wpfToastNotificationService,
            ILocalizationService localizationService,
            IServiceProvider serviceProvider,
            ILogger<CompatibleVehiclesViewModel> logger,
            IDialogService dialogService): base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _toastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));


            // Initial load on construction or via an explicit Initialize command
            //_ = InitializeViewModelAsync();
        }

    }
}
