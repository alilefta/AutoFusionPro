using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.Parts.Dialogs.AddEditPartDialogTabs.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEdit.AddEditPartDialogs
{
    public partial class EditSupplierLinkDialogViewModel : InitializableViewModel<EditSupplierLinkDialogViewModel>, IDialogViewModelWithResult<PartSupplierDto>
    {
        #region Fields

        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;

        #endregion

        #region General Props

        [ObservableProperty]
        private bool _isSaving = false;

        #endregion

        #region Props

        [ObservableProperty]
        private string? _supplierName;

        [ObservableProperty]
        private string? _supplierPartNumberForThisPart;

        [ObservableProperty]
        private decimal _costForThisPart;

        [ObservableProperty]
        private int _leadTimeInDaysForThisPart;

        [ObservableProperty]
        private int _minimumOrderQuantityForThisPart;

        [ObservableProperty]
        private bool _isPreferredSupplierForThisPart;

        [ObservableProperty]
        private PartSupplierDto? _supplierPartDtoToEdit;

        [ObservableProperty]
        private PartSupplierDto? _supplierPartDtoToSave;

        #endregion

        public EditSupplierLinkDialogViewModel(ILocalizationService localizationService, ILogger<EditSupplierLinkDialogViewModel> logger) : base(localizationService, logger)
        {
        }


        #region Initializer

        /// <summary>
        /// Provided by <see cref="InitializableViewModel{TViewModel}"/> to initialize <see cref="EditSupplierLinkDialogViewModel"/>
        /// </summary>
        /// <param name="parameter"><see cref="PartSupplierDto"/></param>
        /// <returns><see cref="Void"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (parameter is not PartSupplierDto partSupplierDto) // Use 'is not' for clearer null check and type check
            {
                _logger.LogError("The parameter provided to EditSupplierLinkDialogViewModel is null or not a PartSupplierDto.");
                // Potentially close dialog or show error immediately
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Initialization Error", "Cannot edit Part-Supplier link: Invalid data provided.", true, CurrentWorkFlow);
                SetResultAndClose(false); // Close dialog if initialization fails
                throw new ArgumentNullException(nameof(parameter), "PartSupplierDto parameter is required for editing.");
            }
            if (IsInitialized) return;

            SupplierPartDtoToEdit = partSupplierDto;

            SupplierName = partSupplierDto.SupplierName;
            SupplierPartNumberForThisPart = partSupplierDto.SupplierPartNumber ?? string.Empty;
            CostForThisPart = partSupplierDto.Cost;
            LeadTimeInDaysForThisPart = partSupplierDto.LeadTimeInDays;
            MinimumOrderQuantityForThisPart = partSupplierDto.MinimumOrderQuantity;
            IsPreferredSupplierForThisPart = partSupplierDto.IsPreferredSupplier;

            await base.InitializeAsync(parameter);
        }

        #endregion

        #region Command Methods

        [RelayCommand]
        private async Task SavePartSupplierLinkAsync()
        {
            try
            {
                IsSaving = true;

                // JUST create the DTO and set results and close

                SupplierPartDtoToSave = new PartSupplierDto(SupplierPartDtoToEdit.Id, SupplierPartDtoToEdit.SupplierId, SupplierPartDtoToEdit.SupplierName, SupplierPartNumberForThisPart, CostForThisPart, IsPreferredSupplierForThisPart, LeadTimeInDaysForThisPart, MinimumOrderQuantityForThisPart);

                SetResultAndClose(true);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsSaving = false;
            }

        }

        [RelayCommand]
        private void Cancel()
        {
            SetResultAndClose(false);
        }

        #endregion


        #region Dialog Methods

        /// <summary>
        /// Provided by <see cref="IDialogAware"/>
        /// </summary>
        /// <param name="dialog">Dialog will be provided internally by the service.</param>
        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(EditSupplierLinkDialog));
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


        public PartSupplierDto? GetResult()
        {
            return SupplierPartDtoToSave; // Return the filters set by ApplyFilters
        }

        #endregion
    }
}
