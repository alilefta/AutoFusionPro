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

        public BaseViewModel(ILocalizationService localizationService, ILogger<VM> logger)
        {
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
            _localizationService.FlowDirectionChanged += OnCurrentFlowDirectionChanged;

            RegisterCleanup(() => _localizationService.FlowDirectionChanged -= OnCurrentFlowDirectionChanged);
        }

        public BaseViewModel()
        {
            
        }

        #region Flow Direction

        [ObservableProperty]
        private FlowDirection _currentWorkFlow;

        #endregion

        #region Helpers

        private void OnCurrentFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;

            LanguageDictionariesChanged?.Invoke(this, EventArgs.Empty);
        }


        #endregion

        /// <summary>
        /// Registers an event unsubscription action.
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

    }
}
