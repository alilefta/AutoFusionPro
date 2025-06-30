using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs;
using AutoFusionPro.UI.Views.Parts.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Parts
{
    public partial class PartsViewModel : BaseViewModel<PartsViewModel>
    {
        #region Fields

        private readonly IPartService _partService;
        private readonly IDialogService _dialogService;

        #endregion

        #region Props

        [ObservableProperty]
        private ObservableCollection<PartSummaryDto> _parts = new();

        [ObservableProperty]
        private bool _isLoading = false;        
        
        [ObservableProperty]
        private bool _isAddingPart = false;

        [ObservableProperty]
        private bool _isEditingPart = false;

        #endregion

        public PartsViewModel(IPartService partService, 
            ILocalizationService localizationService, 
            IDialogService dialogService,
            ILogger<PartsViewModel> logger) : base(localizationService, logger)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            _ = LoadPartsData();
        }

        #region Load and Initialize


        private async Task LoadPartsData()
        {
            IsLoading = true;

            try
            {
                Parts.Clear();

                var filterCriteria = new PartFilterCriteriaDto();

                var partsData = await _partService.GetFilteredPartSummariesAsync(filterCriteria, 0, 50);

                if (partsData.Items.Any())
                { 
                    foreach (var part in partsData.Items)
                    {
                        Parts.Add(part);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying to load Parts data, {ex.Message}");
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(LoadPartsData), MethodOperationType.LOAD_DATA, ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Commands 

        [RelayCommand]
        public async Task ShowAddPartDialogAsync() 
        {
            IsAddingPart = true;
            try
            {
                bool? result = await _dialogService.ShowDialogAsync<AddEditPartDialogViewModel, AddEditPartDialog>(null);
               
                if (result.HasValue && result.Value == true)
                {
                    RefreshData();
                    _logger.LogInformation("AddEditPartDialog was called and returned results.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception happened while opening AddPartDialog, {ex.Message}");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(ShowAddPartDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }finally
            {
                IsAddingPart = false;
            }

        }

        [RelayCommand]
        public async Task ShowEditPartDialogAsync(PartSummaryDto? partToEdit)
        {
            if(partToEdit is null)
            {
                _logger.LogError("The Part Details was null");
                return;
            }

            IsEditingPart = true;
            try
            {

                var partDetails = await _partService.GetPartDetailsByIdAsync(partToEdit.Id);

                bool? result = await _dialogService.ShowDialogAsync<AddEditPartDialogViewModel, AddEditPartDialog>(partDetails);

                if (result.HasValue && result.Value == true)
                {
                    RefreshData();
                    _logger.LogInformation("AddEditPartDialog was called and returned results.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception happened while opening AddPartDialog, {ex.Message}");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(PartsViewModel), nameof(ShowAddPartDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsEditingPart = false;
            }

        }

        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            await LoadPartsData();
        }
        #endregion

        public async void RefreshData()
        {
            await LoadPartsData();
        }

    }
}
