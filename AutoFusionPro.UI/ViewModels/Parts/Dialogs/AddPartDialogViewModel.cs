using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs
{
    public partial class AddPartDialogViewModel : BaseViewModel
    {
        private readonly IPartService _partService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<AddPartDialogViewModel> _logger;

        [ObservableProperty]
        private bool _isAddingPart = false;

        #region Part Properties

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _partNumber = string.Empty;


        #endregion

        public AddPartDialogViewModel(IPartService partService, 
            ILocalizationService localizationService, 
            ILogger<AddPartDialogViewModel> logger)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
            _localizationService.FlowDirectionChanged += OnCurrentFlowDirectionChanged;

            RegisterCleanup(() => _localizationService.FlowDirectionChanged -= OnCurrentFlowDirectionChanged);

        }



        #region Helpers

        private void OnCurrentFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }

        #endregion

    }
}
