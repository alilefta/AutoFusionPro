using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.Parts.Dialogs.AddEditPartDialogTabs.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEdit.AddEditPartDialogs
{
    public partial class LinkNewSupplierDialogViewModel : BaseViewModel<LinkNewSupplierDialogViewModel>, IDialogViewModelWithResult<PartSupplierCreateDto>
    {
        #region Fields

        /// <summary>
        /// Managing Parts backend service
        /// </summary>
        private readonly IPartService _partService;

        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;

        #endregion

        #region General Props

        [ObservableProperty]
        private bool _isSaving = false;

        #endregion


        //[ObservableProperty]
        // public ObservableCollection<SupplierSelectionDto> AvailableSuppliers = new(); //(Loaded from ISupplierService.GetAllSuppliersForSelectionAsync())
        // [ObservableProperty]
        // SupplierSelectionDto? SelectedSupplier;

        #region Props 

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
        private PartSupplierCreateDto _supplierCreateDto;

        #endregion

        #region Constructor

        public LinkNewSupplierDialogViewModel(IPartService partService, ILocalizationService localizationService, ILogger<LinkNewSupplierDialogViewModel> logger) : base(localizationService, logger)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
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

                //SupplierCreateDto = new PartSupplierCreateDto(SelectedSupplier.Id, SupplierPartNumberForThisPart, CostForThisPart, LeadTimeInDaysForThisPart, MinimumOrderQuantityForThisPart, IsPreferredSupplierForThisPart);

                //await _partService.AddSupplierToPartAsync(CurrentPartId createPartSupplierLink);
                
                

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
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(LinkNewSupplierDialog));
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


        public PartSupplierCreateDto? GetResult()
        {
            return SupplierCreateDto; // Return the filters set by ApplyFilters
        }

        #endregion
    }
}
