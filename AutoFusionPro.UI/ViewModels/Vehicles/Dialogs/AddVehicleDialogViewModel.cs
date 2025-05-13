using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Enums.UI.VehicleDialogs;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.Core.Helpers.ErrorMessages;
using AutoFusionPro.Core.Helpers.Operations;
using AutoFusionPro.UI.Helpers;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Media;

namespace AutoFusionPro.UI.ViewModels.Vehicles.Dialogs
{
    public partial class AddVehicleDialogViewModel : BaseViewModel<AddVehicleDialogViewModel>, IDialogAware
    {
        private IDialogWindow _dialog;
        private readonly IVehicleService _vehicleService;

        #region Properties

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BasicInfoVisibility))]
        [NotifyPropertyChangedFor(nameof(TechnicalDetailsVisibility))]
        [NotifyPropertyChangedFor(nameof(AdditionalInfoVisibility))]
        [NotifyPropertyChangedFor(nameof(BackButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(CancelButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(NextButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(FinishButtonVisibility))]
        [NotifyPropertyChangedFor(nameof(Connector1Style))]
        [NotifyPropertyChangedFor(nameof(Connector2Style))]
        [NotifyPropertyChangedFor(nameof(Step1Style))]
        [NotifyPropertyChangedFor(nameof(Step2Style))]
        [NotifyPropertyChangedFor(nameof(Step3Style))]
        private WizardStep _currentStep = WizardStep.BasicInfo;

        // Step 1: Basic Info
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        [Required(ErrorMessage = "Make is required")]
        private string _make = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Model is required")]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private string _model = string.Empty;

        [ObservableProperty]
        [Range(1900, 2100, ErrorMessage = "Please select a valid year")]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private int _year = DateTime.Now.Year;



        [ObservableProperty]
        private string _vin = string.Empty;

        // Step 2: Technical Details
        [ObservableProperty]
        [Required(ErrorMessage = "Engine is required")]
        [NotifyCanExecuteChangedFor(nameof(GoToNextStepCommand))]
        private string _engine = string.Empty;

        [ObservableProperty]
        private string _transmission = string.Empty;

        // Step 3: Additional Info
        [ObservableProperty]
        private string _trimLevel = string.Empty;

        [ObservableProperty]
        private string _bodyType = string.Empty;

        // Status flags
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LoadingVisibility))]
        private bool _isAddingVehicle = false;

        [ObservableProperty]
        private string _vehicleSummary = string.Empty;

        [ObservableProperty]
        private string _notes = string.Empty;


        #endregion

        #region UI Properties
        public Visibility BasicInfoVisibility => CurrentStep == WizardStep.BasicInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility TechnicalDetailsVisibility => CurrentStep == WizardStep.TechnicalDetails ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdditionalInfoVisibility => CurrentStep == WizardStep.AdditionalInfo ? Visibility.Visible : Visibility.Collapsed;

        // Button Visibility
        public Visibility BackButtonVisibility => CurrentStep != WizardStep.BasicInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CancelButtonVisibility => CurrentStep == WizardStep.BasicInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NextButtonVisibility => CurrentStep != WizardStep.AdditionalInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility FinishButtonVisibility => CurrentStep == WizardStep.AdditionalInfo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LoadingVisibility => IsAddingVehicle ? Visibility.Visible : Visibility.Collapsed;

        // Step Indicator Styling
        public StepStyle Step1Style => GetStepStyle(WizardStep.BasicInfo);
        public StepStyle Step2Style => GetStepStyle(WizardStep.TechnicalDetails);
        public StepStyle Step3Style => GetStepStyle(WizardStep.AdditionalInfo);
        public Brush Connector1Style => CurrentStep == WizardStep.BasicInfo ? System.Windows.Application.Current.Resources["StepIndicator.Default.Background"] as SolidColorBrush ?? Brushes.LightGray : CurrentStep ==  WizardStep.AdditionalInfo ? System.Windows.Application.Current.Resources["StepIndicator.Passed.Background"] as SolidColorBrush ?? Brushes.ForestGreen : System.Windows.Application.Current.Resources["StepIndicator.Current.Background"] as SolidColorBrush ?? Brushes.DodgerBlue;
        public Brush Connector2Style => CurrentStep == WizardStep.AdditionalInfo ? System.Windows.Application.Current.Resources["StepIndicator.Current.Background"] as SolidColorBrush ?? Brushes.DodgerBlue : System.Windows.Application.Current.Resources["StepIndicator.Default.Background"] as SolidColorBrush ?? Brushes.LightGray;

        public class StepStyle
        {
            public Brush Background { get; set; } = Brushes.LightGray;
            public Brush Foreground { get; set; } = Brushes.Black;
            public FontWeight FontWeight { get; set; } = FontWeights.Normal;
            public Brush HeadingForeground { get; set; } = Brushes.LightGray;
        }

        [ObservableProperty]
        private string _validationMessage = string.Empty;

        #endregion

        #region Collections

        public ObservableCollection<int> YearRange { get; } = new();

        // Data collections
        [ObservableProperty]
        private ObservableCollection<string> _availableMakes = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableModels = new();


        [ObservableProperty]
        private ObservableCollection<string> _availableEngines = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableTransmissions = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableTrimLevels = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableBodyTypes = new();

        #endregion

        public AddVehicleDialogViewModel(IVehicleService vehicleService, 
            ILocalizationService localizationService, 
            ILogger<AddVehicleDialogViewModel> logger) : base(localizationService, logger)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            LoadInitialData();
        }


        #region Loading Data

        private void LoadInitialData()
        {
            // This would typically come from your service, but for demo purposes:
            AvailableMakes.Clear();
            var makes = new[] { "Nissan", "Toyota", "Honda", "Ford", "Chevrolet", "Hyundai", "Kia", "BMW", "Mercedes-Benz", "Tesla", "Subaru", "Jeep", "Volkswagen" };
            foreach (var make in makes)
            {
                AvailableMakes.Add(make);
            }

            // Setup years
            YearRange.Clear();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear + 1; i >= currentYear - 30; i--)
            {
                YearRange.Add(i);
            }

            // Set default year
            Year = currentYear;

            GetErrors();
        }

        private void LoadEngineOptions()
        {
            AvailableEngines.Clear();
            AvailableTransmissions.Clear();

            AvailableTransmissions = new ObservableCollection<string>(new List<string>
            {
                "Automatic", "CVT", "Manual", "Dual-Clutch"
            });

            AvailableEngines = new ObservableCollection<string>(new List<string>
            {
                "2.5L 4-cylinder", "3.5L V6", "2.0L 4-cylinder", "2.5L 4-cylinder", "3.0L V6", "3.5L V6"
            });
        }

        private void LoadTrimAndBodyOptions()
        {
            AvailableTrimLevels.Clear();
            AvailableBodyTypes.Clear();

            AvailableTrimLevels = new ObservableCollection<string>(new List<string> { "LX", "EX", "XLE", "Platinum", "Limited", "Sport", "Touring", "Base", "Plus", "Premier", "RS", "TRD" });
            AvailableBodyTypes = new ObservableCollection<string>(new List<string> { "Sedan", "SUV", "Truck", "Hatchback", "Coupe", "Crossover", "Convertible", "Wagon" });

        }

        private async void LoadModelsForMakeAsync(string make)
        {
            try
            {
                // In a real implementation, get models for the selected make from your service
                var models = await Task.FromResult(LoadAvailableSampleModels(make));

                foreach (var model in models.OrderBy(m => m))
                {
                    AvailableModels.Add(model);
                }

                //// Also load engines based on make
                //LoadEnginesForMakeAsync(make);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading models for make {make}: {ex.Message}");
            }
        }

        private List<string> LoadAvailableSampleModels(string make)
        {
            AvailableModels.Clear();

            var modelData = new Dictionary<string, List<string>>()
            {
                {"Nissan", new List<string>{"Altima", "Sentra", "Versa", "Rogue", "Pathfinder", "Titan", "Frontier", "Maxima", "Leaf", "Armada", "Murano", "Kicks"}},
                {"Toyota", new List<string> { "Camry", "Corolla", "RAV4", "Highlander", "Tacoma", "Tundra", "4Runner" }},
                {"Honda", new List<string> { "Civic", "Accord", "CR-V", "Pilot", "Odyssey", "HR-V" }},
                {"Ford", new List<string> { "F-150", "Explorer", "Escape", "Mustang", "Bronco", "Focus", "Fusion" }},
                {"Chevrolet", new List<string> { "Silverado", "Equinox", "Traverse", "Malibu", "Camaro", "Bolt" }},
                {"Hyundai", new List<string> { "Elantra", "Sonata", "Tucson", "Santa Fe", "Palisade", "Kona" }},
                {"Kia", new List<string> { "Sorento", "Sportage", "Telluride", "Forte", "K5", "Rio" }},
                {"BMW", new List<string> { "3 Series", "5 Series", "X5", "X3", "X1", "7 Series" }},
                {"Mercedes-Benz", new List<string> { "C-Class", "E-Class", "GLC", "GLE", "S-Class", "A-Class" }},
                {"Tesla", new List<string> { "Model 3", "Model Y", "Model S", "Model X" }},
                {"Subaru", new List<string> { "Outback", "Forester", "Crosstrek", "Impreza", "Ascent", "WRX" }},
                {"Jeep", new List<string> { "Grand Cherokee", "Wrangler", "Cherokee", "Compass", "Gladiator" }},
                {"Volkswagen", new List<string> { "Jetta", "Passat", "Tiguan", "Atlas", "Golf", "ID.4" }}
            };

            return modelData.ContainsKey(make) ? modelData[make] : new List<string>();
        }

        #endregion

        #region Commands 

        private bool CanGoToNextStep()
        {
            if (CurrentStep == WizardStep.BasicInfo)
            {
                // Check if Make, Model are filled and Year is valid
                return !string.IsNullOrWhiteSpace(Make) &&
                       !string.IsNullOrWhiteSpace(Model) &&
                       Year >= 1900 && Year <= 2100; // Example validation
            }
            if (CurrentStep == WizardStep.TechnicalDetails)
            {
                // Check if Engine is filled (assuming it's required)
                return !string.IsNullOrWhiteSpace(Engine);
            }
            return false; // Cannot go next from last step
        }

        [RelayCommand(CanExecute = nameof(CanGoToNextStep))]
        private void GoToNextStep()
        {
            if (CurrentStep == WizardStep.BasicInfo)
            {
                LoadEngineOptions(); // Load based on Make/Model properties
                CurrentStep = WizardStep.TechnicalDetails;
            }
            else if (CurrentStep == WizardStep.TechnicalDetails)
            {
                LoadTrimAndBodyOptions(); // Load based on Make/Model/Engine properties
                UpdateVehicleSummary(); // Update summary before showing last step
                CurrentStep = WizardStep.AdditionalInfo;
            }
            // Notify commands that depend on CurrentStep
            GoToNextStepCommand.NotifyCanExecuteChanged();
            GoToPreviousStepCommand.NotifyCanExecuteChanged();
            AddVehicleCommand.NotifyCanExecuteChanged(); // Finish button visibility depends on step
        }



        [RelayCommand]
        private void GoToPreviousStep()
        {
            if (CurrentStep == WizardStep.TechnicalDetails)
            {
                CurrentStep = WizardStep.BasicInfo;
            }
            else if (CurrentStep == WizardStep.AdditionalInfo)
            {
                CurrentStep = WizardStep.TechnicalDetails;
            }
            // Notify commands that depend on CurrentStep
            GoToNextStepCommand.NotifyCanExecuteChanged();
            GoToPreviousStepCommand.NotifyCanExecuteChanged();
            AddVehicleCommand.NotifyCanExecuteChanged(); // Finish button visibility depends on step
        }

        [RelayCommand]
        private void Cancel()
        {
            try
            {
                SetResultAndClose(false);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when cancelling: {ex.Message}");
            }
        }

        private bool CanAddVehicle()
        {
            // Only allow finish on the last step and if not already adding
            return CurrentStep == WizardStep.AdditionalInfo && !IsAddingVehicle;
        }

        [RelayCommand(CanExecute = nameof(CanAddVehicle))]
        private async Task AddVehicleAsync()
        {
            try
            {
                IsAddingVehicle = true;
                ValidationMessage = string.Empty;

                // Validate the vehicle details
                ValidateAllProperties();

                if (HasErrors)
                {
                    ValidationMessage = "Please fix the validation errors.";
                    IsAddingVehicle = false;
                    return;
                }

                // Create the vehicle
                var vehicle = new Application.DTOs.Vehicle.CreateVehicleDto
                (
                    Make = Make,
                    Model = Model,
                    Year = Year,
                    string.IsNullOrWhiteSpace(Vin) ? null : Vin,
                    Engine = Engine,
                    Transmission = Transmission,
                    string.IsNullOrWhiteSpace(TrimLevel) ? null : TrimLevel,
                    string.IsNullOrWhiteSpace(BodyType) ? null : BodyType
                );

                // Save the vehicle
                await _vehicleService.CreateVehicleAsync(vehicle);

                await MessageBoxHelper.ShowMessageWithoutTitleAsync("Vehicle has been added successfully", false, CurrentWorkFlow);

                _logger.LogInformation("Vehicle has been added successfully");


                SetResultAndClose(true);

            }
            catch(ValidationException vx)
            {
                await MessageBoxHelper.ShowMessageWithoutTitleAsync("The car specifications are already exists", true, CurrentWorkFlow);
                return;
            }
            catch (Exception ex)
            {
                ValidationMessage = "An error occurred while adding the vehicle.";
                _logger.LogError($"An exception happened while adding a new vehicle, {ex.Message}");
                throw new ViewModelException(ErrorMessages.CREATE_DATA_EXCEPTION_MESSAGE, nameof(AddVehicleDialogViewModel), nameof(AddVehicleCommand), MethodOperationType.ADD_DATA, ex);
            }
            finally
            {
                IsAddingVehicle = false;
            }
        }

        #endregion

        #region Property Changed Methods

        partial void OnMakeChanged(string value)
        {
            AvailableModels.Clear();


            if (!string.IsNullOrEmpty(value))
            {
                LoadModelsForMakeAsync(value);
            }

            UpdateVehicleSummary();
        }

        partial void OnYearChanged(int value)
        {
            UpdateVehicleSummary();
        }

        partial void OnModelChanged(string value)
        {
            UpdateVehicleSummary();
        }

        partial void OnEngineChanged(string value)
        {
            UpdateVehicleSummary();
        }

        partial void OnTransmissionChanged(string value)
        {
            UpdateVehicleSummary();
        }
        partial void OnTrimLevelChanged(string value)
        {
            UpdateVehicleSummary();
        }

        partial void OnBodyTypeChanged(string value)
        {
            UpdateVehicleSummary();
        }

        #endregion

        #region Helpers

        private StepStyle GetStepStyle(WizardStep step)
        {
            var res = System.Windows.Application.Current.Resources;


            if (step == CurrentStep)
            {
                var background = res["StepIndicator.Current.Background"] as SolidColorBrush ?? Brushes.DodgerBlue;
                var foreground = res["StepIndicator.Current.Foreground"] as SolidColorBrush ?? Brushes.White;
                var headingForeground = res["StepIndicator.Current.HeadingForeground"] as SolidColorBrush ?? Brushes.White;

                return new StepStyle
                {
                    Background = background,
                    Foreground = foreground,
                    FontWeight = FontWeights.SemiBold,
                    HeadingForeground = headingForeground
                };
            }
            else if (step < CurrentStep)
            {
                var background = res["StepIndicator.Passed.Background"] as SolidColorBrush ?? Brushes.ForestGreen;
                var foreground = res["StepIndicator.Passed.Foreground"] as SolidColorBrush ?? Brushes.White;

                return new StepStyle
                {
                    Background = background,
                    Foreground = foreground,
                    FontWeight = FontWeights.Normal,
                    HeadingForeground = background

                };
            }
            else
            {
                var background = res["StepIndicator.Default.Background"] as SolidColorBrush ?? Brushes.LightGray;
                var foreground = res["StepIndicator.Default.Foreground"] as SolidColorBrush ?? Brushes.Black;
                return new StepStyle
                {
                    Background = background,
                    Foreground = foreground,
                    FontWeight = FontWeights.Normal,
                    HeadingForeground = background

                };
            }
        }
        
        public void SetDialogWindow(IDialogWindow dialog)
        {
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

        private void UpdateVehicleSummary()
        {
            VehicleSummary = $"{Year} {Make} {Model}\n" +
                          $"Engine: {Engine}\n" +
                          $"Transmission: {Transmission ?? "Not specified"}\n" +
                          $"Trim Level: {TrimLevel ?? "Not specified"}\n" +
                          $"Body Type: {BodyType ?? "Not specified"}\n" +
                          $"VIN: {Vin ?? "Not specified"}";
        }


        #endregion

    }
}
