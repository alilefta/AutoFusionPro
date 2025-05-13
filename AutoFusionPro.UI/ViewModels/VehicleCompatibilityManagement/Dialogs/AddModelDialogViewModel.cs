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
    public partial class AddModelDialogViewModel : InitializableViewModel<AddMakeDialogViewModel>, IDialogAware
    {

        private IDialogWindow _dialog = null!;

        [ObservableProperty]
        private int _makeId;

        [ObservableProperty]
        private MakeDto? _make = null;

        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddModelCommand))]
        private string _name = string.Empty;
        private readonly ICompatibleVehicleService _compatibleVehicleService;

        public AddModelDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<AddMakeDialogViewModel> logger) : base(localizationService, logger)
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

            var makeId = (int)parameter;
            MakeId = makeId;

            _logger.LogInformation("AddModelDialogViewModel initialized with parameter {param}", MakeId);

            await base.InitializeAsync(parameter);

            var makeEntity = await _compatibleVehicleService.GetMakeByIdAsync(makeId);

            if (makeEntity == null)
            {
                _logger.LogError("The make was null, for AddModelDialogViewModel");
                return;
            }
            Make = makeEntity;
        }

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

                if(MakeId == 0)
                {
                    _logger.LogError("Failed while adding a new model, the make ID was null");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync("Make ID was not found", true, CurrentWorkFlow);
                    SetResultAndClose(false);
                    return;
                }

                var createModelDto = new CreateModelDto(Name, MakeId);

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


        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the AddModelDialogViewModel");
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
