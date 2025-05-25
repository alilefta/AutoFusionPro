using AutoFusionPro.Core.Enums.NavigationPages;
using System.Windows.Controls;

namespace AutoFusionPro.Application.Interfaces.Navigation
{
    /// <summary>
    /// Abstraction for the Navigation Service
    /// </summary>
    public interface INavigationService
    {
        ApplicationPage CurrentViewName { get; }

        UserControl CurrentView { get; }

        public event EventHandler<ApplicationPage> NavigationChanged;

        void NavigateTo(ApplicationPage page);

        void NavigateTo(ApplicationPage page, object? initializationParameter = null);

        void InitializeDefaultView(ApplicationPage view);

        bool CanGoBack {  get; }

        void GoBack();

    }
}
