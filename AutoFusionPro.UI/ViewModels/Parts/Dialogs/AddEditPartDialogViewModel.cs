using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.DTOs.PartCompatibilityDtos;
using AutoFusionPro.Application.DTOs.PartImage;
using AutoFusionPro.Application.DTOs.UnitOfMeasure;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services.DataServices;
using AutoFusionPro.Core.Enums.UI;
using AutoFusionPro.Core.Exceptions.Service;
using AutoFusionPro.Core.Exceptions.Validation;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs.AddEditPartDialogs;
using AutoFusionPro.UI.Views.Parts.Dialogs;
using AutoFusionPro.UI.Views.Parts.Dialogs.AddEditPartDialogTabs.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs
{
    public partial class AddEditPartDialogViewModel : InitializableViewModel<AddEditPartDialogViewModel>, IDialogAware
    {
        #region Fields
        /// <summary>
        /// Managing Parts back-end service
        /// </summary>
        private readonly IPartService _partService;

        /// <summary>
        /// Managing Categories back-end service
        /// </summary>
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Back-end Unit of measure service
        /// </summary>
        private readonly IUnitOfMeasureService _unitOfMeasureService;
        private readonly IDialogService _dialogService;
        private readonly IVehicleTaxonomyService _vehicleTaxonomyService;

        /// <summary>
        /// Part Compatibility Back-end Service
        /// </summary>
        private readonly IPartCompatibilityRuleService _partCompatibilityRuleService;


        /// <summary>
        /// Value Provided by <see cref="IDialogAware"/> from <see cref="IDialogService"/>
        /// </summary>
        private IDialogWindow _dialog = null!;
        #endregion

        #region General Props

        [ObservableProperty]
        private bool _isEditing = false;

        [ObservableProperty]
        private string _dialogTitle = string.Empty;

        [ObservableProperty]
        private ButtonType _currentMainButtonType = ButtonType.Primary;

        [ObservableProperty]
        private bool _isSaving = false;


        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _isAddingSupplier = false;

        [ObservableProperty]
        private bool _isEditingSupplier = false;

        [ObservableProperty]
        private bool _isRemovingSupplier = false;


        [ObservableProperty]
        private bool _isAddingPartCompatibility = false;

        [ObservableProperty]
        private bool _isEditingPartCompatibility = false;

        [ObservableProperty]
        private bool _isLoadingCategories = false;

        [ObservableProperty]
        private bool _isLoadingUoMs = false;

        #endregion

        #region UI Props

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(CancelButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(NextButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(SaveButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(IsImageAndNotesTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsPricingAndStockTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsSupplierTabEnabled))]
        [NotifyPropertyChangedFor(nameof(IsVehicleCompatibilityTabEnabled))]
        private PartStepsWizard _currentStep = PartStepsWizard.CoreInfo;

        // Button Visibility
        public Visibility BackButtonVisibility => CurrentStep != PartStepsWizard.CoreInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CancelButtonVisibility => CurrentStep == PartStepsWizard.CoreInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NextButtonVisibility => (CurrentStep == PartStepsWizard.CoreInfo || CurrentStep == PartStepsWizard.ImagesAndNotes || CurrentStep == PartStepsWizard.PricingAndStock || CurrentStep == PartStepsWizard.Suppliers) ? Visibility.Visible : Visibility.Collapsed;
        public Visibility SaveButtonVisibility => CurrentStep == PartStepsWizard.CompatibleVehicles ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LoadingVisibility => IsLoading ? Visibility.Visible : Visibility.Collapsed;

        [ObservableProperty]
        private SymbolRegular _nextButtonIcon = SymbolRegular.ArrowRight48;

        [ObservableProperty]
        private int _currentTabIndex = 0;


        [ObservableProperty]
        private bool _isCoreInfoStepFinished = false;

        [ObservableProperty]
        private bool _isImageAndNotesStepFinished = true;

        [ObservableProperty]
        private bool _isPricingAndStockStepFinished = false;

        [ObservableProperty]
        private bool _isSuppliersStepFinished = false;

        public bool IsImageAndNotesTabEnabled => IsCoreInfoStepFinished;
        public bool IsPricingAndStockTabEnabled => IsCoreInfoStepFinished && IsImageAndNotesStepFinished;
        public bool IsSupplierTabEnabled => IsCoreInfoStepFinished && IsImageAndNotesStepFinished && IsPricingAndStockStepFinished;
        public bool IsVehicleCompatibilityTabEnabled => IsCoreInfoStepFinished && IsImageAndNotesStepFinished && IsPricingAndStockStepFinished && IsSuppliersStepFinished;

        #endregion

        #region Part Core Info Tab 

        [ObservableProperty]
        private PartDetailDto? _partToEditOriginalData;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [NotifyPropertyChangedFor(nameof(IsCheckingName))]
        private string _name = string.Empty;


        [ObservableProperty]
        private bool _isCheckingName = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [Required(ErrorMessage = "Part Name is required.")]
        [StringLength(100, ErrorMessage = "Part Name cannot exceed 100 characters.")]
        private string _partNumber = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _manufacturer = string.Empty;

        [ObservableProperty]
        private string _partLocation = string.Empty;

        [ObservableProperty]
        private string _barcode = string.Empty;

        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private bool _isOriginal = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [Required(ErrorMessage = "Category is required.")] // For the DTO object itself
        private CategorySelectionDto? _selectedCategory;

        [ObservableProperty]
        private ObservableCollection<CategorySelectionDto> _availableCategories = new();


        #endregion

        #region Image & Notes Tab

        [ObservableProperty]
        private string? _notes;

        //[ObservableProperty]
        //private string? _imagePath;

        //[ObservableProperty]
        //private ObservableCollection<CreatePartImageDto> _partImages = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasAnyImage))]
        private ObservableCollection<PartImageViewModelItem> _partImagesToDisplay = new();

        public bool HasAnyImage => PartImagesToDisplay.Any();

        [ObservableProperty]
        private bool _isLoadingImages = false;
             
        #endregion

        #region Pricing and Stock

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Cost Price must be non-negative.")]
        private decimal _costPrice;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Selling Price must be non-negative.")]
        // Custom validation for SellingPrice >= CostPrice can be added
        private decimal _sellingPrice;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [Range(0, int.MaxValue, ErrorMessage = "Initial Stock must be non-negative.")]
        private int _initialStockQuantity;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private int _reorderLevel;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private int _minimumStock;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [Required(ErrorMessage = "Stocking Unit of Measure is required.")] // For the DTO object
        [NotifyPropertyChangedFor(nameof(IsSalesUoMDifferent))]
        [NotifyPropertyChangedFor(nameof(IsPurchaseUoMDifferent))]
        private UnitOfMeasureDto? _selectedStockingUnitOfMeasure;

        [ObservableProperty]
        private int _stockingUnitOfMeasureId; //(derived from selection, for DTO)

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSalesUoMDifferent))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToPreviousStepCommand))]
        private UnitOfMeasureDto? _selectedSalesUnitOfMeasure;

        [ObservableProperty]
        private int? _salesUnitOfMeasureId;

        [ObservableProperty]
        private decimal? _salesConversionFactor;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPurchaseUoMDifferent))]
        private UnitOfMeasureDto? _selectedPurchaseUnitOfMeasure;

        [ObservableProperty]
        private int? _purchaseUnitOfMeasureId;

        [ObservableProperty]
        private decimal? _purchaseConversionFactor;

        public bool IsSalesUoMDifferent => (SelectedSalesUnitOfMeasure != null && SelectedSalesUnitOfMeasure.Id != 0); //(Calculated property: SelectedSalesUnitOfMeasure != null && SelectedSalesUnitOfMeasure.Id != SelectedStockingUnitOfMeasure?.Id)

        public bool IsPurchaseUoMDifferent => (SelectedPurchaseUnitOfMeasure != null && SelectedPurchaseUnitOfMeasure.Id != 0);

        [ObservableProperty]
        private ObservableCollection<UnitOfMeasureDto> _availableUnitOfMeasures = new();

        [ObservableProperty]
        private ObservableCollection<UnitOfMeasureDto> _availableSalesUnitOfMeasures = new();

        [ObservableProperty]
        private ObservableCollection<UnitOfMeasureDto> _availablePurchaseUnitOfMeasures = new();

        [ObservableProperty]
        private int _progressBarValue;

        #endregion

        #region Suppliers

        [ObservableProperty]
        private ObservableCollection<PartSupplierDto> _availableSuppliers = new(); //(Loaded from ISupplierService.GetAllSuppliersForSelectionAsync())

        [ObservableProperty]
        private PartSupplierDto? _selectedPartSupplierLink;

        #endregion

        #region Part Compatibility Rules

        [ObservableProperty]
        private ObservableCollection<PartCompatibilityRuleViewModelItem> _currentPartCompatibilities = new();

        // A private list to hold rule definitions when creating a NEW part
        private List<CreatePartCompatibilityRuleDto> _stagedRulesForNewPart = new();

        // Keep this for when you select an item in the grid
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveCompatibilityRuleCommand))]
        private PartCompatibilityRuleViewModelItem? _selectedPartCompatibilityRule;

        [ObservableProperty] private bool _isLoadingCompatibilities = false;
        [ObservableProperty] private bool _isCompatibilityTabContentLoaded = false;
        [ObservableProperty] private bool _isAddingCompatibility = false; // For the "Link" button
        [ObservableProperty] private bool _isRemovingCompatibility = false; // For the remove action

        #endregion

        #region Constructor
        public AddEditPartDialogViewModel(
            IPartCompatibilityRuleService partCompatibilityRuleService,
            IPartService partService,
            ICategoryService categoryService,
            IUnitOfMeasureService unitOfMeasureService,
            IDialogService dialogService,
            IVehicleTaxonomyService vehicleTaxonomyService,
            ILocalizationService localizationService,
            ILogger<AddEditPartDialogViewModel> logger) : base(localizationService, logger)
        {
            _partCompatibilityRuleService = partCompatibilityRuleService ?? throw new ArgumentNullException(nameof(partCompatibilityRuleService));
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _unitOfMeasureService = unitOfMeasureService ?? throw new ArgumentNullException(nameof(unitOfMeasureService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _vehicleTaxonomyService = vehicleTaxonomyService ?? throw new ArgumentNullException(nameof(vehicleTaxonomyService));

        }
        #endregion

        #region Initialization

        public override async Task InitializeAsync(object? parameter = null)
        {
            // Do not call base.InitializeAsync() first if you want to control IsInitialized state
            // based on successful completion of this method's logic.
            if (IsInitialized) return; // Prevent re-initialization

            IsLoading = true; // General loading flag from your BaseViewModel or InitializableViewModel
            try
            {
                var categoriesTask = LoadAvailableCategoriesAsync();
                var uomsTask = LoadAvailableUnitOfMeasuresAsync();
                var partCompatibilityRulesTask = LoadCompatibilitiesForPartAsync();
                await Task.WhenAll(categoriesTask, uomsTask, partCompatibilityRulesTask);

                if (parameter is PartDetailDto partToEdit)
                {
                    IsEditing = true;
                    DialogTitle = System.Windows.Application.Current.Resources["EditPartDialogTitleStr"] as string ?? "Edit Part";
                    CurrentMainButtonType = ButtonType.Warning;
                    PartToEditOriginalData = partToEdit; // Keep a copy of the original

                    // Populate ViewModel properties from partToEdit
                    PartNumber = partToEdit.PartNumber;
                    Name = partToEdit.Name;
                    Description = partToEdit?.Description ?? string.Empty;
                    Manufacturer = partToEdit?.Manufacturer ?? string.Empty;
                    SelectedCategory = AvailableCategories.FirstOrDefault(c => c.Id == partToEdit?.CategoryId);

                    CostPrice = partToEdit?.CostPrice ?? 0;
                    SellingPrice = partToEdit?.SellingPrice ?? 0;
                    // InitialStockQuantity is not typically set from partToEdit.StockQuantity in edit mode,
                    // as partToEdit.StockQuantity is the *base/defined* stock, not necessarily the initial entry value.
                    // CurrentStock (live) is handled by inventory transactions.
                    // If your Part.StockQuantity is meant to be editable for definition purposes, then:
                    // InitialStockQuantity = partToEdit.StockQuantity; // but rename VM prop to e.g., DefinedStockQuantity

                    ReorderLevel = partToEdit.ReorderLevel;
                    MinimumStock = partToEdit.MinimumStock;
                    PartLocation = partToEdit.Location; // Assuming VM prop name
                    Barcode = partToEdit?.Barcode ?? string.Empty;
                    IsActive = partToEdit?.IsActive ?? true;
                    IsOriginal = partToEdit?.IsOriginal ?? false;

                    PartImagesToDisplay.Clear();
                    if (partToEdit.Images != null && partToEdit.Images.Any())
                    {
                        _logger.LogDebug("Populating existing images for Part ID: {PartId}", partToEdit.Id);
                        foreach (var imgDto in partToEdit.Images.OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id))
                        {
                            PartImagesToDisplay.Add(new PartImageViewModelItem(imgDto, this));
                        }
                    }
                    OnPropertyChanged(nameof(HasAnyImage));

                    // UoM 
                    SelectedStockingUnitOfMeasure = AvailableUnitOfMeasures.FirstOrDefault(uom => uom.Id == partToEdit.StockingUnitOfMeasureId);

                    if (partToEdit.SalesUnitOfMeasureId.HasValue && partToEdit.SalesUnitOfMeasureId.Value != 0 && partToEdit.SalesUnitOfMeasureId.Value != partToEdit.StockingUnitOfMeasureId)
                    {
                        SelectedSalesUnitOfMeasure = AvailableSalesUnitOfMeasures.FirstOrDefault(uom => uom.Id == partToEdit.SalesUnitOfMeasureId.Value);
                    }
                    else // No specific sales UoM, it's same as stocking, or not set
                    {
                        SelectedSalesUnitOfMeasure = AvailableSalesUnitOfMeasures.FirstOrDefault(uom => uom.Id == 0); // Select "Same as Stocking" or "--Select--"
                    }
                    SalesConversionFactor = partToEdit.SalesConversionFactor;

                    if (partToEdit.PurchaseUnitOfMeasureId.HasValue && partToEdit.PurchaseUnitOfMeasureId.Value != 0 && partToEdit.PurchaseUnitOfMeasureId.Value != partToEdit.StockingUnitOfMeasureId)
                    {
                        SelectedPurchaseUnitOfMeasure = AvailablePurchaseUnitOfMeasures.FirstOrDefault(uom => uom.Id == partToEdit.PurchaseUnitOfMeasureId.Value);
                    }
                    else
                    {
                        SelectedPurchaseUnitOfMeasure = AvailablePurchaseUnitOfMeasures.FirstOrDefault(uom => uom.Id == 0);
                    }
                    PurchaseConversionFactor = partToEdit.PurchaseConversionFactor;

                    UpdateUoMDifferenceFlagsAndFactors();
                }
                else // ADD mode (parameter is null or not PartDetailDto)
                {
                    IsEditing = false;
                    DialogTitle = System.Windows.Application.Current.Resources["AddPartDialogTitleStr"] as string ?? "Add Part";
                    CurrentMainButtonType = ButtonType.Primary;

                    IsActive = true;
                    IsOriginal = false;
                    PartNumber = "TEST"; // await _partService.GenerateNextPartNumberAsync(); // Example: if you have such a service method
                                         // Default UoMs if applicable

                    SelectedCategory = AvailableCategories.FirstOrDefault(c => c.Id == 0); // If you have a "Select..." item

                    CostPrice = 0;
                    SellingPrice = 0;
                    InitialStockQuantity = 0; // Default initial stock
                    ReorderLevel = 0;
                    MinimumStock = 0;


                    SelectedStockingUnitOfMeasure = AvailableUnitOfMeasures.FirstOrDefault(uom => uom.Name.Equals("Piece", StringComparison.OrdinalIgnoreCase))
                                                    ?? AvailableUnitOfMeasures.FirstOrDefault(uom => uom.Id != 0); // Fallback to first valid UoM
                                                                                                                   // Default Sales and Purchase UoM to "-- Select --" or "Same as Stocking" (ID 0)
                    SelectedSalesUnitOfMeasure = AvailableSalesUnitOfMeasures.FirstOrDefault(uom => uom.Id == 0);
                    SelectedPurchaseUnitOfMeasure = AvailablePurchaseUnitOfMeasures.FirstOrDefault(uom => uom.Id == 0);

                    UpdateUoMDifferenceFlagsAndFactors(); // Crucial call

                }
                // Call base.InitializeAsync() at the END of successful derived class initialization
                // to set IsInitialized = true and complete the TaskCompletionSource.
                await base.InitializeAsync(parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize AddEditPartDialogViewModel with parameter: {@Parameter}", parameter);
                // Show error to user
                // Set an error state on the ViewModel if the dialog is still shown
                // Or, if critical, ensure the dialog doesn't show or closes:
                // SetResultAndClose(false); // This might be too early if called from DialogService
                // The base.InitializeAsync() already sets the exception on TaskCompletionSource
                await base.InitializeAsync(parameter); // Ensure base completes, even with exception for the TCS
                throw; // Re-throw so DialogService can catch it if needed
            }
            finally
            {
                IsLoading = false;
                ValidateAllProperties(); // From ObservableValidator
                SavePartCommand.NotifyCanExecuteChanged();
            }
        }

        #endregion

        #region Data Loading

        private async Task LoadAvailableCategoriesAsync()
        {
            IsLoadingCategories = true; // If you have granular loading flags

            try
            {
                var categories = await _categoryService.GetAllCategoriesForSelectionAsync(true);
                AvailableCategories.Clear();

                var nullableItemStr = System.Windows.Application.Current.Resources["SelectACategoryStr"] as string ?? "-- Select a Category --";
                AvailableCategories.Add(new CategorySelectionDto(0, nullableItemStr, null));
                foreach (var cat in categories)
                {
                    AvailableCategories.Add(cat);
                }
            }
            catch (Exception ex) { }
            finally
            {
                IsLoadingCategories = false;
            }
        }

        private async Task LoadAvailableUnitOfMeasuresAsync()
        {
            IsLoadingUoMs = true;
            try
            {
                var uoms = await _unitOfMeasureService.GetAllUnitOfMeasuresAsync();
                AvailableUnitOfMeasures.Clear();
                AvailableSalesUnitOfMeasures.Clear();
                AvailablePurchaseUnitOfMeasures.Clear();


                var nullableItemStr = System.Windows.Application.Current.Resources["SelectUnitOfMeasureStr"] as string ?? "-- Select UoM --";
                AvailableUnitOfMeasures.Add(new UnitOfMeasureDto(0, nullableItemStr, string.Empty, string.Empty));

                var sameAsStockItemStr = System.Windows.Application.Current.Resources["SameAsStockUOMStr"] as string ?? "Same as Stock Unit of Measure";
                AvailableSalesUnitOfMeasures.Add(new UnitOfMeasureDto(0, sameAsStockItemStr, string.Empty, string.Empty));
                AvailablePurchaseUnitOfMeasures.Add(new UnitOfMeasureDto(0, sameAsStockItemStr, string.Empty, string.Empty));

                foreach (var uom in uoms)
                {
                    AvailableUnitOfMeasures.Add(uom);
                    AvailableSalesUnitOfMeasures.Add(uom);
                    AvailablePurchaseUnitOfMeasures.Add(uom);
                }

            }
            finally
            {
                IsLoadingUoMs = false;
            }
        }

        private async Task LoadCompatibilitiesForPartAsync()
        {
            // Only load if in Edit mode and we have a part ID
            if (!IsEditing || PartToEditOriginalData?.Id <= 0)
            {
                IsCompatibilityTabContentLoaded = true; // Mark as "loaded" since there's nothing to load
                return;
            }

            _logger.LogInformation("Loading compatibility rules for Part ID: {PartId}", PartToEditOriginalData.Id);
            IsLoadingCompatibilities = true;
            try
            {
                var rules = await _partCompatibilityRuleService.GetRulesForPartAsync(PartToEditOriginalData.Id);
                CurrentPartCompatibilities.Clear();
                foreach (var rule in rules)
                {
                    CurrentPartCompatibilities.Add(new PartCompatibilityRuleViewModelItem(rule));
                }
                _logger.LogInformation("Loaded {Count} compatibility rules.", CurrentPartCompatibilities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load compatibility rules for Part ID: {PartId}", PartToEditOriginalData.Id);
                //_wpfToastNotificationService.ShowError(, "Error");
                await MessageBoxHelper.ShowMessageWithoutTitleAsync("Could not load compatibility rules.", true, CurrentWorkFlow);
            }
            finally
            {
                IsLoadingCompatibilities = false;
                IsCompatibilityTabContentLoaded = true;
            }
        }

        #endregion

        #region Property Customization

        partial void OnCurrentTabIndexChanged(int value)
        {
            // Synchronize the CurrentStep with the new tab index
            switch (value)
            {
                case 0:
                    CurrentStep = PartStepsWizard.CoreInfo;
                    break;
                case 1:
                    CurrentStep = PartStepsWizard.ImagesAndNotes;
                    break;
                case 2:
                    CurrentStep = PartStepsWizard.PricingAndStock;
                    break;
                case 3:
                    CurrentStep = PartStepsWizard.Suppliers;
                    break;
                case 4:
                    CurrentStep = PartStepsWizard.CompatibleVehicles;
                    break;
                default:
                    // Default to first step if index is out of bounds
                    CurrentStep = PartStepsWizard.CoreInfo;
                    break;
            }

            // Update the progress bar to match the new step
            ProgressBarValue = value + 1; // Or however you calculate it

            // Now, notify all commands and properties that depend on CurrentStep
            GoToNextStepCommand.NotifyCanExecuteChanged();
            GoToPreviousStepCommand.NotifyCanExecuteChanged();
            SavePartCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(BackButtonVisibility));
            OnPropertyChanged(nameof(CancelButtonVisibility));
            OnPropertyChanged(nameof(NextButtonVisibility));
            OnPropertyChanged(nameof(SaveButtonVisibility));
            OnPropertyChanged(nameof(IsImageAndNotesTabEnabled));
            OnPropertyChanged(nameof(IsPricingAndStockTabEnabled));
            OnPropertyChanged(nameof(IsSupplierTabEnabled));
            OnPropertyChanged(nameof(IsVehicleCompatibilityTabEnabled));
        }

        partial void OnSelectedStockingUnitOfMeasureChanged(UnitOfMeasureDto? value)
        {
            UpdateUoMDifferenceFlagsAndFactors();
        }
        partial void OnSelectedSalesUnitOfMeasureChanged(UnitOfMeasureDto? value)
        {
            UpdateUoMDifferenceFlagsAndFactors();
        }
        partial void OnSelectedPurchaseUnitOfMeasureChanged(UnitOfMeasureDto? value)
        {
            UpdateUoMDifferenceFlagsAndFactors();
        }

        //partial void OnPartNumberChanged(string value)
        //{
        //    _ = CheckPartNumberExistenceAsync();
        //}

        #endregion

        #region Top Commands

        private bool CanSavePart()
        {

            // This will eventually check if ALL required fields across ALL tabs are valid
            // For now, focusing on Core Info for next step enablement, final save checks all.
            if (IsEditing) // Edit mode
            {
                // Check if any property relevant to the DTO has changed from PartToEditOriginalData
                // AND all required fields are valid
                ValidateAllProperties(); // If using ObservableValidator
                return !HasErrors; // Simplistic, assumes all VM props map to DTO & are validated
            }
            else // Add mode
            {
                // Ensure all required fields for CreatePartDto are filled and valid
                // This re-checks conditions similar to CanGoToNextStep for the *final* step
                if (CurrentStep != PartStepsWizard.CompatibleVehicles) return false; // Or whatever your final step is

                // ValidateAllProperties(); // If using ObservableValidator
                // return !HasErrors &&
                return !string.IsNullOrWhiteSpace(PartNumber) &&
                       !string.IsNullOrWhiteSpace(Name) &&
                       SelectedCategory != null && SelectedCategory.Id > 0 &&
                       // CostPrice >= 0 && SellingPrice >= 0 && // From Pricing Tab
                       // SelectedStockingUnitOfMeasure != null && SelectedStockingUnitOfMeasure.Id > 0 && // From Pricing Tab
                       // etc. for all required fields
                       !IsSaving;
            }

            //if (CurrentStep == PartStepsWizard.CompatibleVehicles)
            //{
            //    ValidateAllProperties();

            //    if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(PartNumber) || (SelectedCategory == null || SelectedCategory.Id <= 0)) return false;

            //    if (CostPrice <= 0 || SellingPrice <= 0 || (SelectedStockingUnitOfMeasure == null || SelectedStockingUnitOfMeasure.Id <= 0) ||
            //        (IsSalesUoMDifferent && (SelectedSalesUnitOfMeasure == null || SelectedSalesUnitOfMeasure.Id <= 0)) ||
            //        (IsPurchaseUoMDifferent && (SelectedPurchaseUnitOfMeasure == null || SelectedPurchaseUnitOfMeasure.Id <= 0))) 
            //        return false;

            //    if (SelectedPartSupplierLink == null || SelectedPartSupplierLink.Id <= 0) return false;
            //    if (SelectedPartCompatibility == null || SelectedPartCompatibility.Id <= 0) return false;

            //    return true;
            //}
            //return false;
        }

        [RelayCommand(CanExecute = nameof(CanSavePart))]
        private async Task SavePartAsync()
        {
            try
            {
                // Perform final validation for all properties that will go into the DTO
                // If using ObservableValidator:
                // ValidateAllProperties();
                // if (HasErrors)
                // {
                //     await MessageBoxHelper.ShowMessageWithoutTitleAsync("Please correct all validation errors before saving.", true, CurrentWorkFlow);
                //     return;
                // }

                // More explicit checks again before constructing DTO (belt and suspenders)
                if (string.IsNullOrWhiteSpace(PartNumber) || string.IsNullOrWhiteSpace(Name) || SelectedCategory == null || SelectedCategory.Id == 0 /* || other required field checks */)
                {
                    await MessageBoxHelper.ShowMessageWithoutTitleAsync("Please ensure all required fields are filled correctly.", true, CurrentWorkFlow);
                    return;
                }


                if (CostPrice <= 0 || SellingPrice <= 0 || (SelectedStockingUnitOfMeasure == null || SelectedStockingUnitOfMeasure.Id == 0))
                {
                    await MessageBoxHelper.ShowMessageWithoutTitleAsync("Please ensure all required fields are filled correctly.", true, CurrentWorkFlow);
                    return;
                }

                IsSaving = true;
                PartDetailDto? savedPartDetails = null;

                try
                {
                    if (IsEditing && PartToEditOriginalData != null)
                    {
                        var updateDto = new UpdatePartDto(
                            PartToEditOriginalData.Id, // Use original ID
                            PartNumber,
                            Name,
                            Description,
                            Manufacturer,
                            CostPrice, // From Pricing tab
                            SellingPrice, // From Pricing tab
                            PartToEditOriginalData.StockQuantity, // StockQuantity (base) might not be editable here, or it is. CurrentStock is by transactions.
                                                                  // If StockQuantity IS editable, bind it like other fields.
                            ReorderLevel, // From Pricing tab
                            MinimumStock, // From Pricing tab
                            PartLocation,
                            IsActive,
                            IsOriginal,
                            Notes,
                            Barcode,
                            SelectedCategory.Id,
                            StockingUnitOfMeasureId = SelectedStockingUnitOfMeasure?.Id ?? 0, // Should not be 0 if validated
                            SalesUnitOfMeasureId = (IsSalesUoMDifferent && SelectedSalesUnitOfMeasure != null && SelectedSalesUnitOfMeasure.Id != 0)
                                                   ? SelectedSalesUnitOfMeasure.Id
                                                   : null, // Explicitly null if same or not chosen
                            SalesConversionFactor = IsSalesUoMDifferent ? SalesConversionFactor : null,

                            PurchaseUnitOfMeasureId = (IsPurchaseUoMDifferent && SelectedPurchaseUnitOfMeasure != null && SelectedPurchaseUnitOfMeasure.Id != 0)
                                                      ? SelectedPurchaseUnitOfMeasure.Id
                                                      : null,
                            PurchaseConversionFactor = IsPurchaseUoMDifferent ? PurchaseConversionFactor : null
                        );
                        await _partService.UpdatePartAsync(updateDto);
                        await MessageBoxHelper.ShowMessageWithoutTitleAsync("Part updated successfully!", false, CurrentWorkFlow);

                        savedPartDetails = await _partService.GetPartDetailsByIdAsync(PartToEditOriginalData.Id);

                    }
                    else // Adding new part
                    {
                        var createDto = new CreatePartDto(
                            PartNumber,
                            Name,
                            Description,
                            Manufacturer,
                            CostPrice,
                            SellingPrice,
                            InitialStockQuantity, // From Pricing tab
                            ReorderLevel,
                            MinimumStock,
                            PartLocation,
                            IsActive,
                            IsOriginal,
                            Notes,
                            Barcode,
                            SelectedCategory.Id,
                            StockingUnitOfMeasureId = SelectedStockingUnitOfMeasure?.Id ?? 0, // Should not be 0 if validated
                            SalesUnitOfMeasureId = (IsSalesUoMDifferent && SelectedSalesUnitOfMeasure != null && SelectedSalesUnitOfMeasure.Id != 0)
                                                   ? SelectedSalesUnitOfMeasure.Id
                                                   : null, // Explicitly null if same or not chosen
                            SalesConversionFactor = IsSalesUoMDifferent ? SalesConversionFactor : null,

                            PurchaseUnitOfMeasureId = (IsPurchaseUoMDifferent && SelectedPurchaseUnitOfMeasure != null && SelectedPurchaseUnitOfMeasure.Id != 0)
                                                      ? SelectedPurchaseUnitOfMeasure.Id
                                                      : null,
                            PurchaseConversionFactor = IsPurchaseUoMDifferent ? PurchaseConversionFactor : null
                        // InitialSuppliers and InitialCompatibleVehicles are null for now
                        );
                        savedPartDetails = await _partService.CreatePartAsync(createDto);
                        await MessageBoxHelper.ShowMessageWithoutTitleAsync("Part created successfully!", false, CurrentWorkFlow);


                    }

                    if (savedPartDetails == null)
                    {
                        throw new ServiceException("Failed to save or retrieve part details after core save.");
                    }

                    int currentPartId = savedPartDetails.Id;


                    // ---- STAGE 2: Process Images ----
                    // Delete images marked for removal
                    foreach (var imageItemToRemove in _imagesMarkedForDeletion.ToList()) // Iterate copy
                    {
                        if (!imageItemToRemove.IsNew) // Only existing images need service call to delete
                        {
                            await _partService.RemoveImageFromPartAsync(imageItemToRemove.ExistingPartImageId);
                        }
                        _imagesMarkedForDeletion.Remove(imageItemToRemove); // Remove from tracking list
                    }

                    // Add new images and Update existing modified images
                    foreach (var imageItem in PartImagesToDisplay)
                    {
                        if (imageItem.State == ImageItemState.Added && imageItem.SourceClientPath != null)
                        {
                            var createImageDto = new CreatePartImageDto(
                                imageItem.SourceClientPath, // Pass the local source path
                                imageItem.Caption,
                                imageItem.IsPrimary,
                                imageItem.DisplayOrder
                            );
                            // AddImageToPartAsync in PartService expects the local path
                            await _partService.AddImageToPartAsync(currentPartId, createImageDto);
                        }
                        else if (imageItem.State == ImageItemState.Modified && !imageItem.IsNew)
                        {
                            var updateImageDto = new UpdatePartImageDto(
                                imageItem.ExistingPartImageId,
                                imageItem.Caption,
                                imageItem.IsPrimary,
                                imageItem.DisplayOrder,
                                imageItem.PersistentImagePath
                            );
                            await _partService.UpdatePartImageDetailsAsync(updateImageDto);
                        }
                    }

                    // ---- STAGE 3: Process Suppliers (if changes were made in memory) ----
                    // Similar logic: iterate through a list of supplier changes, call service.

                    // ---- STAGE 4: Process Compatibility Rules (if changes were made in memory) ----
                    // Similar logic.
                    _logger.LogInformation("Linking {Count} staged compatibility rules to new Part ID: {PartId}",
                        _stagedRulesForNewPart.Count, savedPartDetails.Id);

                    foreach (var ruleToCreate in _stagedRulesForNewPart)
                    {
                        try
                        {
                            await _partCompatibilityRuleService.CreateRuleForPartAsync(savedPartDetails.Id, ruleToCreate);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to create staged rule '{RuleName}' for new Part ID {PartId}",
                                ruleToCreate.Name, savedPartDetails.Id);
                            // Notify user that one of the staged rules failed to save
                            await MessageBoxHelper.ShowMessageWithTitleAsync($"Failed to save rule: {ruleToCreate.Name}", "", "Rule Error", true, CurrentWorkFlow);
                        }
                    }

                    _stagedRulesForNewPart.Clear();


                    SetResultAndClose(true); // Signal overall success
                }
                catch
                {

                }
                finally
                {
                    IsSaving = false;
                }
            }
            catch (ValidationException vex)
            {
                _logger.LogError(vex, "Validation error while saving part.");
                // Display validation errors (e.g., from vex.Errors)
                await MessageBoxHelper.ShowMessageWithTitleAsync("Validation Error", "Please correct the following errors:", vex.Message, true, CurrentWorkFlow);
            }
            catch (DuplicationException dex)
            {
                _logger.LogError(dex, "Duplication error while saving part: {DuplicateValue}", dex.Message);
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Duplicate Entry", dex.Message, true, CurrentWorkFlow);
            }
            catch (ServiceException sex)
            {
                _logger.LogError(sex, "Service error while saving part.");
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Service Error", sex.Message, true, CurrentWorkFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving part.");
                await MessageBoxHelper.ShowMessageWithTitleAsync("Error", "Unexpected Error", "An unexpected error occurred. Please try again.", true, CurrentWorkFlow);
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

        private bool CanGoToNextStep()
        {
            if (CurrentStep == PartStepsWizard.CoreInfo)
            {

                bool isStepCompleted = false;
                if (!string.IsNullOrWhiteSpace(PartNumber) &&
                       !string.IsNullOrWhiteSpace(Name) &&
                       SelectedCategory != null && SelectedCategory.Id > 0) isStepCompleted = true; // Assuming ID 0 is a placeholder


                IsCoreInfoStepFinished = isStepCompleted;

                return isStepCompleted;
            }

            if (CurrentStep == PartStepsWizard.ImagesAndNotes)
            {
                IsImageAndNotesStepFinished = true;
                return true;
            }

            if (CurrentStep == PartStepsWizard.PricingAndStock)
            {
                // Basic checks. More detailed validation is done by FluentValidators on Save.
                bool isValid = CostPrice > 0 &&
                               SellingPrice > 0 &&
                               (IsEditing || InitialStockQuantity >= 0) && // InitialStock only relevant for Add mode
                               SelectedStockingUnitOfMeasure != null && SelectedStockingUnitOfMeasure.Id > 0;

                if (IsSalesUoMDifferent)
                {
                    isValid &= SalesConversionFactor.HasValue && SalesConversionFactor.Value > 0;
                }
                if (IsPurchaseUoMDifferent)
                {
                    isValid &= PurchaseConversionFactor.HasValue && PurchaseConversionFactor.Value > 0;
                }

                // Update step finished flag
                IsPricingAndStockStepFinished = isValid;
                return isValid;
            }

            if (CurrentStep == PartStepsWizard.Suppliers)
            {
                return true;
            }

            if (CurrentStep == PartStepsWizard.CompatibleVehicles)
            {
                return true;
            }


            return false; // Default for steps without explicit next-step validation
        }

        [RelayCommand(CanExecute = nameof(CanGoToNextStep))]
        public async Task GoToNextStepAsync()
        {
            try
            {
                bool canProceed = true;
                if (CurrentStep == PartStepsWizard.CoreInfo)
                {

                    // Or manual checks if not using ObservableValidator for immediate feedback:
                    if (string.IsNullOrWhiteSpace(PartNumber) ||
                        string.IsNullOrWhiteSpace(Name) ||
                        SelectedCategory == null || SelectedCategory.Id == 0) // Assuming ID 0 is placeholder
                    {
                        await MessageBoxHelper.ShowMessageWithoutTitleAsync("Part Number, Name, and Category are required.", true, CurrentWorkFlow);
                        canProceed = false;
                    }
                    // Add async uniqueness check for PartNumber here if not done on blur
                    else if (!IsEditing && await _partService.PartNumberExistsAsync(PartNumber))
                    {
                        await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Part Number '{PartNumber}' already exists.", true, CurrentWorkFlow);
                        canProceed = false;
                    }
                    // Add similar for Barcode if it should be unique and is on this tab


                    if (canProceed)
                    {
                        CurrentStep = PartStepsWizard.ImagesAndNotes;
                        CurrentTabIndex = 1;
                        ProgressBarValue = 2;
                        IsCoreInfoStepFinished = true; // Mark as finished
                    }
                    else
                    {
                        return;
                    }
                }
                else if (CurrentStep == PartStepsWizard.ImagesAndNotes)
                {
                    CurrentStep = PartStepsWizard.PricingAndStock;
                    CurrentTabIndex = 2;
                    ProgressBarValue = 3;
                    IsImageAndNotesStepFinished = true; // Mark as finished
                }
                else if (CurrentStep == PartStepsWizard.PricingAndStock)
                {
                    // Validate this step's fields
                    // This re-evaluates CanGoToNextStep essentially, or you can be more explicit
                    bool stepValid = CostPrice >= 0 &&
                                     SellingPrice >= 0 &&
                                     (IsEditing || InitialStockQuantity >= 0) &&
                                     SelectedStockingUnitOfMeasure != null && SelectedStockingUnitOfMeasure.Id > 0;

                    if (IsSalesUoMDifferent && (!SalesConversionFactor.HasValue || SalesConversionFactor.Value <= 0))
                    {
                        await MessageBoxHelper.ShowMessageWithoutTitleAsync("Sales Conversion Factor is required and must be positive when Sales UoM differs from Stocking UoM.", true, CurrentWorkFlow);
                        canProceed = false; // Assuming canProceed is defined at the start of GoToNextStepAsync
                    }
                    if (IsPurchaseUoMDifferent && (!PurchaseConversionFactor.HasValue || PurchaseConversionFactor.Value <= 0))
                    {
                        await MessageBoxHelper.ShowMessageWithoutTitleAsync("Purchase Conversion Factor is required and must be positive when Purchase UoM differs from Stocking UoM.", true, CurrentWorkFlow);
                        canProceed = false;
                    }
                    if (!stepValid && canProceed) // If basic validation fails and specific factor validation didn't already set canProceed to false
                    {
                        await MessageBoxHelper.ShowMessageWithoutTitleAsync("Please ensure all pricing, stock, and unit of measure fields are correctly filled.", true, CurrentWorkFlow);
                        canProceed = false;
                    }

                    if (canProceed)
                    {
                        CurrentStep = PartStepsWizard.Suppliers;
                        CurrentTabIndex = 3;
                        ProgressBarValue = 4;
                        IsPricingAndStockStepFinished = true; // Mark as finished
                    }
                    else
                    {
                        return;
                    }
                }
                else if (CurrentStep == PartStepsWizard.Suppliers)
                {
                    CurrentStep = PartStepsWizard.CompatibleVehicles;
                    CurrentTabIndex = 4;
                    ProgressBarValue = 5;
                    IsSuppliersStepFinished = true;
                }

                if (!canProceed) return;

                GoToNextStepCommand.NotifyCanExecuteChanged();
                GoToPreviousStepCommand.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [RelayCommand]
        private void GoToPreviousStep()
        {
            if (CurrentStep == PartStepsWizard.CompatibleVehicles)
            {
                SavePartCommand.NotifyCanExecuteChanged(); // Finish button visibility depends on step

                CurrentStep = PartStepsWizard.Suppliers;
                CurrentTabIndex = 3;
                ProgressBarValue = 4;
            }

            else if (CurrentStep == PartStepsWizard.Suppliers)
            {
                CurrentStep = PartStepsWizard.PricingAndStock;
                CurrentTabIndex = 2;
                ProgressBarValue = 3;
            }

            else if (CurrentStep == PartStepsWizard.PricingAndStock)
            {
                CurrentStep = PartStepsWizard.ImagesAndNotes;
                CurrentTabIndex = 1;
                ProgressBarValue = 2;
            }

            else if (CurrentStep == PartStepsWizard.ImagesAndNotes)
            {
                CurrentStep = PartStepsWizard.CoreInfo;
                CurrentTabIndex = 0;
                ProgressBarValue = 1;
            }

            // Notify commands that depend on CurrentStep
            GoToNextStepCommand.NotifyCanExecuteChanged();
            GoToPreviousStepCommand.NotifyCanExecuteChanged();
            SavePartCommand.NotifyCanExecuteChanged();


        }

        #endregion

        #region Supplier Link Commands

        [RelayCommand]
        private async Task AddSupplierLinkAsync()
        {
            try
            {
                IsAddingSupplier = true;

                int currentPartId = 0;

                var newSupplierPartLink = await _dialogService.ShowDialogWithResultsAsync<LinkNewSupplierDialogViewModel, LinkNewSupplierDialog, PartSupplierCreateDto>(currentPartId, _dialog.CurrentDialogInstance);

            }
            catch
            {

            }
            finally
            {
                IsAddingSupplier = false;
            }
        }

        [RelayCommand]
        private async Task EditSupplierLinkAsync()
        {
            try
            {
                IsEditingSupplier = true;


                //var editedSupplierPartLink = await _dialogService.ShowDialogWithResultsAsync<EditSupplierLinkDialogViewModel, EditSupplierLinkDialog, PartSupplierDto>(PartSupplierLink);

                //try
                //{
                //    if (editedSupplierPartLink != null)
                //    {
                //        //await _partService.UpdateSupplierLinkForPartAsync(editedSupplierPartLink);
                //    }



                //}catch(Exception ex)
                //{
                //    throw;
                //}

            }
            catch
            {

            }
            finally
            {
                IsEditingSupplier = false;
            }
        }

        [RelayCommand]
        private async Task RemoveSupplierLinkAsync()
        {
            try
            {
                IsRemovingSupplier = true;

                SetResultAndClose(true);
            }
            catch
            {

            }
            finally
            {
                IsRemovingSupplier = false;
            }
        }

        #endregion

        #region Part Compatibility Commands

        [RelayCommand]
        private async Task ShowLinkVehicleSpecDialogAsync()
        {
            if (IsEditing && (PartToEditOriginalData?.Id ?? 0) <= 0)
            {
                await MessageBoxHelper.ShowMessageWithoutTitleAsync("Cannot add rule: Part ID is invalid.", true, CurrentWorkFlow);
                return;
            }

            IsAddingPartCompatibility = true;

            try
            {
                // Open the dialog to get ONE rule definition
                var ruleToCreateDto = await _dialogService.ShowDialogWithResultsAsync<DefineVehicleSpecificationDialogViewModel, DefineVehicleSpecificationDialog, CreatePartCompatibilityRuleDto>(null, _dialog.CurrentDialogInstance);

                if (ruleToCreateDto != null)
                {
                    if (IsEditing) // EDIT MODE: Save to DB immediately
                    {
                        _logger.LogInformation("Creating and linking new rule '{RuleName}' to existing Part ID: {PartId}", ruleToCreateDto.Name, PartToEditOriginalData.Id);
                        await _partCompatibilityRuleService.CreateRuleForPartAsync(PartToEditOriginalData.Id, ruleToCreateDto);
                        await LoadCompatibilitiesForPartAsync(); // Refresh the grid from DB
                        //_wpfToastNotificationService.ShowSuccess("Compatibility rule added successfully!");

                        await MessageBoxHelper.ShowMessageWithoutTitleAsync("Compatibility rule added successfully!", false, CurrentWorkFlow);

                    }
                    else
                    {
                        // ADD MODE: Stage it
                        _stagedRulesForNewPart.Add(ruleToCreateDto);
                        // Add to UI collection by creating a new ViewModelItem wrapper
                        // Note: We need IVehicleTaxonomyService to build display strings
                        CurrentPartCompatibilities.Add(new PartCompatibilityRuleViewModelItem(ruleToCreateDto, _vehicleTaxonomyService));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while showing or processing the Define Vehicle Specification dialog.");
                // _wpfToastNotificationService.ShowError("An unexpected error occurred while adding compatibility.", "Error");
                await MessageBoxHelper.ShowMessageWithoutTitleAsync($"An error occurred while showing or processing the Define Vehicle Specification dialog. {ex.Message}", true, CurrentWorkFlow);
            }
            finally
            {
                IsAddingPartCompatibility = false;
            }
        }

        private bool CanRemoveCompatibilityRule() => SelectedPartCompatibilityRule != null && !IsRemovingCompatibility;

        [RelayCommand(CanExecute = nameof(CanRemoveCompatibilityRule))]
        private async Task RemoveCompatibilityRuleAsync()
        {
            if (SelectedPartCompatibilityRule == null) return; // Should be blocked by CanExecute, but defensive check

            var ruleToDelete = SelectedPartCompatibilityRule; // Copy reference before nulling selection
            if (ruleToDelete.IsNew)
            {
                // It's a pending rule, just remove it from our temporary lists
                _logger.LogInformation("Removed staged compatibility rule: {RuleName}", ruleToDelete.Name);
                _stagedRulesForNewPart.Remove(ruleToDelete.NewRuleData!);
                CurrentPartCompatibilities.Remove(ruleToDelete);
            }
            else
            {
                var confirmResult = _dialogService.ShowConfirmDeleteItemsDialog(1);
                if (!confirmResult.HasValue || !confirmResult.Value) return;

                IsRemovingCompatibility = true;


                try
                {
                    _logger.LogInformation("Attempting to delete PartCompatibilityRule ID: {RuleId}", ruleToDelete.ExistingRule!.Id);

                    await _partCompatibilityRuleService.DeleteRuleAsync(ruleToDelete.ExistingRule!.Id);
                    CurrentPartCompatibilities.Remove(ruleToDelete);


                    //_wpfToastNotificationService.ShowSuccess($"Rule '{ruleToDelete.Name}' was removed successfully.");
                    await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Rule '{ruleToDelete.Name}' was removed successfully.", false, CurrentWorkFlow);

                    // Remove from the local collection for immediate UI feedback
                    CurrentPartCompatibilities.Remove(ruleToDelete);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete PartCompatibilityRule ID: {RuleId}", ruleToDelete.ExistingRule.Id);
                    await MessageBoxHelper.ShowMessageWithoutTitleAsync($"Could not remove rule '{ruleToDelete.Name}': {ex.Message}", true, CurrentWorkFlow);

                    //_wpfToastNotificationService.ShowError($"Could not remove rule '{ruleToDelete.Name}': {ex.Message}", "Delete Error");
                }
                finally
                {
                    IsRemovingCompatibility = false;
                }
            }

        }

        #endregion

        #region General Methods

        private void UpdateUoMDifferenceFlagsAndFactors()
        {
            // The [NotifyPropertyChangedFor] attributes on Selected...UoM properties will trigger updates
            // for IsSalesUoMDifferent and IsPurchaseUoMDifferent automatically when those selections change.
            // So, we just need to handle the factor nullification.

            if (!IsSalesUoMDifferent) // This uses the up-to-date calculated property
            {
                SalesConversionFactor = null;
            }
            if (!IsPurchaseUoMDifferent) // This uses the up-to-date calculated property
            {
                PurchaseConversionFactor = null;
            }
            // Notify CanExecute for commands that depend on the validity of these factors
            GoToNextStepCommand.NotifyCanExecuteChanged();
            SavePartCommand.NotifyCanExecuteChanged();
        }

        private async Task CheckPartNumberExistenceAsync()
        {
            try
            {
                IsCheckingName = true;

                bool isPartNumValid = await _partService.PartNumberExistsAsync(PartNumber);
                // Show success mark or cross mark
            }
            finally
            {
                IsCheckingName = false;
            }
        }

        #endregion

        #region Image Handling Commands

        [RelayCommand]
        private async Task LoadImageAsync() // Renamed for clarity, was LoadImage
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                    Title = "Select Part Image(s)",
                    Multiselect = true,
                };

                IsLoadingImages = true; // Assuming you have this flag


                if (dialog.ShowDialog() == true)
                {
                    await Task.Delay(300); // Allow UI to update with spinner

                    foreach (string filePath in dialog.FileNames)
                    {
                        // Check if this local file path is already added (to prevent duplicates in the current session)
                        if (!PartImagesToDisplay.Any(pivm => pivm.IsNew && pivm.SourceClientPath == filePath))
                        {
                            var newImageItem = new PartImageViewModelItem(filePath, this)
                            {
                                // Set initial DisplayOrder based on current count or a more sophisticated logic
                                DisplayOrder = PartImagesToDisplay.Count
                            };
                            PartImagesToDisplay.Add(newImageItem);
                        }
                    }

                    OnPropertyChanged(nameof(HasAnyImage));
                    RemoveImageCommand.NotifyCanExecuteChanged(); // If CanExecute depends on items existing
                }
            }
            finally
            {
                IsLoadingImages = false;
            }
         
        }
        private bool CanRemoveImage(PartImageViewModelItem? imageItem) => imageItem != null;
        private List<PartImageViewModelItem> _imagesMarkedForDeletion = new(); // Add this field

        [RelayCommand(CanExecute = nameof(CanRemoveImage))] // CanExecute if parameter is not null
        private void RemoveImage(PartImageViewModelItem? imageItem)
        {
            if (imageItem == null) return;

            if (imageItem.IsNew)
            {
                PartImagesToDisplay.Remove(imageItem); // Simply remove from UI list
            }
            else // It's an existing image
            {
                // Mark it for deletion. Actual deletion from DB/FileService happens during SavePartAsync.
                // Or, if you want immediate deletion for existing images:
                // _ = DeleteExistingImageAsync(imageItem); // Call an async method to delete via service
                // For now, let's mark it and handle in SavePartAsync for atomicity with other part changes.
                imageItem.State = ImageItemState.Removed;
                // You might want to visually indicate it's marked for removal (e.g., strikethrough, opacity)
                // or remove it from display and keep it in a separate _imagesToDelete list.
                // For simplicity, let's remove from display and add to a separate list for processing on save.
                PartImagesToDisplay.Remove(imageItem);
                _imagesMarkedForDeletion.Add(imageItem); // Add a private List<PartImageViewModelItem> _imagesMarkedForDeletion;
            }
            RemoveImageCommand.NotifyCanExecuteChanged(); // Update CanExecute
            OnPropertyChanged(nameof(HasAnyImage));
        }




        // In AddEditPartDialogViewModel:
        public void HandlePrimaryImageSet(PartImageViewModelItem justSetAsPrimary)
        {
            foreach (var imgItem in PartImagesToDisplay)
            {
                if (imgItem != justSetAsPrimary && imgItem.IsPrimary)
                {
                    imgItem.IsPrimary = false; // This will trigger its OnIsPrimaryChanged
                }
            }
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
                _logger.LogError("The Dialog window was null on the {Dialog}", nameof(AddEditPartDialog));
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
