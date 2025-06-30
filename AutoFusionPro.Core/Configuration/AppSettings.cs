using AutoFusionPro.Core.Enums.SystemEnum;

namespace AutoFusionPro.Core.Configuration
{
    public class AppSettings
    {
        public Languages Language { get; set; } = Languages.Arabic; // Default to English

        public bool IsDarkThemeEnabled { get; set; } = false; // Default to light theme
        public string SystemName { get; set; } = "AutoFusion Pro"; // Default to light theme
        public string? LogoPath { get; set; } // Default to light theme

        // --- NEW CURRENCY SETTING ---
        public Currency SelectedCurrency { get; set; } = Currency.USD; // Default to USD

    }
}
