using AutoFusionPro.Core.Enums.SystemEnum;
using LiveChartsCore.SkiaSharpView;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using Wpf.Ui.Appearance;

namespace AutoFusionPro.UI.Services
{
    public class LocalizationService : ILocalizationService
    {

        public event Action? FlowDirectionChanged;
        // public event Action? LanguageOrCurrencyChanged; // Implement if needed

        public string CurrentCurrencySymbol => GetCurrencySymbolInternal(_currentGlobalCurrency, _currentLangString);
        public string CurrentFullCurrencyName => GetFullCurrencyName(_currentGlobalCurrency, _currentLangString); // NEW: Optional: e.g., "US Dollar", "Thousand Iraqi Dinar"

        private string _currentLangString = "en-US";

        public string CurrentLangString
        {
            get => _currentLangString;
            set
            {
                _currentLangString = value;
            }
        }

        private Currency _currentGlobalCurrency; // Store the globally selected currency

        private ResourceDictionary? _currentLanguageDictionary; // Cache current dictionary

        private FlowDirection _currentFlowDirection = FlowDirection.LeftToRight;

        public FlowDirection CurrentFlowDirection
        {
            get => _currentFlowDirection;
            private set
            {
                if (_currentFlowDirection != value)
                {
                    _currentFlowDirection = value;
                    FlowDirectionChanged?.Invoke();
                }
            }
        }


        public LocalizationService() // Load initial settings
        {
            // Load initial settings perhaps via a settings manager passed in or a static call
            var initialSettings = SettingsManager.LoadSettings(); // Assuming SettingsManager is accessible
            ApplyLanguageAndCurrency(initialSettings.Language, initialSettings.SelectedCurrency);
            ApplyTheme(initialSettings.IsDarkThemeEnabled);
        }



        public void ApplyLanguageAndCurrency(Languages language, Currency currency)
        {
            string langCode = language == Languages.English ? "en" : "ar";
            _currentGlobalCurrency = currency; // Store the current currency

            var culture = new CultureInfo(langCode);
            // Consider setting culture for numbers and dates specifically based on language/currency needs
            // For example, for IQD, you might want Arabic number formatting even if UI is English
            // CultureInfo.CurrentCulture = culture; // Affects formatting
            // CultureInfo.CurrentUICulture = culture; // Affects resource lookup


            // More robust way to set default thread cultures
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            CurrentFlowDirection = (langCode == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            CurrentLangString = (langCode == "ar") ? "ar-SA" : "en-US"; // Or just langCode


            var newLanguageDictionary = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/AutoFusionPro.UI;component/Resources/Dictionaries/Resources.{langCode}.xaml", UriKind.Absolute)
            };

            // Remove old language dictionary if it exists
            var existingLangDict = System.Windows.Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Resources/Dictionaries/Resources."));
            if (existingLangDict != null)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(existingLangDict);
            }


            System.Windows.Application.Current.Resources.MergedDictionaries.Add(newLanguageDictionary);
            _currentLanguageDictionary = newLanguageDictionary; // Cache it

            // LanguageOrCurrencyChanged?.Invoke(); // Notify subscribers
            OnPropertyChanged(nameof(CurrentCurrencySymbol)); // If this class implements INotifyPropertyChanged
            OnPropertyChanged(nameof(CurrentFullCurrencyName)); // If this class implements INotifyPropertyChanged
        }

        public void ApplyTheme(bool isDarkThemeEnabled)
        {

            var theme = isDarkThemeEnabled
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;

            // Apply the theme using WPF UI's ApplicationThemeManager
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
              theme, // Theme type
              Wpf.Ui.Controls.WindowBackdropType.Mica,  // Background type
              true                                      // Whether to change accents automatically
            );


            if (isDarkThemeEnabled)
            {
                LiveChartsCore.LiveCharts.Configure(config => config.AddDarkTheme());
            }
            else
            {
                LiveChartsCore.LiveCharts.Configure(config => config.AddLightTheme());
            }


            var themeUri = isDarkThemeEnabled
                ? "pack://application:,,,/AutoFusionPro.UI;component/Resources/Themes/DarkTheme.xaml"
                : "pack://application:,,,/AutoFusionPro.UI;component/Resources/Themes/LightTheme.xaml";

            var themeDictionary = new ResourceDictionary { Source = new Uri(themeUri, UriKind.Absolute) };

            var existingThemeDict = System.Windows.Application.Current.Resources.MergedDictionaries
                        .FirstOrDefault(d => d.Source != null && (d.Source.OriginalString.EndsWith("DarkTheme.xaml") || d.Source.OriginalString.EndsWith("LightTheme.xaml")));
            if (existingThemeDict != null)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(existingThemeDict);
            }

            // TODO Could be problematic
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(themeDictionary);
        }

        public string GetString(string resourceKey)
        {
            if (_currentLanguageDictionary != null && _currentLanguageDictionary.Contains(resourceKey))
            {
                return _currentLanguageDictionary[resourceKey]?.ToString() ?? $"[{resourceKey}_NotFound]";
            }
            // Fallback or global resource lookup if needed
            // var fallback = System.Windows.Application.Current.TryFindResource(resourceKey);
            // return fallback?.ToString() ?? $"[{resourceKey}_NotFoundGlobally]";
            return $"[{resourceKey}_DictNullOrKeyNotFound]";
        }

        #region Helpers

        private string GetCurrencySymbolInternal(Currency currency, string langCode)
        {
            // This is a simplified version. For true localization, you'd use CultureInfo
            // or dedicated resource strings for currency symbols per language.
            if (langCode.StartsWith("ar"))
            {
                return currency switch
                {
                    Currency.USD => "$", // Or "دولار أمريكي" if you want the full name
                    Currency.IQD => "د.ع", // Arabic symbol for Iraqi Dinar
                    _ => currency.ToString(),
                };
            }
            else // Default to English-like symbols
            {
                return currency switch
                {
                    Currency.USD => "$",
                    Currency.IQD => "IQD", // Or "Iraqi Dinar"
                    _ => currency.ToString(),
                };
            }
        }

        private string GetFullCurrencyName(Currency currency, string langCode)
        {
            // This is a simplified version. For true localization, you'd use CultureInfo
            // or dedicated resource strings for currency symbols per language.
            if (langCode.StartsWith("ar"))
            {
                return currency switch
                {
                    Currency.USD => "دولار امريكي", // "دولار أمريكي"
                    Currency.IQD => "الف دينار عراقي", // Thousand Iraqi Dinar
                    _ => currency.ToString(),
                };
            }
            else // Default to English-like symbols
            {
                return currency switch
                {
                    Currency.USD => "US Dollar",
                    Currency.IQD => "Thousand Iraqi Dinar", // Or "Iraqi Dinar"
                    _ => currency.ToString(),
                };
            }
        }
        


        // If LocalizationService itself needs to notify changes (e.g., for CurrentCurrencySymbol)
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion
    }
}
