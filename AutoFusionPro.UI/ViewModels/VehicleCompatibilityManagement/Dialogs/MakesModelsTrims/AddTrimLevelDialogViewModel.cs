using AutoFusionPro.Application.DTOs.CompatibleVehicleDTOs;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs.MakesModelsTrims;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs.MakesModelsTrims
{
    public partial class AddTrimLevelDialogViewModel : InitializableViewModel<AddTrimLevelDialogViewModel>, IDialogAware
    {
        #region Fields
        private IDialogWindow _dialog = null!;
        private readonly ICompatibleVehicleService _compatibleVehicleService;
        #endregion

        #region Props

        [ObservableProperty] private bool _isAdding = false;

        [ObservableProperty] private ModelDto? _model = null;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddTrimCommand))]
        private string _name = string.Empty;

        #endregion

        #region Constructor

        public AddTrimLevelDialogViewModel(
            ICompatibleVehicleService compatibleVehicleService, 
            ILocalizationService localizationService, 
            ILogger<AddTrimLevelDialogViewModel> logger) : base(localizationService, logger)
        {
            _compatibleVehicleService = compatibleVehicleService ?? throw new ArgumentNullException(nameof(compatibleVehicleService));
        }

        #endregion

        #region Initializer

        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/> To initialize <see cref="AddTrimLevelDialogViewModel"/>
        /// </summary>
        /// <param name="parameter"><see cref="ModelDto"/></param>
        /// <returns><see cref="Void"/></returns>
        public override async Task InitializeAsync(object? parameter)
        {

            if (IsInitialized)
            {
                return;
            }

            if (parameter == null) return;

            var model = (ModelDto)parameter;
            Model = model;

            _logger.LogInformation("AddTrimLevelDialogViewModel initialized with parameter {param}", Model.Id);

            await base.InitializeAsync(parameter);

        }

        #endregion

        #region Commands

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

                if (Model == null || Model.Id == 0)
                {
                    _logger.LogError("Failed while adding a new trim level, the make ID was null");

                    await MessageBoxHelper.ShowMessageWithoutTitleAsync("Model ID was not found", true, CurrentWorkFlow);
                    SetResultAndClose(false);
                    return;
                }

                var createDto = new CreateTrimLevelDto(Name, Model.Id);

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

        #endregion

        #region Dialog Specific Methods

        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(AddTrimLevelDialog));
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

        #endregion
    }
}
