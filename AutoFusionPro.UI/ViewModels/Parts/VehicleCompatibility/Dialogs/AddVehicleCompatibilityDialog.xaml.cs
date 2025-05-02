using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Domain.Models;
using System.Windows;
using System.Windows.Controls;

namespace AutoFusionPro.UI.ViewModels.Parts.VehicleCompatibility.Dialogs
{
    /// <summary>
    /// Interaction logic for AddVehicleCompatibilityDialog.xaml
    /// </summary>
    public partial class AddVehicleCompatibilityDialog : Window
    {
        private readonly IVehicleService _vehicleService;
        private List<string> _makes;
        private Dictionary<string, List<string>> _modelsByMake;
        private Dictionary<string, List<string>> _enginesByModel;
        private readonly Part _currentPart;

        public PartCompatibility Result { get; private set; }

        public AddVehicleCompatibilityDialog(IVehicleService vehicleService, Part currentPart)
        {
            InitializeComponent();
            _vehicleService = vehicleService;
            _currentPart = currentPart;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // Load all vehicle makes
                //_makes = await _vehicleService.GetAllMakesAsync();
                //MakeComboBox.ItemsSource = _makes;

                //// Load model data
                //_modelsByMake = await _vehicleService.GetModelsByMakeAsync();

                //// Load engine data
                //_enginesByModel = await _vehicleService.GetEnginesByModelAsync();

                // Set up year range combos
                int currentYear = DateTime.Now.Year;
                List<int> years = Enumerable.Range(1950, currentYear - 1950 + 1).Reverse().ToList();
                StartYearComboBox.ItemsSource = years;
                EndYearComboBox.ItemsSource = years;

                // Default to current year
                EndYearComboBox.SelectedItem = currentYear;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicle data: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MakeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MakeComboBox.SelectedItem != null)
            {
                string selectedMake = MakeComboBox.SelectedItem.ToString();
                if (_modelsByMake.ContainsKey(selectedMake))
                {
                    ModelComboBox.ItemsSource = _modelsByMake[selectedMake];
                    ModelComboBox.IsEnabled = true;
                }
            }
            else
            {
                ModelComboBox.ItemsSource = null;
                ModelComboBox.IsEnabled = false;
            }

            // Reset subsequent fields
            EngineComboBox.ItemsSource = null;
        }

        private void ModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelComboBox.SelectedItem != null)
            {
                string selectedModel = ModelComboBox.SelectedItem.ToString();
                if (_enginesByModel.ContainsKey(selectedModel))
                {
                    EngineComboBox.ItemsSource = _enginesByModel[selectedModel];
                }
            }
            else
            {
                EngineComboBox.ItemsSource = null;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (MakeComboBox.SelectedItem == null || ModelComboBox.SelectedItem == null ||
                StartYearComboBox.SelectedItem == null || EndYearComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields", "Validation Error",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string make = MakeComboBox.SelectedItem.ToString();
                string model = ModelComboBox.SelectedItem.ToString();
                int startYear = (int)StartYearComboBox.SelectedItem;
                int endYear = (int)EndYearComboBox.SelectedItem;
                string engine = EngineComboBox.Text;
                string notes = NotesTextBox.Text;

                if (startYear > endYear)
                {
                    MessageBox.Show("Start year cannot be greater than end year", "Validation Error",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Two options:
                // 1. Create multiple vehicle compatibility entries (one per year)
                // 2. Extend the model to support year ranges (recommended)

                // For this example, we'll create a single vehicle and compatibility
               // Vehicle vehicle = await _vehicleService.GetOrCreateVehicleAsync(make, model, startYear, engine);

                Result = new PartCompatibility
                {
                    PartId = _currentPart.Id,
                    //VehicleId = vehicle.Id,
                    Notes = $"{notes} (Fits {startYear}-{endYear})"
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding vehicle compatibility: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
