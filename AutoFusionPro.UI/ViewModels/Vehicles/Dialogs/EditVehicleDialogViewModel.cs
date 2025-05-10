using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Vehicles.Dialogs
{
    public class EditVehicleDialogViewModel : BaseViewModel<EditVehicleDialogViewModel>
    {
        public EditVehicleDialogViewModel(ILocalizationService localizationService, ILogger<EditVehicleDialogViewModel> logger) : base(localizationService, logger)
        {
            
        }
    }
}
