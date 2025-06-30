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
    /// <summary>
    /// 
    /// </summary>
    public partial class EditEngineTypeDialogViewModel : InitializableViewModel<EditEngineTypeDialogViewModel>, IDialogAware
    {
        private IDialogWindow _dialog = null!;
        private readonly IVehicleTaxonomyService _compatibleVehicleService;

        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditEngineTypeCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private EngineTypeDto? _engineTypeToEdit;


        public EditEngineTypeDialogViewModel(IVehicleTaxonomyService compatibleVehicleService, ILocalizationService localizationService, ILogger<EditEngineTypeDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));

        }


        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/>.
        /// </summary>
        /// <param name="parameter">Model of Type <see cref="EngineTypeDto"/></param>
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

            EngineTypeToEdit = (EngineTypeDto)parameter;

            Name = EngineTypeToEdit.Name;

            await base.InitializeAsync(parameter);
        }

        #region Commands 

        private bool CanEditEngineType()
        {
            return !string.IsNullOrEmpty(Name);
        }


        /// <summary>
        /// Edit <see cref="UpdateEngineTypeDto"/>
        /// </summary>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ViewModelException"></exception>
        [RelayCommand(CanExecute = nameof(CanEditEngineType))]
        private async Task EditEngineTypeAsync()
        {
            try
            {
                if (EngineTypeToEdit == null)
                {
                    _logger.LogError("The Engine type to be updated is Null!");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync($"An error occurred, please, try again!", true, CurrentWorkFlow);

                    SetResultAndClose(false);

                    return;
                }

                IsEditing = true;
                var updateEngineTypeDto = new UpdateEngineTypeDto(EngineTypeToEdit.Id, Name, string.Empty);
                await _compatibleVehicleService.UpdateEngineTypeAsync(updateEngineTypeDto);

                _logger.LogInformation("Engine Type with with name={Name} and ID={ID} has been updated successfully", updateEngineTypeDto.Name, updateEngineTypeDto.Id);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Engine Type '{updateEngineTypeDto.Name}' has been updated successfully!", false, CurrentWorkFlow);

                SetResultAndClose(true);
            }

            catch (DuplicationException dx)
            {
                _logger.LogError("An error occurred while updating Engine, {Message}", dx.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Choose different name", $"The Engine Type '{Name}' is already exists!", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed while updating Engine Type, {message}", ex.Message);
                throw new ViewModelException(ErrorMessages.UPDATE_DATA_EXCEPTION_MESSAGE, nameof(EditEngineTypeDialogViewModel), nameof(EditEngineTypeAsync), Core.Helpers.Operations.MethodOperationType.UPDATE_DATA, ex);

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
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(EditEngineTypeDialog));
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
