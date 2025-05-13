using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs
{
    public partial class AddTrimLevelDialogViewModel : InitializableViewModel<AddTrimLevelDialogViewModel>
    {
        private IDialogWindow _dialog = null!;

        [ObservableProperty] private int _modelId;

        [ObservableProperty] private bool _isAdding = false;

        [ObservableProperty] private ModelDto? _model = null;



        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddTrimCommand))]
        private string _name = string.Empty;

        private readonly ICompatibleVehicleService _compatibleVehicleService;

        public AddTrimLevelDialogViewModel(
            ICompatibleVehicleService compatibleVehicleService, 
            ILocalizationService localizationService, 
            ILogger<AddTrimLevelDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
        }

        public override async Task InitializeAsync(object? parameter)
        {

            if (IsInitialized)
            {
                return;
            }

            if (parameter == null) return;

            var modelId = (int)parameter;
            ModelId = modelId;

            _logger.LogInformation("AddTrimLevelDialogViewModel initialized with parameter {param}", ModelId);


            await base.InitializeAsync(parameter);

            var model = await _compatibleVehicleService.GetModelByIdAsync(ModelId);
            if(model == null)
            {
                _logger.LogError("The model was null, for AddTrimLevelDialogViewModel");
                return;
            }
            Model = model;
        }

        private bool CanAddModel()
        {
            return !string.IsNullOrEmpty(Name);
        }


        [RelayCommand(CanExecute = nameof(CanAddModel))]
        private async Task AddTrimAsync()
        {
            try
            {
                IsAdding = true;

                if (ModelId == 0)
                {
                    _logger.LogError("Failed while adding a new trim level, the make ID was null");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync("Model ID was not found", true, CurrentWorkFlow);
                    SetResultAndClose(false);
                    return;
                }

                var createDto = new CreateTrimLevelDto(Name, ModelId);

                var newTrim = await _compatibleVehicleService.CreateTrimLevelAsync(createDto);

                _logger.LogInformation("New trim with name {ModelName} and ID {Id} has been added successfully", newTrim.Name, newTrim.Id);


                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"New trim {newTrim.Name} has been created successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }
            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while creating Trim level, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The trim {Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed while adding a new trim, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddMakeDialogViewModel), nameof(AddTrimAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);

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


        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the AddTrimLevelDialogViewModel");
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
    }
}
