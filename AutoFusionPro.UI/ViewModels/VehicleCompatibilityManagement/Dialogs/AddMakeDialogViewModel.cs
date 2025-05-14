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
    public partial class AddMakeDialogViewModel : BaseViewModel<AddMakeDialogViewModel>, IDialogAware
    {

        private IDialogWindow _dialog = null!;

        [ObservableProperty]
        private bool _isAdding = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddMakeCommand))]
        private string _name = string.Empty;
        private readonly ICompatibleVehicleService _compatibleVehicleService;

        public AddMakeDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<AddMakeDialogViewModel> logger) : base(localizationService,  logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
        }

        private bool CanAddMake()
        {
            return !string.IsNullOrEmpty(Name);
        }

        [RelayCommand(CanExecute = nameof(CanAddMake))]
        private async Task AddMakeAsync()
        {
            try
            {
                IsAdding = true;

                var createMakeDTO = new CreateMakeDto(Name);

                var newItem = await _compatibleVehicleService.CreateMakeAsync(createMakeDTO);

                _logger.LogInformation("New make with name = {MakeName} and ID = {ID} has been added successfully", newItem.Name, newItem.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Make {newItem.Name} has been created successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while creating Make, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The make {Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex) {

                _logger.LogError("Failed while adding a new Make, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddMakeDialogViewModel), nameof(AddMakeAsync), Core.Helpers.Operations.MethodOperationType.ADD_DATA, ex);

            }
            finally
            {
                IsAdding  = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
        }


        public void SetDialogWindow(IDialogWindow dialog)
        {
            if(dialog == null)
            {
                _logger.LogError("The Dialog window was null on the AddMakeDialogViewModel");
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
