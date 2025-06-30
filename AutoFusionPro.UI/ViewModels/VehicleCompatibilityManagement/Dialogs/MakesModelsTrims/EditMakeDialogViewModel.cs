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
    /// Used for updating the <see cref="Name"/> for <see cref="MakeDto"/> 
    /// Requires <see cref="InitializeAsync(object?)"/> to be called from the <see cref="IDialogService"/> or by other Caller, to send the <see cref="MakeDto"/>  to be updated.
    /// Implemented <see cref="InitializableViewModel{TViewModel}"/> for the <see cref="InitializeAsync(object?)"/> Initialization process and <see cref="IDialogAware"/> to be able to set the associated Dialog Window via <see cref="IDialogWindow"/>. 
    /// To be able to set results and close <seealso cref="SetResultAndClose(bool)"/>
    /// </summary>
    public partial class EditMakeDialogViewModel : InitializableViewModel<EditMakeDialogViewModel>, IDialogAware
    {
        #region Fields
        /// <summary>
        /// Value Provided by DI container to manage Models.
        /// </summary>
        private readonly IVehicleTaxonomyService _compatibleVehicleService;
        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;
        #endregion

        #region Props

        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditMakeCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private string? _imagePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedImage))]
        private string? _selectedImagePreview;
        public bool HasSelectedImage => !string.IsNullOrEmpty(SelectedImagePreview);

        [ObservableProperty]
        private MakeDto? _makeToEdit;

        #endregion

        #region Constructor
        public EditMakeDialogViewModel(
            IVehicleTaxonomyService compatibleVehicleService, 
            ILocalizationService localizationService, 
            ILogger<EditMakeDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));


        }
        #endregion

        #region Initializer

        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/> to initialize <see cref="EditMakeDialogViewModel"/>
        /// </summary>
        /// <param name="parameter"><see cref="MakeDto"/></param>
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

            MakeToEdit = (MakeDto)parameter;

            Name = MakeToEdit.Name;
            ImagePath = MakeToEdit.ImagePath;
            SelectedImagePreview = ImagePath;
            RemoveImageCommand.NotifyCanExecuteChanged();

            await base.InitializeAsync(parameter);
        }

        #endregion

        #region Commands
        private bool CanEditMake()
        {
            return !string.IsNullOrEmpty(Name);
        }


        /// <summary>
        /// Edit <see cref="UpdateMakeDto"/>
        /// </summary>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ViewModelException"></exception>
        [RelayCommand(CanExecute = nameof(CanEditMake))]
        private async Task EditMakeAsync()
        {
            try
            {
                if (MakeToEdit == null || MakeToEdit.Id == 0)
                {
                    _logger.LogError("The make to be updated is Null!");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync($"An error occurred, please, try again!", true, CurrentWorkFlow);

                    SetResultAndClose(false);

                    return;
                }


                IsEditing = true;

                var updateMakeDTO = new UpdateMakeDto(MakeToEdit.Id, Name, ImagePath);

                 await _compatibleVehicleService.UpdateMakeAsync(updateMakeDTO);

                _logger.LogInformation("Make with name = {MakeName} and ID = {ID} has been updated successfully", updateMakeDTO.Name, updateMakeDTO.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Make {updateMakeDTO.Name} has been updated successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while updating Make, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The make {Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while updating Make, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.UPDATE_DATA_EXCEPTION_MESSAGE, nameof(EditMakeDialogViewModel), nameof(EditMakeAsync), Core.Helpers.Operations.MethodOperationType.UPDATE_DATA, ex);

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
        #endregion

        #region Image Handling Commands

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

        #region Dialog Specific Methods

        /// <summary>
        /// Provided by <see cref="IDialogAware"/> to set the window for Setting Dialog Results and Closing purposes <see cref="SetResultAndClose(bool)"/>
        /// </summary>
        /// <param name="dialog">Dialog will be provided internally by the service.</param>
        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(EditMakeDialog));
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

        #endregion

    }
}
