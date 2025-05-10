using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public class TransmissionTypesManagementViewModel : BaseViewModel<TransmissionTypesManagementViewModel>, ITabViewModel
    {
        public TransmissionTypesManagementViewModel(ILocalizationService localizationService,
                ILogger<TransmissionTypesManagementViewModel> logger) : base(localizationService, logger)
        {

        }
    }
}
