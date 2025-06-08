using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
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
    public partial class AddSubCategoryDialogViewModel : InitializableViewModel<AddSubCategoryDialogViewModel>, IDialogAware
    {

        #region Fields
        /// <summary>
        /// Value Provided by DI container to manage Models.
        /// </summary>
        private readonly ICategoryService _categoryService;
        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;
        #endregion

        #region Props

        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCategoryCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private CategoryDto _parentCategory;


        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private string? _imagePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedImage))]
        private string? _selectedImagePreview;

        public bool HasSelectedImage => !string.IsNullOrEmpty(SelectedImagePreview);

        #endregion

        #region Constructor
        public AddSubCategoryDialogViewModel(ICategoryService categoryService, ILocalizationService localizationService, ILogger<AddSubCategoryDialogViewModel> logger) : base(localizationService, logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }
        #endregion



        #region Initializer

        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/> To Initialize <see cref="AddSubCategoryDialogViewModel"/>
        /// </summary>
        /// <param name="parameter"><see cref="CategoryDto"/></param>
        /// <returns><see cref="Void"/></returns>
        public override async Task InitializeAsync(object? parameter)
        {
            if (parameter is not CategoryDto categoryDto) {
                _logger.LogError("The Parent Category was null for AddSubCategoryDialogViewModel");
                throw new ArgumentNullException(nameof(categoryDto));
                        }

            if (IsInitialized)
            {
                return;
            }

            if (parameter == null) return;

            var parentCategory = (CategoryDto)parameter;
            ParentCategory = parentCategory;

            _logger.LogInformation("AddSubCategoryDialogViewModel initialized with parameter {param}", parentCategory.Id);

            await base.InitializeAsync(parameter);
        }

        #endregion


        #region Commands
        private bool CanAddCategory()
        {
            return !string.IsNullOrEmpty(Name) || ParentCategory != null;
        }

        [RelayCommand(CanExecute = nameof(CanAddCategory))]
        private async Task AddCategoryAsync()
        {
            try
            {
                IsAdding = true;

                // Saving image is handled by back-end service CompatibleVehicleService
                var createCategoryDto = new CreateCategoryDto(
                    Name,
                    string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    ImagePath, 
                    ParentCategory.Id,
                    IsActive);
                var newItem = await _categoryService.CreateCategoryAsync(createCategoryDto);

                _logger.LogInformation("New category with name = {categoryName} and ID = {ID} has been added successfully", newItem.Name, newItem.Id);

                var msg = System.Windows.Application.Current.Resources["ItemAddedSuccessfullyStr"] as string ?? "Item has been added successfully!";

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"{newItem.Name} {msg}", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (ValidationException vex) // From FluentValidation in the service
            {
                _logger.LogWarning(vex, "Validation failed while creating category: Name='{CategoryName}'", Name);
                // Construct a user-friendly message from validation errors
                var errorMessages = string.Join(Environment.NewLine, vex.Errors.Select(e => e.ErrorMessage));
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ValidationErrorStr"] as string ?? "Validation Error",
                    System.Windows.Application.Current.Resources["PleaseCorrectTheFollowingErrorsStr"] as string ?? "Please correct the following errors:",
                    errorMessages,
                    true, CurrentWorkFlow);
                // ValidationMessage = errorMessages; // Optionally set a property for UI binding
            }
            catch (DuplicationException dx) // Your custom duplication exception
            {
                _logger.LogWarning(dx, "Duplication error while creating category: Name='{CategoryName}'", Name);
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["DuplicateEntryStr"] as string ?? "Duplicate Entry",
                    dx.Message, // Use the message from the exception
                    true, CurrentWorkFlow);
            }
            catch (IOException ioEx) // From _imageFileService
            {
                _logger.LogError(ioEx, "File IO error while creating category (image handling): Name='{CategoryName}'", Name);
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["FileOperationErrorStr"] as string ?? "File Operation Error",
                    System.Windows.Application.Current.Resources["ErrorSavingImageStr"] as string ?? "An error occurred while saving the category image. Please try again or select a different image.",
                    true, CurrentWorkFlow);
            }
            catch (DbUpdateException dbEx) // From _unitOfWork.SaveChangesAsync()
            {
                _logger.LogError(dbEx, "Database update error while creating category: Name='{CategoryName}'", Name);
                // Check for specific DB errors if possible (e.g., unique constraint on a field not caught by validator)
                // For now, a general message.
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["DatabaseErrorStr"] as string ?? "Database Error",
                    System.Windows.Application.Current.Resources["ErrorSavingCategoryToDBStr"] as string ?? "A database error occurred while saving the category. Please try again.",
                    true, CurrentWorkFlow);
            }
            catch (ServiceException srvEx) // Your general service layer exception
            {
                _logger.LogError(srvEx, "Service error while creating category: Name='{CategoryName}'", Name);
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["OperationFailedStr"] as string ?? "Operation Failed",
                    srvEx.Message, // Use the message from the service exception
                    true, CurrentWorkFlow);
            }
            catch (Exception ex) // Catch-all for unexpected errors
            {
                _logger.LogError(ex, "Unexpected error while creating category: Name='{CategoryName}'", Name);
                // Do NOT re-throw ViewModelException from here typically, unless your global handler needs it.
                // Instead, show a user-friendly message.
                await MessageBoxHelper.ShowMessageWithTitleAsync(
                    System.Windows.Application.Current.Resources["ErrorStr"] as string ?? "Error",
                    System.Windows.Application.Current.Resources["UnexpectedErrorStr"] as string ?? "Unexpected Error",
                    System.Windows.Application.Current.Resources["UnexpectedErrorOccurredStr"] as string ?? "An unexpected error occurred. Please try again or contact support.",
                    true, CurrentWorkFlow);

                // For DEV:
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddSubCategoryDialogViewModel), nameof(AddCategoryAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);
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
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on {VM}", nameof(AddSubCategoryDialog));
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
