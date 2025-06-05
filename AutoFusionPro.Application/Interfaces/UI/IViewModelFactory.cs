using AutoFusionPro.Core.Enums.NavigationPages;

namespace AutoFusionPro.Application.Interfaces.UI
{
    public interface IViewModelFactory
    {
        object ResolveViewModel(ApplicationPage page);
        Task<object> ResolveViewModelWithInitialization(ApplicationPage page, object parameter);
    }
}
