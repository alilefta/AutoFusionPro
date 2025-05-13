using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.TabsViewModels
{
    public partial class MakesModelsTrimsManagementViewModel : BaseViewModel<MakesModelsTrimsManagementViewModel>, ITabViewModel
    {
        private readonly IWpfToastNotificationService _wpfToastNotificationService;
        private readonly ICompatibleVehicleService _compatibleVehicleService;
        private readonly IDialogService _dialogService;


        #region General Properties

        [ObservableProperty] private bool _isVisible = false;
        [ObservableProperty] private bool _isLoading = false;
        [ObservableProperty] private bool _isLoadingMakes = false;
        [ObservableProperty] private bool _isLoadingModels = false;
        [ObservableProperty] private bool _isLoadingTrims = false;

        [ObservableProperty] private bool _isAddingMake = false;
        [ObservableProperty] private bool _isAddingModel = false;
        [ObservableProperty] private bool _isAddingTrim = false;

        [ObservableProperty]
        private string _displayName = "Vehicle Structure";

        [ObservableProperty]
        private string _icon = "";

        #endregion


        #region Collections

        [ObservableProperty]
        private ObservableCollection<MakeDto> _makesCollection;

        [ObservableProperty]
        private ObservableCollection<ModelDto> _modelsCollection;

        [ObservableProperty]
        private ObservableCollection<TrimLevelDto> _trimLevelsCollection;

        #endregion


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedMake))]
        private MakeDto _selectedMake;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedModel))]
        private ModelDto _selectedModel;

        [ObservableProperty]
        private TrimLevelDto _selectedTrimLevel;

        public bool HasSelectedMake => SelectedMake != null;
        public bool HasSelectedModel => SelectedModel != null;

        //public ICommand EditMakeCommand { get; set; }


        public MakesModelsTrimsManagementViewModel(
            IWpfToastNotificationService wpfToastNotificationService,
            ICompatibleVehicleService compatibleVehicleService,
            IDialogService dialogService,
            ILocalizationService localizationService,
            ILogger<MakesModelsTrimsManagementViewModel> logger) : base(localizationService, logger)
        {
            _wpfToastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            //EditMakeCommand = new AutoFusionPro.Application.Commands.RelayCommand(o => )

            _ = LoadMakesDataAsync();
        }


        #region Loading Data

        private async Task LoadMakesDataAsync()
        {
            try
            {
                IsLoadingMakes = true;

                var makes = await _compatibleVehicleService.GetAllMakesAsync();
                if (makes.Any())
                {
                    MakesCollection = new ObservableCollection<MakeDto>(makes);
                }
            }
            catch (Exception ex)
            {
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(LoadMakesDataAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingMakes = false;
            }
        }

        private async Task LoadModelsForMakeAsync(int makeId)
        {
            try
            {
                IsLoadingModels = true;

                var models = await _compatibleVehicleService.GetModelsByMakeIdAsync(makeId);
                if (models.Any())
                {
                    ModelsCollection = new ObservableCollection<ModelDto>(models);
                }
            }
            catch (Exception ex)
            {
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(LoadModelsForMakeAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingModels = false;
            }
        }

        private async Task LoadTrimsForModelAsync(int modelId)
        {
            try
            {
                IsLoadingTrims = true;

                var trimLevels = await _compatibleVehicleService.GetTrimLevelsByModelIdAsync(modelId);
                if (trimLevels.Any())
                {
                    TrimLevelsCollection = new ObservableCollection<TrimLevelDto>(trimLevels);
                }
            }
            catch (Exception ex)
            {
                throw new ViewModelException(ErrorMessages.LOADING_DATA_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(LoadTrimsForModelAsync), MethodOperationType.LOAD_DATA, ex);

            }
            finally
            {
                IsLoadingTrims = false;
            }
        }

        #endregion


        #region Selection Change 

        partial void OnSelectedMakeChanged(MakeDto value)
        {
            _ = LoadModelsForMakeAsync(value.Id);
        }

        partial void OnSelectedModelChanged(ModelDto value)
        {
            _ = LoadTrimsForModelAsync(value.Id);
        }

        #endregion


        #region Commands

        [RelayCommand]
        public async Task ShowAddMakeDialogAsync()
        {
            try
            {
                IsAddingMake = true;


                // open dialog
                var results = _dialogService.ShowAddMakeDialog();

                if (results.HasValue && results.Value == true)
                {
                    _logger.LogInformation("A new make has been added!");

                    await LoadMakesDataAsync();

                    _wpfToastNotificationService.ShowSuccess($"New make {MakesCollection.Last<MakeDto>()} has been added!");

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening AddMakeDialog");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowAddMakeDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }finally
            {
                IsAddingMake = false;
            }
        }

        [RelayCommand]
        public async Task ShowAddModelDialogAsync()
        {
            try
            {
                IsAddingModel = true;   

                if(SelectedMake == null)
                {
                    _wpfToastNotificationService.ShowError("You should select a make, before adding a Model!");
                    return;
                }

                var results = await _dialogService.ShowAddModelDialog(SelectedMake.Id);
                if(results.HasValue && results.Value == true)
                {
                    await LoadModelsForMakeAsync(SelectedMake.Id);

                    _wpfToastNotificationService.ShowSuccess($"New model {ModelsCollection.Last()} has been Added!");

                    _logger.LogInformation("A new model {last} has been added!", ModelsCollection.Last());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening AddModelDialog");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowAddModelDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAddingModel = false;
            }
        }

        [RelayCommand]
        public async Task ShowAddTrimLevelDialogAsync()
        {
            try
            {
                IsAddingTrim = true;


                if (SelectedModel == null)
                {
                    _wpfToastNotificationService.ShowError("You should select a model, before adding a trim level!");
                    return;
                }

                var results = await _dialogService.ShowAddTrimLevelDialog(SelectedModel.Id);
                if (results.HasValue && results.Value == true)
                {
                    await LoadTrimsForModelAsync(SelectedModel.Id);

                    _wpfToastNotificationService.ShowSuccess($"New trim {TrimLevelsCollection.Last()} has been Added!");

                    _logger.LogInformation("A new trim level {last} has been added!", TrimLevelsCollection.Last());

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE}, with opening AddTrimLevelDialog");
                throw new ViewModelException(ErrorMessages.OPEN_DIALOG_EXCEPTION_MESSAGE, nameof(MakesModelsTrimsManagementViewModel), nameof(ShowAddTrimLevelDialogAsync), MethodOperationType.OPEN_DIALOG, ex);
            }
            finally
            {
                IsAddingTrim = false;
            }
        }

        [RelayCommand]
        public async Task ShowEditMakeDialogAsync(MakeDto? makeToEdit)
        {
            if (makeToEdit == null) return;
            try
            {
                //var make = makeToEdit as MakeDto;

                //var editDialog = _dialogService.ShowAddMakeDialog();

            }
            catch (Exception ex) { }
        }

        [RelayCommand]
        public async Task ShowEditModelAsync(object param)
        {
            if (param == null) return;
            try
            {
                var make = param as MakeDto;

                //var editDialog = _dialogService.ShowAddMakeDialog();

            }
            catch (Exception ex) { }
        }

        [RelayCommand]
        public async Task ShowEditTrimLevelDialogAsync(object param)
        {
            if (param == null) return;
            try
            {
                var make = param as MakeDto;

                //var editDialog = _dialogService.ShowAddMakeDialog();

            }
            catch (Exception ex) { }
        }

        [RelayCommand]
        public async Task ShowDeleteMakeDialogAsync(object param)
        {
            if (param == null) return;
            try
            {
                var make = param as MakeDto;

                //var editDialog = _dialogService.ShowAddMakeDialog();

            }
            catch (Exception ex) { }
        }

        [RelayCommand]
        public void ShowDeleteModelDialogAsync(object param)
        {
            if (param == null) return;
            try
            {
                var make = param as MakeDto;

                //var editDialog = _dialogService.ShowAddMakeDialog();

            }
            catch (Exception ex) { }
        }

        [RelayCommand]
        public void ShowDeleteTrimLevelDialogAsync(object param)
        {
            if (param == null) return;
            try
            {
                var make = param as MakeDto;

                //var editDialog = _dialogService.ShowAddMakeDialog();

            }
            catch (Exception ex) { }
        }

        #endregion
    }
}
