using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public class BodyTypesManagementViewModel : BaseViewModel<BodyTypesManagementViewModel>, ITabViewModel
    {
        public BodyTypesManagementViewModel(ILocalizationService localizationService,
                ILogger<BodyTypesManagementViewModel> logger) : base(localizationService, logger)
        {
            
        }
    }
}
