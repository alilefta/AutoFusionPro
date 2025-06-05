using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.Categories.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;

namespace AutoFusionPro.UI.ViewModels.Categories.Dialogs
{
    public partial class EditSubCategoryDialogViewModel : InitializableViewModel<EditSubCategoryDialogViewModel>, IDialogAware
    {
        #region Fields
        /// <summary>
        /// Value Provided by DI container to manage Models.
        /// </summary>
        private readonly ICategoryService _categoryService;
        private readonly IWpfToastNotificationService _wpfToastNotificationService;
        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;
        #endregion

        #region Props

        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditCategoryCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;


        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private string? _imagePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedImage))]
        private string? _selectedImagePreview;
        public bool HasSelectedImage => !string.IsNullOrEmpty(SelectedImagePreview);

        [ObservableProperty]
        private CategoryDto? _categoryToEdit;

        #endregion

        #region Constructor
        public EditSubCategoryDialogViewModel(
            ICategoryService categoryService,
            ILocalizationService localizationService,
            IWpfToastNotificationService wpfToastNotificationService,
            ILogger<EditSubCategoryDialogViewModel> logger) : base(localizationService, logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));

        }
        #endregion

        #region Initializer

        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/> to initialize <see cref="EditSubCategoryDialogViewModel"/>
        /// </summary>
        /// <param name="parameter"><see cref="MakeDto"/></param>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (parameter is not CategoryDto categoryDto) // Use 'is not' for clearer null check and type check
            {
                _logger.LogError("The parameter provided to EditSubCategoryDialogViewModel is null or not a CategoryDto.");
                // Potentially close dialog or show error immediately
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Initialization Error", "Cannot edit category: Invalid data provided.", true, CurrentWorkFlow);
                SetResultAndClose(false); // Close dialog if initialization fails
                throw new ArgumentNullException(nameof(parameter), "CategoryDto parameter is required for editing.");
            }
            if (IsInitialized) return;

            CategoryToEdit = categoryDto; // Store the original DTO

            Name = CategoryToEdit.Name;
            Description = CategoryToEdit.Description ?? string.Empty;
            ImagePath = CategoryToEdit.ImagePath; // Store original persistent path
            IsActive = CategoryToEdit.IsActive;
            SelectedImagePreview = ImagePath; // Preview initially shows existing image

            RemoveImageCommand.NotifyCanExecuteChanged();
            await base.InitializeAsync(parameter);
        }

        #endregion

        #region Commands

        private bool CanEditCategory()
        {
            return !string.IsNullOrWhiteSpace(Name) && CategoryToEdit != null && !IsEditing;
        }


        /// <summary>
        /// Edit <see cref="CategoryDto"/>
        /// </summary>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ViewModelException"></exception>
        [RelayCommand(CanExecute = nameof(CanEditCategory))]
        private async Task EditCategoryAsync()
        {
            if (CategoryToEdit == null || CategoryToEdit.Id == 0)
            {
                _logger.LogError("CategoryToEdit is null or has an invalid ID during EditCategoryAsync.");
                await MessageBoxHelper.ShowMessageWithoutTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorOccurredPleaseTryAgainStr"] as string ?? "An error occurred, please try again!",
                    true, CurrentWorkFlow);
                SetResultAndClose(false);
                return;
            }

            try
            {
                IsEditing = true;

                // ImagePath here is the *source* path if a new image was selected by the user,
                // or the original persistent path if unchanged,
                // or null if the user cleared the image.
                // The service layer's UpdateCategoryAsync will handle the logic of:
                // - if ImagePath is new source -> save it, delete old
                // - if ImagePath is null -> delete old
                // - if ImagePath is same as original persistent path -> do nothing with files
                var updateCategoryDto = new UpdateCategoryDto(
                    CategoryToEdit.Id,
                    Name.Trim(),
                    string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    ImagePath, // This is the key: it's the current state of the image path from UI
                    CategoryToEdit.ParentCategoryId, // Assuming root categories cannot change parent here
                                                     // If they can, you need a ParentCategory selection in this dialog.
                                                     // For "EditRootCategory", ParentCategoryId should be null.
                    IsActive
                );

                await _categoryService.UpdateCategoryAsync(updateCategoryDto);

                _logger.LogInformation("Category with name = {CategoryName} and ID = {ID} has been updated successfully", updateCategoryDto.Name, updateCategoryDto.Id);

                var successMsgResource = System.Windows.Application.Current.Resources["ItemUpdatedSuccessfullyStr"] as string ?? "Item '{0}' has been updated successfully!";
                var successMsg = string.Format(successMsgResource, updateCategoryDto.Name); // Use string.Format for placeholders

                _wpfToastNotificationService.ShowSuccess(successMsg);
                await MessageBoxHelper.ShowMessageWithoutTitleAsync(successMsg, false, CurrentWorkFlow);
                SetResultAndClose(true);
            }
            catch (ValidationException vex)
            {
                _logger.LogWarning(vex, "Validation failed while updating category: ID='{CategoryId}', Name='{CategoryName}'", CategoryToEdit.Id, Name);
                var errorMessages = string.Join(Environment.NewLine, vex.Errors.Select(e => e.ErrorMessage));
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ValidationErrorStr"] as string ?? "Validation Error",
                    System.Windows.Application.Current.Resources["PleaseCorrectTheFollowingErrorsStr"] as string ?? "Please correct the following errors:",
                    errorMessages, true, CurrentWorkFlow);
            }
            catch (DuplicationException dx)
            {
                _logger.LogWarning(dx, "Duplication error while updating category: ID='{CategoryId}', Name='{CategoryName}'", CategoryToEdit.Id, Name);
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["DuplicateEntryStr"] as string ?? "Duplicate Entry",
                    dx.Message, true, CurrentWorkFlow);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "File IO error while updating category (image handling): ID='{CategoryId}', Name='{CategoryName}'", CategoryToEdit.Id, Name);
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["FileOperationErrorStr"] as string ?? "File Operation Error",
                    System.Windows.Application.Current.Resources["ErrorUpdatingImageStr"] as string ?? "An error occurred while updating the category image. Details may have been saved.",
                    true, CurrentWorkFlow);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while updating category: ID='{CategoryId}', Name='{CategoryName}'", CategoryToEdit.Id, Name);
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["DatabaseErrorStr"] as string ?? "Database Error",
                    System.Windows.Application.Current.Resources["ErrorUpdatingCategoryInDBStr"] as string ?? "A database error occurred while saving category updates. Please try again.",
                    true, CurrentWorkFlow);
            }
            catch (ServiceException srvEx)
            {
                _logger.LogError(srvEx, "Service error while updating category: ID='{CategoryId}', Name='{CategoryName}'", CategoryToEdit.Id, Name);
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["OperationFailedStr"] as string ?? "Operation Failed",
                    srvEx.Message, true, CurrentWorkFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating category: ID='{CategoryId}', Name='{CategoryName}'", CategoryToEdit.Id, Name);
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["UnexpectedErrorStr"] as string ?? "Unexpected Error",
                    System.Windows.Application.Current.Resources["UnexpectedErrorOccurredStr"] as string ?? "An unexpected error occurred. Please try again or contact support.",
                    true, CurrentWorkFlow);
                // For DEV:
                // throw new ViewModelException(ErrorMessages.UPDATE_DATA_EXCEPTION_MESSAGE, nameof(EditRootCategoryDialogViewModel), nameof(EditCategoryAsync), Core.Helpers.Operations.MethodOperationType.UPDATE_DATA, ex);
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
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(EditSubCategoryDialog));
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
