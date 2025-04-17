namespace AutoFusionPro.Application.Interfaces
{

    public interface IInitializableViewModel
    {
        /// <summary>
        /// Gets a value indicating whether the view model has been successfully initialized.
        /// </summary>
        public bool IsInitialized { get; }

        /// <summary>
        /// Gets a task that completes when initialization is finished.
        /// Can be awaited to ensure the view model is ready for use.
        /// </summary>
        public Task Initialized { get; }

        /// <summary>
        /// Initializes the view model asynchronously with optional parameters.
        /// </summary>
        /// <param name="parameter">Optional initialization parameter (could be ID, complex object, etc.)</param>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        public Task InitializeAsync(object? parameter = null);

        /// <summary>
        /// Resets the view model to its uninitialized state.
        /// </summary>
        public void Reset();
    }


}
