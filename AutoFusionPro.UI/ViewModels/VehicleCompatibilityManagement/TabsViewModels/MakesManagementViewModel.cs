using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class MakesManagementViewModel : BaseViewModel<MakesManagementViewModel>, ITabViewModel
    {
        public MakesManagementViewModel(ILocalizationService localizationService,
                ILogger<MakesManagementViewModel> logger) : base(localizationService, logger)
        {

        }
    }
}
