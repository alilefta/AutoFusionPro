using AutoFusionPro.Application.Commands;
using AutoFusionPro.Core.Configuration;
using AutoFusionPro.Core.Enums.SystemEnum;
using AutoFusionPro.Core.Services;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AutoFusionPro.UI.ViewModels.Settings
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private ILogger<SettingsViewModel> _logger;
        private readonly IGlobalSettingsService<BitmapImage> _globalSettingsService;
        private ILocalizationService<FlowDirection> _localizationService;

        private AppSettings _settings;

        [ObservableProperty]
        private Languages _language = Languages.Arabic;

        [ObservableProperty]
        private bool _isDarkThemeEnabled;


        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private BitmapImage _systemLogo;

        [ObservableProperty]
        private string _systemName;

        public List<Languages> LanguageList { get; } = Enum.GetValues(typeof(Languages)).Cast<Languages>().ToList();

        #region Commands

        public ICommand SaveSettingsCommand { get; }
        public ICommand RestoreDefaultsCommand { get; }

        #endregion

        public SettingsViewModel(ILocalizationService<FlowDirection> localizationService, ILogger<SettingsViewModel> logger, IGlobalSettingsService<BitmapImage> globalSettingsService)
        {
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _globalSettingsService = globalSettingsService ?? throw new ArgumentNullException(nameof(globalSettingsService));

            CurrentWorkFlow = localizationService.CurrentFlowDirection;
            localizationService.FlowDirectionChanged += OnFlowDirectionChanged;

            _settings = SettingsManager.LoadSettings();

            Language = _settings.Language;
            IsDarkThemeEnabled = _settings.IsDarkThemeEnabled;
            SystemName = _settings.SystemName;

            SaveSettingsCommand = new RelayCommand(async _ => await SaveSettingsAsync(), canExecute: o => true);
            RestoreDefaultsCommand = new RelayCommand(async _ => await RestoreDefaultsAsync(), canExecute: o => true);

            LoadLogo();
        }

        private async void LoadLogo()
        {
            try
            {
                if (!string.IsNullOrEmpty(_settings.LogoPath) && File.Exists(_settings.LogoPath))
                {
                    var bitmap = new BitmapImage(new Uri(_settings.LogoPath, UriKind.Absolute));
                    bitmap.Freeze();
                    SystemLogo = bitmap;
                    _logger.LogInformation($"Logo loaded from settings: {_settings.LogoPath}");
                }
                else
                {
                    SystemLogo = new BitmapImage(new Uri("pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/OscarLogo.png"));
                    _logger.LogInformation("Placeholder logo loaded");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading logo");
                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Error loading logo: {ex.Message}", true, CurrentWorkFlow);
            }
        }

        private async Task RestoreDefaultsAsync()
        {
            IsBusy = true;
            try
            {
                SettingsManager.RestoreDefaultSettings();
                _settings = SettingsManager.LoadSettings();
                IsDarkThemeEnabled = false;
                Language = _settings.Language;
                SystemName = _settings.SystemName ?? "Oscar Lab";
                SystemLogo = new BitmapImage(new Uri("pack://application:,,,/AutoFusionPro.UI;component/Assets/Images/OscarLogo.png"));

                _localizationService.ApplyLanguage(Language);
                _localizationService.ApplyTheme(IsDarkThemeEnabled);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync("Default settings have been restored successfully", false, CurrentWorkFlow);
                _logger.LogInformation("Default settings restored");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring default settings");
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Restore Failed", $"Error restoring default settings: {ex.Message}", true, CurrentWorkFlow);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveSettingsAsync()
        {
            IsBusy = true;
            try
            {
                _settings.Language = Language;
                _settings.IsDarkThemeEnabled = IsDarkThemeEnabled;

                // User cannot change Lab name and logo, if i decided against that, uncomment!

                //_settings.LabName = LabName;

                // string newLogoPath = null;
                //if (LabLogo != null && LabLogo.UriSource != null)
                //{
                //    newLogoPath = await SaveLogoAsync(LabLogo.UriSource.LocalPath);
                //    _settings.LogoPath = newLogoPath;
                //}

                _localizationService.ApplyLanguage(Language);
                _localizationService.ApplyTheme(IsDarkThemeEnabled);

                SettingsManager.SaveSettings(_settings);
                _globalSettingsService.UpdateSettings(_settings);



                //if (!string.IsNullOrEmpty(newLogoPath))
                //{
                //    await Application.Current.Dispatcher.InvokeAsync(async () =>
                //    {
                //        await LoadLogoFromPathAsync(newLogoPath);
                //        using (var memoryStream = new MemoryStream(File.ReadAllBytes(newLogoPath)))
                //        {
                //            Application.Current.MainWindow.Icon = BitmapFrame.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                //        }
                //    });
                //}

                await MessageBoxHelper.ShowMessageWithoutTitleAsync("Settings saved successfully", false, CurrentWorkFlow);
                System.Windows.Application.Current.MainWindow.Title = $"{SystemName}";
                _logger.LogInformation("Settings saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings");
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Save Failed", $"Error saving settings: {ex.Message}", true, CurrentWorkFlow);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }
    }
}
