using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.MakesModelsTrims;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.Transmissions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.Transmissions
{
    public partial class AddTransmissionTypeDialogViewModel : BaseViewModel<AddTransmissionTypeDialogViewModel>, IDialogAware
    {

        private IDialogWindow _dialog = null!;
        private readonly IVehicleTaxonomyService _compatibleVehicleService;
        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddTransmissionTypeCommand))]
        private string _name = string.Empty;

        public AddTransmissionTypeDialogViewModel(IVehicleTaxonomyService compatibleVehicleService, ILocalizationService localizationService, ILogger<AddTransmissionTypeDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
        }

        private bool CanAddTransmissionType()
        {
            return !string.IsNullOrEmpty(Name);
        }

        [RelayCommand(CanExecute = nameof(CanAddTransmissionType))]
        private async Task AddTransmissionTypeAsync()
        {
            try
            {
                IsAdding = true;

                // Saving image is handled by back-end service CompatibleVehicleService
                var createTTDTO = new CreateLookupDto(Name);
                var newItem = await _compatibleVehicleService.CreateTransmissionTypeAsync(createTTDTO);

                _logger.LogInformation("New transmission type with name = {MakeName} and ID = {ID} has been added successfully", newItem.Name, newItem.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Transmission type {newItem.Name} has been created successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while creating Transmission type, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The make {Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while adding a new transmission type, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddTransmissionTypeDialogViewModel), nameof(AddTransmissionTypeAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);

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
                _logger.LogError("The Dialog window was null on {VM}", nameof(AddTransmissionTypeDialog));
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
