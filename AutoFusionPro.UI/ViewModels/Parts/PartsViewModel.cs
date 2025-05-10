using AutoFusionPro.Application.DTOs.Part;
using AutoFusionPro.Application.Interfaces.DataServices;
using AutoFusionPro.Core.Exceptions.ViewModel;
using AutoFusionPro.UI.Services;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.Parts.Dialogs;
using AutoFusionPro.UI.Views.Parts.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AutoFusionPro.UI.ViewModels.Parts
{
    public partial class PartsViewModel : BaseViewModel<PartsViewModel>
    {
        private readonly IPartService _partService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<PartSummaryDto> Parts { get; private set; }

        [ObservableProperty]
        private bool _isLoading = false;        
        
        [ObservableProperty]
        private bool _isAddingPart = false;

        public PartsViewModel(IPartService partService, 
            ILocalizationService localizationService, 
            IServiceProvider serviceProvider,
            ILogger<PartsViewModel> logger) : base(localizationService, logger)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _ = LoadPartsData();
        }

        #region Load and Initialize


        private async Task LoadPartsData()
        {
            IsLoading = true;

            try
            {

                var partsData = await _partService.GetPartSummariesAsync();

                Parts = new ObservableCollection<PartSummaryDto>(partsData);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying to load Parts data, {ex.Message}");
                throw new ViewModelException("An error occurred while trying to load Parts data", nameof(PartsViewModel), nameof(LoadPartsData), "Load", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        [RelayCommand]
        public void ShowAddPartDialog() 
        {
            IsAddingPart = true;
            try
            {
                var addPartDialog = new AddPartDialog();
                var addPartsDialogViewModelLogger = _serviceProvider.GetRequiredService<ILogger<AddPartDialogViewModel>>();
                var addPartDialogViewModel = new AddPartDialogViewModel(_partService, _localizationService, addPartsDialogViewModelLogger);


                addPartDialog.DataContext = addPartDialogViewModel;
                addPartDialog.Owner = System.Windows.Application.Current.MainWindow;

                bool? result = addPartDialog.ShowDialog();

                if (result == true)
                {
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception happened while opening AddPartDialog, {ex.Message}");
                throw new ViewModelException("An error occurred in AddPartDialog", nameof(PartsViewModel), nameof(ShowAddPartDialog), "Open Dialog", ex);
            }finally
            {
                IsAddingPart = false;
            }

        }

        public async void RefreshData()
        {
            await LoadPartsData();
        }

    }
}
