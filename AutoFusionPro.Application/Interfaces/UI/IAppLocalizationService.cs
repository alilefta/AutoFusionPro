using AutoFusionPro.Core.Enums.SystemEnum;

namespace AutoFusionPro.Application.Interfaces.UI
{
    public interface IAppLocalizationService<FD> where FD : Enum
    {
        FD CurrentFlowDirection { get; }
        string CurrentLangString { get; }
        string CurrentCurrencySymbol { get; }
        string CurrentFullCurrencyName { get; } // NEW: Optional: e.g., "US Dollar", "Thousand Iraqi Dinar"
        //string CurrentFullCurrencyTipName { get; } // NEW: Optional: e.g., "US Dollar", "Thousand Iraqi Dinar"


        event Action FlowDirectionChanged;
        // event Action LanguageOrCurrencyChanged; // NEW: More general event

        void ApplyLanguageAndCurrency(Languages language, Currency currency);
        void ApplyTheme(bool isDarkThemeEnabled);
        string GetString(string resourceKey); // NEW: Method for direct translation
    }
}
