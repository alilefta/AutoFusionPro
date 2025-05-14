using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.General.Dialogs
{
    public partial class ConfirmDeleteItemsDialogViewModel : BaseViewModel<ConfirmDeleteItemsDialogViewModel>, IDialogAware
    {
        private IDialogWindow _dialog = null!;

        [ObservableProperty]
        private int _itemsCount = 0;

        public ConfirmDeleteItemsDialogViewModel(
            ILocalizationService localizationService, 
            ILogger<ConfirmDeleteItemsDialogViewModel> logger) : base(localizationService, logger)
        {
            
        }

        [RelayCommand]
        private void ConfirmDelete()
        {
            SetResultAndClose(true);
        }


        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
        }

        /// <summary>
        /// Provided by <see cref="IDialogAware"/> to set the window for Setting Dialog Results and Closing purposes <see cref="SetResultAndClose(bool)"/>
        /// </summary>
        /// <param name="dialog">Dialog will be provided internally by the service.</param>
        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the EditModelDialogViewModel");
                return;
            }

            _dialog = dialog;
        }

        /// <summary>
        /// Helper for Setting results and Close
        /// </summary>
        /// <param name="res"></param>
        private void SetResultAndClose(bool res)
        {
            if (_dialog != null)
            {
                // Set the result first
                _dialog.DialogResult = res;

                // Then close with animation
                _dialog.Close();
            }

        }
    }
}
