using AutoFusionPro.Application.DTOs.Vehicle;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Services;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Models;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Vehicles
{
    public partial class VehiclesViewModel : BaseViewModel
    {

        private readonly IVehicleService _vehicleService;
        private readonly ILocalizationService _localizationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWpfToastNotificationService _toastNotificationService;
        private readonly ILogger<VehiclesViewModel> _logger;
        private readonly IDialogService _dialogService;


        #region General Properties

        [ObservableProperty]
        private bool _isLoading = false;


        [ObservableProperty]
        private bool _hasError = false;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private ObservableCollection<VehicleSummaryDto> _vehicles = new();

        [ObservableProperty]
        private VehicleDetailDto? _selectedVehicle;

        #endregion

        #region Filter Criteria 

        [ObservableProperty]
        private string? _searchText;

        [ObservableProperty]
        private string? _selectedMake; // For filter dropdown

        [ObservableProperty]
        private string? _selectedModel; // For filter dropdown

        [ObservableProperty]
        private int? _selectedMinYear; // For filter input

        [ObservableProperty]
        private int? _selectedMaxYear; // For filter input


        [ObservableProperty]
        private string _searchQuery;

        [ObservableProperty]
        private bool _enableClearSearchButton;

        // --- Filter Options (Populated from Service) ---
        [ObservableProperty]
        private List<string> _availableMakes = new();

        [ObservableProperty]
        private List<string> _availableModels = new();

        [ObservableProperty]
        private List<int> _availableYears = new();

        #endregion

        #region Pagination

        // --- Pagination ---
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
        private int _currentPage = 1;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
        private int _totalPages = 1;

        [ObservableProperty]
        private int _totalItems = 0;

        [ObservableProperty]
        private int _pageSize = 20; // Or get from settings




        #endregion
        public VehiclesViewModel(IVehicleService vehicleService,
            IWpfToastNotificationService wpfToastNotificationService,
        ILocalizationService localizationService,
        IServiceProvider serviceProvider,
        ILogger<VehiclesViewModel> logger, IDialogService dialogService)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _toastNotificationService = wpfToastNotificationService ?? throw new ArgumentNullException(nameof(wpfToastNotificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
            _localizationService.FlowDirectionChanged += OnCurrentFlowDirectionChanged;

            RegisterCleanup(() => _localizationService.FlowDirectionChanged -= OnCurrentFlowDirectionChanged);


            // Initial load on construction or via an explicit Initialize command
            _ = InitializeViewModelAsync();
        }


        #region Initialization

        private async Task InitializeViewModelAsync()
        {
            await LoadFilterOptionsAsync(); // Load dropdowns first
            await LoadVehiclesAsync(); // Load initial data
        }

        // Placeholder for filter dropdown loading
        private async Task LoadFilterOptionsAsync()
        {
            IsLoading = true; // Show loading indicator for options
            try
            {
                AvailableMakes = (await _vehicleService.GetDistinctMakesAsync()).ToList();
                // AvailableModels could be loaded dynamically based on SelectedMake
                AvailableYears = (await _vehicleService.GetDistinctYearsAsync()).ToList();
                _logger.LogInformation("Loaded distinct filter options (Makes, Years).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load filter options.");
                _toastNotificationService.ShowError("Could not load filter options."); // Inform user
            }
            finally
            {
                IsLoading = false;
            }
        }


        #endregion


        #region Commands

        // --- LoadVehiclesAsync Implementation ---
        // Command definition (can be triggered by Refresh button, filter changes, pagination)
        [RelayCommand]
        private async Task LoadVehiclesAsync()
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;
            _logger.LogInformation("Loading vehicles for Page: {CurrentPage}, PageSize: {PageSize}", CurrentPage, PageSize);

            try
            {
                // 1. Prepare Filter Criteria DTO
                var filterCriteria = new VehicleFilterCriteriaDto(
                    SearchTerm: SearchText, // Assuming SearchText covers multiple fields
                    Make: SelectedMake,
                    Model: SelectedModel, // Ensure this is updated when Make changes
                    MinYear: SelectedMinYear,
                    MaxYear: SelectedMaxYear
                );

                // 2. Call Service
                PagedResult<VehicleSummaryDto> result = await _vehicleService.GetFilteredVehiclesAsync(
                    filterCriteria,
                    CurrentPage,
                    PageSize
                );

                // 3. Process Result
                if (result != null)
                {
                    // Update ObservableCollection efficiently
                    Vehicles.Clear();
                    if (result.Items != null)
                    {
                        foreach (var vehicleDto in result.Items)
                        {
                            Vehicles.Add(vehicleDto);
                        }
                    }

                    // Update Pagination Properties
                    CurrentPage = result.PageNumber; // Ensure it reflects the actual page returned
                    TotalItems = result.TotalCount;
                    PageSize = result.PageSize; // Update if service could change it
                    TotalPages = result.TotalPages; // Use calculated property from PagedResult

                    _logger.LogInformation("Successfully loaded {Count} vehicles. Total items: {TotalItems}, Total pages: {TotalPages}", result.Items.Count(), TotalItems, TotalPages);
                }
                else
                {
                    // Handle null result from service if possible (though service should ideally throw or return empty PagedResult)
                    _logger.LogWarning("Received null PagedResult from VehicleService.");
                    Vehicles.Clear();
                    TotalItems = 0;
                    TotalPages = 1; // Reset pagination
                    CurrentPage = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading vehicles.");
                HasError = true;
                ErrorMessage = "Failed to load vehicles. Please try again."; // User-friendly message
                Vehicles.Clear(); // Clear list on error
                TotalItems = 0;
                TotalPages = 1;
                CurrentPage = 1;
                _toastNotificationService.ShowError(ErrorMessage, "Loading Error"); // Show toast/dialog
                                                                                    // Avoid throwing ViewModelException here unless absolutely necessary for higher-level handling

                throw new ViewModelException($"An exception happened while loading data", nameof(VehiclesViewModel), nameof(LoadVehiclesAsync), "Loading", ex);

            }
            finally
            {
                IsLoading = false;
            }
        }

        // Add CRUD Commands here (AddVehicle, EditVehicle, DeleteVehicle, SaveVehicle, CancelVehicle)
        // These would interact with dialogs/popups and call corresponding _vehicleService methods

        // ... Example Add command
        [RelayCommand]
        private void AddVehicle()
        {
            try
            {
                bool? results = _dialogService.ShowAddVehicleDialog();
                if (results.HasValue && results.Value == true)
                {
                    _toastNotificationService.ShowSuccess("Vehicle Has been added successfully");
                    
                }

            }
            catch (Exception ex)
            {
                _toastNotificationService.ShowError("Failed to add vehicle");
                throw new ViewModelException("An exception happened in AddVehicleCommand", nameof(VehiclesViewModel), nameof(AddVehicleCommand), "Show Dialog", ex);
            }
        }


        [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
        private async Task PreviousPageAsync()
        {
            if (CanGoToPreviousPage())
            {
                CurrentPage--;
                await LoadVehiclesAsync();
            }
        }
        private bool CanGoToPreviousPage() => CurrentPage > 1 && !IsLoading;

        [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
        private async Task NextPageAsync()
        {
            if (CanGoToNextPage())
            {
                CurrentPage++;
                await LoadVehiclesAsync();
            }
        }
        private bool CanGoToNextPage() => CurrentPage < TotalPages && !IsLoading;

        // Command to apply filters (triggered by button or property changes)
        [RelayCommand]
        private async Task ApplyFiltersAsync()
        {
            CurrentPage = 1; // Reset to first page when filters change
            await LoadVehiclesAsync();
        }

        // Example: Handle Make selection changing to update Model list
        partial void OnSelectedMakeChanged(string? value)
        {
            // Asynchronously load models based on the selected make
            _ = LoadModelsForMakeAsync(value);
            // Reset current model selection
            SelectedModel = null;
            // Optionally apply filters immediately or wait for Apply button
            // await ApplyFiltersAsync();
        }

        private async Task LoadModelsForMakeAsync(string? make)
        {
            if (string.IsNullOrWhiteSpace(make))
            {
                AvailableModels = new List<string>(); // Clear models if no make selected
                return;
            }
            // Show loading indicator specifically for models? Optional.
            try
            {
                AvailableModels = (await _vehicleService.GetDistinctModelsAsync(make)).ToList();
                _logger.LogInformation("Loaded {Count} models for Make: {Make}", AvailableModels.Count, make);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load models for Make: {Make}", make);
                AvailableModels = new List<string>(); // Clear on error
                _toastNotificationService.ShowError($"Could not load models for {make}.");
            }
        }



        #endregion



        #region Helpers

        private void OnCurrentFlowDirectionChanged()
        {
            CurrentWorkFlow = _localizationService.CurrentFlowDirection;
        }

        #endregion
    }
}
