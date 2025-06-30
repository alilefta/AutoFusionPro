using AutoFusionPro.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace AutoFusionPro.UI.ViewModels.Base
{
    public partial class BaseViewModel<VM>: ObservableValidator, IDisposable where VM : class
    {
        private readonly List<Action> _cleanupActions = new();

        /// <summary>
        /// Child View models register this even if they use Dictionaries for language strings
        /// </summary>
        public event EventHandler? LanguageDictionariesChanged;


        #region Private Fields

        protected readonly ILocalizationService _localizationService;
        protected readonly ILogger<VM> _logger;

        #endregion

        #region props

        [ObservableProperty]
        private FlowDirection _currentWorkFlow;

        [ObservableProperty]
        private string _currentCurrency;
        [ObservableProperty]
        private string _currentCurrencyFullName;

        #endregion

        public BaseViewModel(ILocalizationService localizationService, ILogger<VM> logger)
        {
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
            CurrentCurrency = _localizationService.CurrentCurrencySymbol; 
            CurrentCurrencyFullName = _localizationService.CurrentFullCurrencyName;

            _localizationService.FlowDirectionChanged += OnCurrentFlowDirectionChanged;

            RegisterCleanup(() => _localizationService.FlowDirectionChanged -= OnCurrentFlowDirectionChanged);
        }




        #region Event Listeners

        private void OnCurrentFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
            CurrentCurrency = _localizationService.CurrentCurrencySymbol;
            CurrentCurrencyFullName = _localizationService.CurrentFullCurrencyName;

            LanguageDictionariesChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        #region subscription & Disposition

        /// <summary>
        /// Registers an event unsubscripted action.
        /// </summary>
        protected void RegisterCleanup(Action cleanupAction)
        {
            _cleanupActions.Add(cleanupAction);
        }


        public void Dispose()
        {
            foreach (var action in _cleanupActions)
            {
                action.Invoke();
            }
            _cleanupActions.Clear();
        }

        #endregion

        #region Public Helpers

        public string GetString(string resourceKey)
        {
            // Logic to find the current language ResourceDictionary and get the string
            // This might involve looking up the active dictionary in Application.Current.Resources.MergedDictionaries
            // or caching it.
            var langCode = _localizationService.CurrentLangString.StartsWith("ar") ? "ar" : "en";
            var dictSource = new Uri($"pack://application:,,,/AutoFusionPro.UI;component/Resources/Dictionaries/Resources.{langCode}.xaml", UriKind.Absolute);

            var langDict = System.Windows.Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source == dictSource);

            if (langDict != null && langDict.Contains(resourceKey))
            {
                return langDict[resourceKey]?.ToString() ?? $"[{resourceKey}_NotFound]";
            }
            return $"[{resourceKey}_DictOrKeyNotFound]";
        }

        #endregion

    }
}
