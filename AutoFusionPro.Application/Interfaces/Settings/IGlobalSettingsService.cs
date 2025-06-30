using AutoFusionPro.Core.Configuration;
using AutoFusionPro.Core.Enums.SystemEnum;
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

        Currency CurrentCurrency { get; }
        string CurrentCurrencySymbol { get; } // e.g., "$", "IQD" or "د.ع"

        // string CurrentCurrencyCode { get; } // e.g., "USD", "IQD" (optional if symbol is enough for UI)
        // event EventHandler CurrencyChanged; // Optional, if UI needs to react dynamically beyond initial load
    
    }
}
