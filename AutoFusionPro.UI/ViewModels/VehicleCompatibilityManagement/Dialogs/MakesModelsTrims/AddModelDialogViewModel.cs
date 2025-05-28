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
    public partial class AddModelDialogViewModel : InitializableViewModel<AddMakeDialogViewModel>, IDialogAware
    {
        #region Fields
        /// <summary>
        /// Value Provided by DI container to manage Models.
        /// </summary>
        private readonly ICompatibleVehicleService _compatibleVehicleService;
        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;
        #endregion

        #region General & Model Props

        [ObservableProperty]
        private MakeDto _make = null!;

        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddModelCommand))]
        private string _name = string.Empty;

        #endregion

        #region Image Handling Props

        [ObservableProperty]
        private string? _imagePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedImage))]
        private string? _selectedImagePreview;

        public bool HasSelectedImage => !string.IsNullOrEmpty(SelectedImagePreview);

        #endregion

        #region Constructor

        public AddModelDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<AddMakeDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));

        }

        #endregion

        #region Initializer

        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/> To Initialize <see cref="AddModelDialogViewModel"/>
        /// </summary>
        /// <param name="parameter"><see cref="MakeDto"/></param>
        /// <returns><see cref="Void"/></returns>
        public override async Task InitializeAsync(object? parameter)
        {

            if (IsInitialized)
            {
                return;
            }

            if (parameter == null) return;

            var make = (MakeDto)parameter;
            Make = make;

            _logger.LogInformation("AddModelDialogViewModel initialized with parameter {param}", Make.Id);

            await base.InitializeAsync(parameter);
        }

        #endregion

        #region Commands

        private bool CanAddModel()
        {
            return !string.IsNullOrEmpty(Name);
        }


        [RelayCommand(CanExecute = nameof(CanAddModel))]
        private async Task AddModelAsync()
        {
            try
            {
                IsAdding = true;

                if(Make == null || Make.Id == 0)
                {
                    _logger.LogError("Failed while adding a new model, the make was null");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync("Make ID was not found", true, CurrentWorkFlow);
                    SetResultAndClose(false);
                    return;
                }

                var createModelDto = new CreateModelDto(Name, Make.Id, ImagePath);

                var newItem = await _compatibleVehicleService.CreateModelAsync(createModelDto);

                _logger.LogInformation("New model with Name = {ModelName} and ID = {ID} has been added successfully", newItem.Name, newItem.Id);


                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Model {newItem.Name} has been created successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }
            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while creating Model, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The model {Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed while adding a new model, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddMakeDialogViewModel), nameof(AddModelAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);

            }
            finally
            {
                IsAdding = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
        }


        #endregion

        #region Dialog Specific Methods

        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(AddModelDialog));
                return;
            }

            _dialog = dialog;
        }

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
    }
}
