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
    /// <summary>
    /// Used for updating the <see cref="Name"/> for <see cref="MakeDto"/> 
    /// Requires <see cref="InitializeAsync(object?)"/> to be called from the <see cref="IDialogService"/> or by other Caller, to send the <see cref="MakeDto"/>  to be updated.
    /// Implemented <see cref="InitializableViewModel{TViewModel}"/> for the <see cref="InitializeAsync(object?)"/> Initialization process and <see cref="IDialogAware"/> to be able to set the associated Dialog Window via <see cref="IDialogWindow"/>. 
    /// To be able to set results and close <seealso cref="SetResultAndClose(bool)"/>
    /// </summary>
    public partial class EditMakeDialogViewModel : InitializableViewModel<EditMakeDialogViewModel>, IDialogAware
    {

        private IDialogWindow _dialog = null!;

        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditMakeCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private MakeDto? _makeToEdit;

        private readonly ICompatibleVehicleService _compatibleVehicleService;

        public EditMakeDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<EditMakeDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));

        }

        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/>.
        /// </summary>
        /// <param name="parameter"><see cref="MakeDto"/></param>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (parameter == null)
            {
                _logger.LogError("The parameter is null!");
                throw new ArgumentNullException(nameof(parameter));
            }

            if (IsInitialized)
            {
                return;
            }

            MakeToEdit = (MakeDto)parameter;

            Name = MakeToEdit.Name;


            await base.InitializeAsync(parameter);
        }


        private bool CanEditMake()
        {
            return !string.IsNullOrEmpty(Name);
        }


        /// <summary>
        /// Edit <see cref="UpdateMakeDto"/>
        /// </summary>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ViewModelException"></exception>
        [RelayCommand(CanExecute = nameof(CanEditMake))]
        private async Task EditMakeAsync()
        {
            try
            {

                if (MakeToEdit == null)
                {
                    _logger.LogError("The make to be updated is Null!");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync($"An error occurred, please, try again!", true, CurrentWorkFlow);

                    SetResultAndClose(false);

                    return;
                }


                IsEditing = true;

                var updateMakeDTO = new UpdateMakeDto(MakeToEdit.Id, Name);

                 await _compatibleVehicleService.UpdateMakeAsync(updateMakeDTO);

                _logger.LogInformation("Make with name = {MakeName} and ID = {ID} has been updated successfully", updateMakeDTO.Name, updateMakeDTO.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Make {updateMakeDTO.Name} has been updated successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while updating Make, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The make {Name} is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while updating Make, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.UPDATE_DATA_EXCEPTION_MESSAGE, nameof(EditMakeDialogViewModel), nameof(EditMakeAsync), Core.Helpers.Operations.MethodOperationType.UPDATE_DATA, ex);

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

        /// <summary>
        /// Provided by <see cref="IDialogAware"/> to set the window for Setting Dialog Results and Closing purposes <see cref="SetResultAndClose(bool)"/>
        /// </summary>
        /// <param name="dialog">Dialog will be provided internally by the service.</param>
        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the EditMakeDialogViewModel");
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
    }
}
