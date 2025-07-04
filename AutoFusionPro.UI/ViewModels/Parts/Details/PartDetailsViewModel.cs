using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Interfaces.Navigation;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEdit;
using AutoFusionPro.UI.Views.Parts.Dialogs.AddEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Parts.Details
{
    public partial class PartDetailsViewModel : InitializableViewModel<PartDetailsViewModel>
    {
        #region Fields
        private readonly IPartService _partService;
        private readonly IDialogService _dialogService;
        private readonly IWpfToastNotificationService _toastNotificationService;
        private readonly INavigationService _navigationService;
        #endregion

        #region Props

        [ObservableProperty]
        private PartDetailDto? _partDetailDto;

        [ObservableProperty]
        private PartSummaryDto? _partSummaryDto;

        [ObservableProperty]
        private bool _isLoading = false;        
        
        [ObservableProperty]
        private bool _isEditing = false;        
        
        [ObservableProperty]
        private bool _isRemoving = false;

        #endregion

        #region Constructor
        public PartDetailsViewModel(
            IPartService partService, 
            IDialogService dialogService, 
            IWpfToastNotificationService wpfToastNotificationService,
            INavigationService navigationService,
            ILocalizationService localizationService, ILogger<PartDetailsViewModel> logger) : base(localizationService, logger)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _toastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }
        #endregion

        #region Initialization And Data Loading

        /// <summary>
        /// Initialization method provided by <see cref="InitializableViewModel{TViewModel}"/> to load <see cref="PartSummaryDto"/> parameter required to allow <see cref="CategoryDetailViewModel"/> to work.
        /// </summary>
        /// <param name="parameter"><see cref="PartSummaryDto"/></param>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ArgumentNullException">If <see cref="PartSummaryDto"/> param was null</exception>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (parameter is not PartSummaryDto partSummaryDto)
            {
                _logger.LogError("The parameter provided to PartDetailsViewModel is null or not a PartSummaryDto.");
                // Potentially close dialog or show error immediately
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Initialization Error", "Cannot get PartSummaryDto: Invalid data provided.", true, CurrentWorkFlow);
                throw new ArgumentNullException(nameof(parameter), "PartSummaryDto parameter is required for editing.");
            }
            if (IsInitialized) return;

            PartSummaryDto = partSummaryDto;

            await LoadPartDetailsAsync();

            //await BuildBreadcrumbTrailAsync(categoryDto);

           // await LoadSubCategoriesAsync();
            //await LoadPartsAsync();

            await base.InitializeAsync(parameter);
        }


        private async Task LoadPartDetailsAsync()
        {
            if (!IsInitialized) return;

            if(PartSummaryDto ==  null) return;

            IsLoading = true;

            try
            {
                var partDetails = await _partService.GetPartDetailsByIdAsync(PartSummaryDto.Id);

                if (partDetails == null)
                {
                    _logger.LogError("The Part details returned null value.");
                    return;
                    // Should navigate back
                }

                PartDetailDto = partDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                IsLoading = false;
            }
        }

        #endregion


        #region Commands

        [RelayCommand]
        public async Task ShowEditPartDialogAsync()
        {
            if (PartDetailDto == null) return;
            try
            {
                IsEditing = true;

                bool? results = await _dialogService.ShowDialogAsync<AddEditPartDialogViewModel, AddEditPartDialog>(PartDetailDto);
                _logger.LogInformation("Part With Id {Id} has been updated successfully", PartDetailDto.Id);

                if (results.HasValue && results.Value == true)
                {
                    await LoadPartDetailsAsync();
                    var msg = System.Windows.Application.Current.Resources["ItemUpdatedSuccessfullyStr"] as string ?? "Part Has Been Updated Successfully!";
                    _toastNotificationService.ShowSuccess(msg);
                }

            }
            catch (Exception ex)
            {
                var msg = System.Windows.Application.Current.Resources["UpdateOperationFailedStr"] as string ?? "Update Operation Failed";
                _toastNotificationService.ShowError(msg);
                return;
            }
            finally
            {
                IsEditing = false;

            }
        }

        [RelayCommand]
        public async Task ShowDeletePartDialogAsync()
        {
            if (PartDetailDto == null) return;

            try
            {
                IsRemoving = true;

                var results = _dialogService.ShowConfirmDeleteItemsDialog(1);

                if (results.HasValue && results.Value == true)
                {

                    try
                    {
                        await _partService.DeactivatePartAsync(PartDetailDto.Id);


                        var msg = System.Windows.Application.Current.Resources["ItemDeletedSuccessfullyStr"] as string ?? "Item Has Been Deleted Successfully!";


                        _toastNotificationService.ShowSuccess(msg);

                        _logger.LogInformation("Part with ID={ID} has been deleted successfully!", PartDetailDto.Id);

                        _navigationService.GoBack();


                    }
                    catch (DeletionBlockedException dx)
                    {
                        _logger.LogError(dx, "{ErrorMessage}. Entity: {EntityName}, Key: {EntityKey}, Dependents: {Dependents}",
                            ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, dx.EntityName, dx.EntityKey, string.Join(", ", dx.DependentEntityTypes ?? Enumerable.Empty<string>()));

                        // Construct a more user-friendly message
                        string entityDisplayName = dx.EntityName ?? "the item"; // Fallback
                        string dependentItemsText = "other items"; // Default
                        if (dx.DependentEntityTypes != null && dx.DependentEntityTypes.Any())
                        {
                            dependentItemsText = string.Join(" and ", dx.DependentEntityTypes.Select(FormatDependentTypeName));
                        }

                        var msgTitle = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";
                        var msg = System.Windows.Application.Current.Resources["BecauseTheFollowingItemsDependOnItStr"] as string ?? $"Because the following Items Depends on it:";


                        _toastNotificationService.Show($"{msg}\n{dependentItemsText}", msgTitle, Core.Enums.UI.ToastType.Error, TimeSpan.FromSeconds(10));
                        return;
                    }
                    catch (ServiceException ex)
                    {
                        _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                        var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                        _toastNotificationService.ShowError(msg);

                        // FOR DEV ONLY
                        throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(PartDetailsViewModel), nameof(ShowDeletePartDialogAsync), MethodOperationType.DELETE_DATA, ex);


                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE}, while deleting category Item");
                        var msg = System.Windows.Application.Current.Resources["DeleteOperationFailedStr"] as string ?? "Cannot Delete the Item!";

                        _toastNotificationService.ShowError(msg);

                        // FOR DEV ONLY
                        throw new ViewModelException(ErrorMessages.DELETE_DATA_EXCEPTION_MESSAGE, nameof(PartDetailsViewModel), nameof(ShowDeletePartDialogAsync), MethodOperationType.DELETE_DATA, ex);
                    }
                }
            }
            finally
            {
                IsRemoving = false;

            }

        }

        [RelayCommand]
        public void GoBack()
        {
            _navigationService.GoBack();
        }


        #endregion

        #region Helpers

        private string FormatDependentTypeName(string entityType)
        {
            // Simple pluralization and spacing, can be more sophisticated with localization
            if (entityType.EndsWith("y"))
                return entityType.Substring(0, entityType.Length - 1) + "ies";
            if (entityType.EndsWith("s"))
                return entityType + "es"; // Or handle specific cases
            return entityType + "s";
        }

        #endregion

    }
}
