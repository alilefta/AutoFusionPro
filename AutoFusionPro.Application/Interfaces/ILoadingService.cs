namespace AutoFusionPro.Application.Interfaces
{
    public interface ILoadingService
    {
        bool IsLoading { get; }
        string LoadingMessage { get; }
        LoadingSize CurrentSize { get; }

        void Show(string message = "Loading...", LoadingSize size = LoadingSize.Medium);
        void Hide();

        /// <summary>
        /// Executes an async operation with loading indicator
        /// </summary>
        /// <typeparam name="T">Return type of the operation</typeparam>
        /// <param name="operation">The async operation to execute</param>
        /// <param name="message">Optional loading message</param>
        /// <param name="size">Optional loading size</param>
        /// <returns>Result of the operation</returns>
        Task<T> ExecuteWithLoadingAsync<T>(Func<Task<T>> operation,
            string message = "Loading...",
            LoadingSize size = LoadingSize.Medium);

        /// <summary>
        /// Executes an async operation with loading indicator
        /// </summary>
        /// <param name="operation">The async operation to execute</param>
        /// <param name="message">Optional loading message</param>
        /// <param name="size">Optional loading size</param>
        Task ExecuteWithLoadingAsync(Func<Task> operation,
            string message = "Loading...",
            LoadingSize size = LoadingSize.Medium);
    }

    public enum LoadingSize
    {
        Small,
        Medium,
        Large
    }
}
