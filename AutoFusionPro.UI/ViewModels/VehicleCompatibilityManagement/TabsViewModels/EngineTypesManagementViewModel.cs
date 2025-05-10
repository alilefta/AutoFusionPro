using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public class EngineTypesManagementViewModel : BaseViewModel<EngineTypesManagementViewModel>, ITabViewModel
    {
        public EngineTypesManagementViewModel(ILocalizationService localizationService,
                ILogger<EngineTypesManagementViewModel> logger) : base(localizationService, logger)
        {

        }
    }
}
