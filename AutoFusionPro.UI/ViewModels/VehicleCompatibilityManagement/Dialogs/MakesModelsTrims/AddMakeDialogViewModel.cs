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
    /// Used to make a new <see cref="MakeDto"/>
    /// </summary>
    public partial class AddMakeDialogViewModel : BaseViewModel<AddMakeDialogViewModel>, IDialogAware
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
        private bool _isAdding = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddMakeCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private string? _imagePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedImage))]
        private string? _selectedImagePreview;

        public bool HasSelectedImage => !string.IsNullOrEmpty(SelectedImagePreview);

        #endregion

        #region Constructor

        public AddMakeDialogViewModel(
            IVehicleTaxonomyService compatibleVehicleService, 
            ILocalizationService localizationService, 
            ILogger<AddMakeDialogViewModel> logger) : base(localizationService,  logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
        }

        #endregion

        #region Commands
        private bool CanAddMake()
        {
            return !string.IsNullOrEmpty(Name);
        }

        [RelayCommand(CanExecute = nameof(CanAddMake))]
        private async Task AddMakeAsync()
        {
            try
            {
                IsAdding = true;

                // Saving image is handled by back-end service CompatibleVehicleService
                var createMakeDTO = new CreateMakeDto(Name, ImagePath);
                var newItem = await _compatibleVehicleService.CreateMakeAsync(createMakeDTO);

                _logger.LogInformation("New make with name = {MakeName} and ID = {ID} has been added successfully", newItem.Name, newItem.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Make {newItem.Name} has been created successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while creating Make, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The make {Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex) {

                _logger.LogError("Failed while adding a new Make, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddMakeDialogViewModel), nameof(AddMakeAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);

            }
            finally
            {
                IsAdding  = false;
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
        /// Provided By <see cref="IDialogAware"/> with value <see cref="AddMakeDialog"/>.
        /// </summary>
        /// <param name="dialog"><see cref="AddMakeDialog"/></param>
        public void SetDialogWindow(IDialogWindow dialog)
        {
            if(dialog == null)
            {
                _logger.LogError("The Dialog window was null on {VM}", nameof(AddMakeDialog));
                return;
            }

            _dialog = dialog;
        }

        /// <summary>
        /// Helper Method to set DialogResults to either True/False to helper Caller ViewModel to Update UI based on results.
        /// </summary>
        /// <param name="res"><see cref="bool?"/></param>
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
