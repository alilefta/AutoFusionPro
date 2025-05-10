using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public class ModelsManagementViewModel : BaseViewModel<ModelsManagementViewModel>, ITabViewModel
    {
        public ModelsManagementViewModel(ILocalizationService localizationService,
                ILogger<ModelsManagementViewModel> logger) : base(localizationService, logger)
        {

        }
    }
}
