using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.MakesModelsTrims;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.MakesModelsTrims
{
    /// <summary>
    /// Used for updating the <see cref="Name"/> for <see cref="ModelDto"/> 
    /// Requires <see cref="InitializeAsync(object?)"/> to be called from the <see cref="IDialogService"/> or by other Caller, to send the Model to be updated.
    /// Implemented <see cref="InitializableViewModel{TViewModel}"/> for the <see cref="InitializeAsync(object?)"/> Initialization process and <see cref="IDialogAware"/> to be able to set the associated Dialog Window via <see cref="IDialogWindow"/>. 
    /// To be able to set results and close <seealso cref="SetResultAndClose(bool)"/>
    /// </summary>
    public partial class EditModelDialogViewModel : InitializableViewModel<EditModelDialogViewModel>, IDialogAware
    {
        private IDialogWindow _dialog = null!;
        private readonly ICompatibleVehicleService _compatibleVehicleService;


        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditModelCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private ModelDto? _modelToEdit;


        #region Image Handling

        [ObservableProperty]
        private string? _imagePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedImage))]
        private string? _selectedImagePreview;

        public bool HasSelectedImage => !string.IsNullOrEmpty(SelectedImagePreview);

        #endregion


        public EditModelDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<EditModelDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
        }

        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/>.
        /// </summary>
        /// <param name="parameter">Model of Type <see cref="ModelDto"/></param>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (parameter == null)
            {
                _logger.LogError("The parameter is null!");
                throw new ArgumentNullException(nameof(parameter));
            }

            if (IsInitialized)
            {
                return;
            }

            ModelToEdit = (ModelDto)parameter;

            Name = ModelToEdit.Name;
            ImagePath = ModelToEdit.ImagePath; 
            SelectedImagePreview = ImagePath;
            RemoveImageCommand.NotifyCanExecuteChanged();


            await base.InitializeAsync(parameter);
        }


        private bool CanEditModel()
        {
            return !string.IsNullOrEmpty(Name);
        }


        /// <summary>
        /// Edit <see cref="UpdateModelDto"/>
        /// </summary>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ViewModelException"></exception>
        [RelayCommand(CanExecute = nameof(CanEditModel))]
        private async Task EditModelAsync()
        {
            try
            {
                if (ModelToEdit == null)
                {
                    _logger.LogError("The model to be updated is Null!");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync($"An error occurred, please, try again!", true, CurrentWorkFlow);

                    SetResultAndClose(false);

                    return;
                }

                IsEditing = true;

                var updateModelDTO = new UpdateModelDto(ModelToEdit.Id, Name, ModelToEdit.MakeId, ImagePath);

                await _compatibleVehicleService.UpdateModelAsync(updateModelDTO);

                _logger.LogInformation("Model with with name={Name} and ID={ID} has been updated successfully", updateModelDTO.Name, updateModelDTO.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Model {updateModelDTO.Name} has been updated successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while updating Model, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The model with name={Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while updating Model, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.UPDATE_DATA_EXCEPTION_MESSAGE, nameof(EditModelDialogViewModel), nameof(EditModelAsync), Core.Helpers.Operations.MethodOperationType.UPDATE_DATA, ex);

            }
            finally
            {
                IsEditing = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
        }


        #region Image Commands

        [RelayCommand]
        private async Task LoadImage(object parameter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Title = "Select Product Image"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    SelectedImagePreview = dialog.FileName;
                    ImagePath = dialog.FileName; // Store original path temporarily

                }
                catch (Exception ex)
                {
                    await MessageBoxHelper.ShowMessageWithTitleAsync(
                        "Error",
                        "Image Loading Error",
                        $"Failed to load image: {ex.Message}",
                        true,
                        CurrentWorkFlow
                    );
                }
            }

            RemoveImageCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(HasSelectedImage))]
        private void RemoveImage(object parameter)
        {
            SelectedImagePreview = null;
            ImagePath = null;
        }

        #endregion

        /// <summary>
        /// Provided by <see cref="IDialogAware"/> to set the window for Setting Dialog Results and Closing purposes <see cref="SetResultAndClose(bool)"/>
        /// </summary>
        /// <param name="dialog">Dialog will be provided internally by the service.</param>
        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(EditModelDialog));
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
