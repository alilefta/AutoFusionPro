using AutoFusionPro.Application.DTOs.Category;
using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Core.Enums.DTOEnums;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.Views.Parts.Dialogs.Filters;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Parts.Dialogs.Filters
{
    public partial class PartFilterOptionsDialogViewModel : InitializableViewModel<PartFilterOptionsDialogViewModel>, IDialogViewModelWithResult<PartFilterCriteriaDto>
    {

        #region Fields
        private IDialogWindow _dialog = null!;
        private PartFilterCriteriaDto? _resultFilters;

        #endregion

        #region Props

        [ObservableProperty]
        private CategoryDto? _selectedCategory;

        //[ObservableProperty]
        //private SupplierDto _selectedSupplier;

        [ObservableProperty]
        private decimal? _minSellingPrice;
        [ObservableProperty]
        private decimal? _maxSellingPrice;

        [ObservableProperty]
        private string? _manufacturer;

        [ObservableProperty]
        private StockStatusFilter? _selectedStockFilter;

        [ObservableProperty]
        private bool? _isActive;

        [ObservableProperty]
        private bool? _isOriginal;

        #endregion

        #region Data Collections

        [ObservableProperty]
        private ObservableCollection<CategoryDto> _categories = new();


        //[ObservableProperty]
        //private ObservableCollection<SupplierDto> _suppliers = new();

        #endregion

        public PartFilterOptionsDialogViewModel(ILocalizationService localizationService, ILogger<PartFilterOptionsDialogViewModel> logger) : base(localizationService, logger)
        {
        }

        #region Initializer
        /// <summary>
        /// Inherited from <see cref="InitializableViewModel{TViewModel}"/>
        /// </summary>
        /// <param name="parameter"><see cref="null"/></param>
        /// <returns><see cref="void"/></returns>
        public override async Task InitializeAsync(object? parameter = null)
        {
            if (IsInitialized) return;

            //await LoadMakesAsync();
            //await LoadEngineTypesAsync();
            //await LoadTransmissionTypesAsync();
            //await LoadBodyTypesAsync();

            //PopulaShowByActivityCombobox();
            //PopulateYearsCollection();

            //// Populate fields if Parameter (CompatibleVehicleFilterCriteriaDto) already have filters.
            //if (parameter is RuleFilterCriteriaDto currentFilters)
            //{
            //    if (currentFilters.MakeId != null && currentFilters.MakeId.HasValue && currentFilters.MakeId.Value > 0)
            //        SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == currentFilters.MakeId.Value);
            //    else
            //        SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);


            //    if (SelectedMake != null && SelectedMake.Id > 0 && currentFilters.ModelId != null && currentFilters.ModelId.HasValue && currentFilters.ModelId.Value > 0)
            //    {
            //        SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == currentFilters.ModelId.Value);
            //    }
            //    else
            //    {
            //        SelectedModel = ModelsCollection.FirstOrDefault(m => m.Id == 0);
            //    }

            //    if (SelectedMake != null &&
            //        SelectedMake.Id > 0 &&
            //        SelectedModel != null &&
            //        SelectedModel.Id > 0 &&
            //        currentFilters.TrimLevelId != null &&
            //        currentFilters.TrimLevelId.HasValue &&
            //        currentFilters.TrimLevelId.Value > 0)
            //    {
            //        SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(m => m.Id == currentFilters.TrimLevelId.Value);
            //    }
            //    else
            //    {
            //        SelectedTrimLevel = TrimLevelsCollection.FirstOrDefault(m => m.Id == 0);
            //    }

            //    if (currentFilters.EngineTypeId != null && currentFilters.EngineTypeId.HasValue)
            //    {
            //        SelectedEngineType = EngineTypesCollection.FirstOrDefault(m => m.Id == currentFilters.EngineTypeId.Value);
            //    }
            //    else
            //    {
            //        SelectedEngineType = EngineTypesCollection.FirstOrDefault(m => m.Id == 0);
            //    }

            //    if (currentFilters.BodyTypeId != null && currentFilters.BodyTypeId.HasValue)
            //    {
            //        SelectedBodyType = BodyTypesCollection.FirstOrDefault(m => m.Id == currentFilters.BodyTypeId.Value);
            //    }
            //    else
            //    {
            //        SelectedBodyType = BodyTypesCollection.FirstOrDefault(m => m.Id == 0);
            //    }

            //    if (currentFilters.TransmissionTypeId != null && currentFilters.TransmissionTypeId.HasValue)
            //    {
            //        SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == currentFilters.TransmissionTypeId.Value);
            //    }
            //    else
            //    {
            //        SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
            //    }

            //    if (currentFilters.ExactYear != null)
            //    {
            //        SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == currentFilters.ExactYear.Value)
            //                            ?? YearsCollection.FirstOrDefault(y => y.Year == null); // "All Years" item
            //    }
            //    else
            //    {
            //        SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null); // Default to "All Years"
            //    }


            //    if (currentFilters.ShowActiveOnly.HasValue && currentFilters.ShowActiveOnly.Value == true)
            //    {
            //        SelectedShowByActivityItem = ShowRulesByActivityFilterDtos.FirstOrDefault(r => r.value == true);
            //    }
            //    else if (currentFilters.ShowActiveOnly.HasValue && currentFilters.ShowActiveOnly.Value == false)
            //    {
            //        SelectedShowByActivityItem = ShowRulesByActivityFilterDtos.FirstOrDefault(r => r.value == false);

            //    }
            //    else
            //    {
            //        SelectedShowByActivityItem = ShowRulesByActivityFilterDtos.FirstOrDefault(r => r.value == null);
            //    }
            //}
            //else
            //{
            //    // Set default "All" selections if no filters are passed in
            //    SelectedMake = MakesCollection.FirstOrDefault(m => m.Id == 0);
            //    SelectedEngineType = EngineTypesCollection.FirstOrDefault(e => e.Id == 0);
            //    SelectedTransmission = TransmissionTypesCollection.FirstOrDefault(t => t.Id == 0);
            //    SelectedBodyType = BodyTypesCollection.FirstOrDefault(b => b.Id == 0);
            //    SelectedExactYear = YearsCollection.FirstOrDefault(y => y.Year == null);
            //    SelectedShowByActivityItem = ShowRulesByActivityFilterDtos.FirstOrDefault(r => r.value == null);
            //    ShowOnlyTemplates = false; // Default
            //}

            await base.InitializeAsync(parameter); // Call base to set IsInitialized and complete TaskCompletionSource
        }

        #endregion


        #region Dialog Specific Methods

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

        public void SetDialogWindow(IDialogWindow dialog)
        {
            if (dialog == null)
            {
                _logger.LogError("The Dialog window was null on {VM}", nameof(PartFilterOptionsDialog));
                return;
            }

            _dialog = dialog;
        }

        /// <summary>
        /// Set the Dialog Results. Inherited from <see cref="IDialogViewModelWithResult{TResult}"/> interface.
        /// </summary>
        /// <returns><see cref="CompatibleVehicleFilterCriteriaDto?"/></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PartFilterCriteriaDto? GetResult()
        {
            return _resultFilters; // Return the filters set by ApplyFilters
        }

        #endregion
    }
}
