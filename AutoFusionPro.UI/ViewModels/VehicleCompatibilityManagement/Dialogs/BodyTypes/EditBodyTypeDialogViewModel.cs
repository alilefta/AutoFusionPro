using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.BodyTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.BodyTypes
{
    public partial class EditBodyTypeDialogViewModel : InitializableViewModel<EditBodyTypeDialogViewModel>, IDialogAware
    {
        private IDialogWindow _dialog = null!;
        private readonly ICompatibleVehicleService _compatibleVehicleService;

        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditBodyTypeCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private BodyTypeDto? _bodyTypeToEdit;


        public EditBodyTypeDialogViewModel(ICompatibleVehicleService compatibleVehicleService, ILocalizationService localizationService, ILogger<EditBodyTypeDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));

        }


        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/>.
        /// </summary>
        /// <param name="parameter">Model of Type <see cref="BodyTypeDto"/></param>
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

            BodyTypeToEdit = (BodyTypeDto)parameter;

            Name = BodyTypeToEdit.Name;

            await base.InitializeAsync(parameter);
        }

        #region Commands 

        private bool CanEditBodyType()
        {
            return !string.IsNullOrEmpty(Name);
        }


        /// <summary>
        /// Edit <see cref="UpdateModelDto"/>
        /// </summary>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ViewModelException"></exception>
        [RelayCommand(CanExecute = nameof(CanEditBodyType))]
        private async Task EditBodyTypeAsync()
        {
            try
            {
                if (BodyTypeToEdit == null)
                {
                    _logger.LogError("The Body type to be updated is Null!");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync($"An error occurred, please, try again!", true, CurrentWorkFlow);

                    SetResultAndClose(false);

                    return;
                }

                IsEditing = true;
                var updateBodyTypeDto = new UpdateLookupDto(BodyTypeToEdit.Id, Name);

                await _compatibleVehicleService.UpdateBodyTypeAsync(updateBodyTypeDto);

                _logger.LogInformation("Body Type with with name={Name} and ID={ID} has been updated successfully", updateBodyTypeDto.Name, updateBodyTypeDto.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Body Type '{updateBodyTypeDto.Name}' has been updated successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while updating Body, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The Body Type '{Name}' is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while updating Body Type, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.UPDATE_DATA_EXCEPTION_MESSAGE, nameof(EditBodyTypeDialogViewModel), nameof(EditBodyTypeAsync), Core.Helpers.Operations.MethodOperationType.UPDATE_DATA, ex);

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

        #endregion

        #region Dialog Specific Methods
        /// <summary>
        /// Provided by <see cref="IDialogAware"/> to set the window for Setting Dialog Results and Closing purposes <see cref="SetResultAndClose(bool)"/>
        /// </summary>
        /// <param name="dialog">Dialog will be provided internally by the service.</param>
        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(EditBodyTypeDialog));
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

        #endregion
    }
}
