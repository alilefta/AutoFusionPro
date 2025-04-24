using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;

namespace AutoFusionPro.UI.ViewModels.Controls.Dialogs
{
    public class ConfirmLogoutViewModel : BaseViewModel
    {
        private readonly ILocalizationService _localizationService;


        public ConfirmLogoutViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _localizationService.FlowDirectionChanged += OnCurrentWorkFlowChanged;
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;

            RegisterCleanup(() => _localizationService.FlowDirectionChanged -= OnCurrentWorkFlowChanged);
        }

        private void OnCurrentWorkFlowChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }
    }
}
