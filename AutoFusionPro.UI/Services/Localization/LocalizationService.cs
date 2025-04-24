using AutoFusionPro.Core.Enums.SystemEnum;
using LiveChartsCore.SkiaSharpView;
using System.Globalization;
using System.Windows;
using Wpf.Ui.Appearance;

namespace AutoFusionPro.UI.Services
{
    public class LocalizationService : ILocalizationService
    {
        public void ApplyLanguage(Languages language)
        {
            string lang = string.Empty;

            if (language == Languages.English)
            {
                lang = "en";
            }else
            {
                lang = "ar";
            }

            var culture = new CultureInfo(lang);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            CurrentFlowDirection = (lang == "ar") ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            CurrentLangString = (lang == "ar") ? "ar-SA" : "en-US";


            var languageDictionary = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/AutoFusionPro.UI;component/Resources/Dictionaries/Resources.{lang}.xaml")
            };

            var existingLanguageDictionary = System.Windows.Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Dictionaries"));

            if (existingLanguageDictionary != null)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(existingLanguageDictionary);
            }


            // TODO Could be problematic
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(languageDictionary);
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

            var themeDictionary = new ResourceDictionary
            {
                Source = new Uri(themeUri)
            };

            var existingThemeDictionary = System.Windows.Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Theme"));

            if (existingThemeDictionary != null)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(existingThemeDictionary);
            }


            // TODO Could be problematic
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(themeDictionary);
        }

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

        public event Action FlowDirectionChanged;

        private string _currentLangString = "en-US";

        public string CurrentLangString
        {
            get => _currentLangString;
            set {
                _currentLangString = value; 
            }
        }


    }
}
