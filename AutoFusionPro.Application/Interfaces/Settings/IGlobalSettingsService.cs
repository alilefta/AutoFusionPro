using AutoFusionPro.Core.Configuration;
using System.Windows.Media.Imaging;

namespace AutoFusionPro.Application.Interfaces.Settings
{
    public interface IGlobalSettingsService
    {
        string LogoPath { get; }
        string IconPath { get; }
        string SystemName { get; }
        event EventHandler SettingsChanged;
        void UpdateSettings(AppSettings newSettings);
        BitmapImage GetLogoImage();
        BitmapImage GetIconImage();
    }

}
