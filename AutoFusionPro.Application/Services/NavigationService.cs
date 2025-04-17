using AutoFusionPro.Application.Interfaces;
using AutoFusionPro.Core.Enums.NavigationPages;
using AutoFusionPro.Core.Exceptions.Navigation;
using Microsoft.Extensions.Logging;
using System.Windows.Controls;

namespace AutoFusionPro.Application.Services
{
    public class NavigationService : INavigationService
    {
        //public ApplicationPage DefaultViewName { get; set; }

        public ApplicationPage CurrentViewName { get; set; }

        public UserControl CurrentView { get; set; } = null!;

        public event EventHandler<ApplicationPage> NavigationChanged = null!;

        private Stack<NavigationRecord> _backHistory = new Stack<NavigationRecord>();

        private record NavigationRecord(ApplicationPage Page, UserControl View, object ViewModel);

        public bool CanGoBack => _backHistory.Count > 0;

        private readonly IViewFactory _viewFactory;
        private readonly IViewModelFactory _viewModelFactory;

        public bool IsLoading { get; set; } = false;

        private readonly ILogger<NavigationService> _logger;

        /// <summary>
        /// Constructor for NavigationService
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving views</param>
        /// <param name="logger">Logger for logging navigation activities</param>
        public NavigationService(IViewFactory viewFactory, IViewModelFactory viewModelFactory, ILogger<NavigationService> logger)
        {
            //_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("Trying to get dashboard");

            _logger.LogInformation("NavigationService initialized with starting view: {CurrentViewName}", CurrentViewName);

        }

        public void InitializeDefaultView(ApplicationPage page)
        {
            CurrentViewName = page;
            CurrentView = _viewFactory.CreateView(CurrentViewName);
        }

        /// <summary>
        /// Navigates to a specified ApplicationPage.
        /// </summary>
        /// <param name="page">The target page to navigate to</param>
        public void NavigateTo(ApplicationPage page)
        {
            NavigateTo(page, null);
        }


        /// <summary>
        /// Navigates to a specified ApplicationPage and sets its DataContext to the provided view model.
        /// </summary>
        /// <param name="page">The target page to navigate to</param>
        /// <param name="viewModel">The view model to set as the DataContext of the view</param>
        public async void NavigateTo(ApplicationPage page, object? initializationParameter = null)
        {
            UserControl view;
            object viewModel;

            try
            {
                IsLoading = true;

                if (page == CurrentViewName)
                { 
                    _logger.LogInformation("Attempted navigation to current page: {Page}", page);
                    return;
                }

                // Create view
                view = _viewFactory.CreateView(page);


                if (initializationParameter != null)
                {

                    await Task.Delay(1000);
                    viewModel = _viewModelFactory.ResolveViewModelWithInitialization(page, initializationParameter);

                }
                else
                {
                   viewModel = _viewModelFactory.ResolveViewModel(page);

                }
                if (viewModel == null)
                {
                    _logger.LogWarning("View model initialization failed for page: {Page}", page);
                    NavigateTo(ApplicationPage.Dashboard);
                    return;
                }

                if (viewModel is IInitializableViewModel initializableViewModel && initializationParameter != null && !initializableViewModel.IsInitialized)
                {
                    _logger.LogWarning("View model initialization failed for page: {Page}", page);
                    NavigateTo(ApplicationPage.Dashboard);
                    return;
                }

                // Set view model
                view.DataContext = viewModel;

                // Manage navigation history
                if (CurrentView != null)
                {
                    // Push current view to back history
                    _backHistory.Push(new NavigationRecord(
                        CurrentViewName,
                        CurrentView,
                        CurrentView.DataContext
                    ));
                }


                CurrentView = view;
                CurrentViewName = page;

                // Raise navigation changed event
                OnNavigationChanged(page);  // Raise event for navigation change

                _logger.LogInformation("Navigated to page: {Page} with ViewModel: {ViewModelType}, and Parameter {Param}", page, viewModel.GetType(), initializationParameter);
            }
            catch (NavigationException navEx)
            {
                // Specific navigation exceptions
                _logger.LogError(navEx,
                    "Navigation failed for page: {Page} with parameter: {Param}",
                    page,
                    initializationParameter);

                // Fall back to dashboard
                NavigateTo(ApplicationPage.Dashboard);
            }
            catch (Exception ex)
            {
                // Unexpected errors
                _logger.LogCritical(ex,
                    "Unexpected error during navigation to page: {Page} with parameter: {Param}",
                    page,
                    initializationParameter);

                // Create and throw a new navigation exception with context
                throw new NavigationException(
                    page,
                    $"Unexpected navigation error for page {page}",
                    ex
                );
            }finally
            {
                IsLoading = false;
            }
        }

        protected virtual void OnNavigationChanged(ApplicationPage page)
        {
            NavigationChanged?.Invoke(this, page);
        }

        public void GoBack()
        {
            if (!CanGoBack)
            {
                _logger.LogWarning("Cannot go back - no history available");
                return;
            }

            try
            {
                // Pop last navigation record
                var previousNavigation = _backHistory.Pop();

                // Restore previous view
                CurrentView = previousNavigation.View;
                CurrentViewName = previousNavigation.Page;

                // Restore view model if available
                if (previousNavigation.ViewModel != null)
                {
                    CurrentView.DataContext = previousNavigation.ViewModel;
                }

                // Raise navigation changed event
                OnNavigationChanged(CurrentViewName);

                _logger.LogInformation("Navigated back to page: {Page}", CurrentViewName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Back navigation failed");
                NavigateTo(ApplicationPage.Dashboard);
            }
        }
    }
}

