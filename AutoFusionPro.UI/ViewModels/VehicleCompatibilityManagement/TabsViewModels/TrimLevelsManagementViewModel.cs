using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public class TrimLevelsManagementViewModel : BaseViewModel<TrimLevelsManagementViewModel>, ITabViewModel
    {
        public TrimLevelsManagementViewModel(ILocalizationService localizationService,
                ILogger<TrimLevelsManagementViewModel> logger) : base(localizationService, logger)
        {

        }
    }
}
