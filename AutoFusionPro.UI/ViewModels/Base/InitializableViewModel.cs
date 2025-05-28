using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.UI.Services;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Base
{
    public abstract class InitializableViewModel<TViewModel> : BaseViewModel<TViewModel>, IInitializableViewModel where TViewModel : class
    {
        private TaskCompletionSource<bool> _initializationComplete;
        private bool _isInitialized;

        protected InitializableViewModel(ILocalizationService localizationService, ILogger<TViewModel> logger) : base(localizationService, logger)
        {
            _initializationComplete = new TaskCompletionSource<bool>();
            IsInitialized = false;
        }

        /// <inheritdoc />
        public bool IsInitialized
        {
            get => _isInitialized;
            protected set
            {
                _isInitialized = value;
                OnPropertyChanged(nameof(IsInitialized));
            }
        }

        /// <inheritdoc />
        public Task Initialized => _initializationComplete.Task;

        /// <inheritdoc />
        public virtual async Task InitializeAsync(object? parameter = null)
        {
            if (IsInitialized)
            {
                return;
            }

            try
            {

                // Derived classes should override this method and call base.InitializeAsync
                // after their initialization logic is complete
                IsInitialized = true;
                _initializationComplete.TrySetResult(true);
            }
            catch (Exception ex)
            {
                _initializationComplete.TrySetException(ex);
                throw;
            }
        }

        /// <inheritdoc />
        public virtual void Reset()
        {
            IsInitialized = false;
            _initializationComplete = new TaskCompletionSource<bool>();
        }

        /// <summary>
        /// Helper method to execute code on the UI thread.
        /// </summary>
        protected async Task RunOnUiThread(Func<Task> action)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                await action();
            }
            else
            {
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () => await action());
            }
        }
    }
}
