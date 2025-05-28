using System.Windows;

namespace AutoFusionPro.Application.Interfaces.Dialogs
{
    public interface IDialogService
    {
       
        /// <summary>
        /// General Delete Confirm Dialog
        /// </summary>
        /// <param name="count">Number of items to be deleted, just for UI show</param>
        /// <returns></returns>
        bool? ShowConfirmDeleteItemsDialog(int count);

        /// <summary>
        /// General Method to show a dialog and initialize it with ViewModel.
        /// </summary>
        /// <typeparam name="TDialogViewModel">The Dialog View Model. The DialogViewModel MUST inherit from <see cref="IDialogAware"/></typeparam>
        /// <typeparam name="TView">The Dialog Window</typeparam>
        /// <param name="param">Optional Object Parameter for initializing the view model that inherits from <see cref="InitializbleViewModel"/></param>
        /// <returns><see cref="bool?"/></returns>
        /// <example>
        /// await _dialogService.ShowDialogAsync<AddTrimLevelDialogViewModel, AddTrimLevelDialog>(SelectedModel); 
        /// // Results will be either true or false or null.
        /// </example>
        Task<bool?> ShowDialogAsync<TDialogViewModel, TView>(object? param = null) where TDialogViewModel : class where TView : Window, new();

        /// <summary>
        /// General Method to show a dialog, initialize it with ViewModel and Return general <see cref="Object"/> param.
        /// </summary>
        /// <typeparam name="TDialogViewModel">The Dialog View Model. The DialogViewModel MUST inherit from <see cref="IDialogViewModelWithResult{TResult}"/></typeparam>
        /// <typeparam name="TView">The Dialog Window</typeparam>
        /// <typeparam name="TResult">The Type of the Return Type.</typeparam>
        /// <param name="param">Optional Object Parameter for initializing the view model that inherits from <see cref="InitializbleViewModel"/></param>
        /// <returns><see cref="object?"/>Param</returns>
        /// <example>
        /// CompatibleVehicleFilterCriteriaDto filters = await _dialogService.ShowDialogWithResultsAsync<CompatibleVehicleFilterOptionsDialogViewModel, CompatibleVehicleFilterOptionsDialog>(null); 
        /// // Results will be <see cref="Nullable"/> <see cref="Object"/>.
        /// </example>
        Task<TResult?> ShowDialogWithResultsAsync<TDialogViewModel, TView, TResult>(object? param = null) where TDialogViewModel : class, IDialogViewModelWithResult<TResult> where TView : Window, new();

    }

}
