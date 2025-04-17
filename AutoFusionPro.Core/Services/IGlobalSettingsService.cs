
using AutoFusionPro.Core.Configuration;

namespace AutoFusionPro.Core.Services
{
    public interface IGlobalSettingsService<BitMapImageClassImp> where BitMapImageClassImp : class
    {
        string LogoPath { get; }
        string IconPath { get; }
        string SystemName { get; }
        event EventHandler SettingsChanged;
        void UpdateSettings(AppSettings newSettings);
        BitMapImageClassImp GetLogoImage();
        BitMapImageClassImp GetIconImage();
    }

}
