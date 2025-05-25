using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.EngineTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.EngineTypes
{
    public partial class AddEngineTypeDialogViewModel : BaseViewModel<AddEngineTypeDialogViewModel>, IDialogAware
    {
        private IDialogWindow _dialog = null!;
        private readonly ICompatibleVehicleService _compatibleVehicleService;
        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddEngineTypeCommand))]
        private string _name = string.Empty;

        public AddEngineTypeDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<AddEngineTypeDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
        }

        private bool CanAddEngineType()
        {
            return !string.IsNullOrEmpty(Name);
        }

        [RelayCommand(CanExecute = nameof(CanAddEngineType))]
        private async Task AddEngineTypeAsync()
        {
            try
            {
                IsAdding = true;

                // Saving image is handled by back-end service CompatibleVehicleService
                var createTTDTO = new CreateEngineTypeDto(Name, string.Empty);
                var newItem = await _compatibleVehicleService.CreateEngineTypeAsync(createTTDTO);

                _logger.LogInformation("New Engine type with name = {Name} and ID = {ID} has been added successfully", newItem.Name, newItem.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Engine type {newItem.Name} has been created successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while creating Engine type, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The Engine Type {Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while adding a new Engine type, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddEngineTypeDialogViewModel), nameof(AddEngineTypeAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);

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
                _logger.LogError("The Dialog window was null on {VM}", nameof(AddEngineTypeDialog));
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
