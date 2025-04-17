using AutoFusionPro.Core.Configuration;
using AutoFusionPro.Core.Services;
using Microsoft.Extensions.Logging;
using System.Windows.Media.Imaging;

namespace AutoFusionPro.UI.Services
{

    public class GlobalSettingsService : IGlobalSettingsService<BitmapImage>
    {
        private readonly ILogger<GlobalSettingsService> _logger;
        private AppSettings _currentSettings;

        // TODO Check if component is correct
        public string LogoPath => _currentSettings.LogoPath ?? "pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/OscarLogo.png";
        public string IconPath => _currentSettings.LogoPath ?? "pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/croppedRound.png";
        public string SystemName => _currentSettings.SystemName;

        public event EventHandler SettingsChanged;

        public GlobalSettingsService(ILogger<GlobalSettingsService> logger)
        {
            _logger = logger;
            _currentSettings = SettingsManager.LoadSettings();
        }

        public void UpdateSettings(AppSettings newSettings)
        {
            _currentSettings = newSettings;
            SettingsManager.SaveSettings(_currentSettings);
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public BitmapImage GetLogoImage()
        {
            try
            {
                _logger.LogInformation($"Trying to load a logo image from the LogoPath: ${LogoPath}");

                if (string.IsNullOrEmpty(LogoPath))
                {
                    var defaultImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/OscarLogo.png";

                    return new BitmapImage(new Uri(defaultImagePath));
                }


                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(LogoPath, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading logo from the path: {LogoPath}");

                var defaultImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/OscarLogo.png";

                return new BitmapImage(new Uri(defaultImagePath));

            }

        }

        public BitmapImage GetIconImage()
        {
            try
            {
                _logger.LogInformation($"Trying to load a logo icon from the LogoPath: ${IconPath}");

                if (string.IsNullOrEmpty(IconPath))
                {
                    var defaultImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/croppedRound.png";

                    return new BitmapImage(new Uri(defaultImagePath));
                }


                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(LogoPath, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading icon from the path: {LogoPath}");

                var defaultImagePath = "pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/croppedRound.png";

                return new BitmapImage(new Uri(defaultImagePath));

            }

        }
    }
}
