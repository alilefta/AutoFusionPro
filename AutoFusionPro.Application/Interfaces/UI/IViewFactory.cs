using AutoFusionPro.Core.Enums.NavigationPages;
using System.Windows.Controls;

namespace AutoFusionPro.Application.Interfaces.UI
{
    // Create a factory interface for creating views
    public interface IViewFactory
    {
        UserControl CreateView(ApplicationPage page);
    }
}
