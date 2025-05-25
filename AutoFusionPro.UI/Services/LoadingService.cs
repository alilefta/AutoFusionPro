using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.UI.ViewModels.Base;

namespace AutoFusionPro.UI.Services
{
    public class LoadingService : ObservableObject, ILoadingService
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        private string _loadingMessage = "Loading...";
        public string LoadingMessage
        {
            get => _loadingMessage;
            private set => SetProperty(ref _loadingMessage, value);
        }

        private LoadingSize _currentSize = LoadingSize.Medium;
        public LoadingSize CurrentSize
        {
            get => _currentSize;
            private set => SetProperty(ref _currentSize, value);
        }

        public void Show(string message = "Loading...", LoadingSize size = LoadingSize.Medium)
        {
            LoadingMessage = message;
            CurrentSize = size;
            IsLoading = true;
        }

        public void Hide()
        {
            IsLoading = false;
            LoadingMessage = "Loading...";
            CurrentSize = LoadingSize.Medium;
        }

        public async Task<T> ExecuteWithLoadingAsync<T>(
            Func<Task<T>> operation,
            string message = "Loading...",
            LoadingSize size = LoadingSize.Medium)
        {
            try
            {
                Show(message, size);
                return await operation();
            }
            finally
            {
                Hide();
            }
        }

        public async Task ExecuteWithLoadingAsync(
            Func<Task> operation,
            string message = "Loading...",
            LoadingSize size = LoadingSize.Medium)
        {
            try
            {
                Show(message, size);
                await operation();
            }
            finally
            {
                Hide();
            }
        }
    }

}
