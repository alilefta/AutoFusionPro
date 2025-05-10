using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Controls.Dialogs
{
    public class ConfirmLogoutViewModel : BaseViewModel<ConfirmLogoutViewModel>
    {
        public ConfirmLogoutViewModel(ILocalizationService localizationService, ILogger<ConfirmLogoutViewModel> logger) : base(localizationService, logger)
        {
        }
    }
}
