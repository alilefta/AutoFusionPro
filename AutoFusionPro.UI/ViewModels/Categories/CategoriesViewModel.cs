using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Categories
{
    public class CategoriesViewModel : BaseViewModel<CategoriesViewModel>
    {
        public CategoriesViewModel(ILocalizationService localizationService, ILogger<CategoriesViewModel> logger) : base(localizationService, logger)
        {
            
        }
    }
}
