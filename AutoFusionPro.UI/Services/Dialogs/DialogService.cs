using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.Application.Interfaces.UI;
using AutoFusionPro.UI.Extensions;
using AutoFusionPro.UI.Services.Animations;
using AutoFusionPro.UI.ViewModels.Base;
using AutoFusionPro.UI.ViewModels.General.Dialogs;
using AutoFusionPro.UI.Views.General.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace AutoFusionPro.UI.Services.Dialogs
{
    /// <summary>
    /// Show Animated Dialogs Service.
    /// </summary>
    public class DialogService : IDialogService
    {
        #region Provider

        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        #endregion

        #region General Dialog Methods

        public bool? ShowConfirmDeleteItemsDialog(int count)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            double originalOpacity = mainWindow.DimBackground();

            try
            {
                // Create the dialog
                var dialog = new ConfirmDeleteItemsDialog
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                };

                // Get the ViewModel
                var viewModel = _serviceProvider.GetRequiredService<ConfirmDeleteItemsDialogViewModel>();

                viewModel.ItemsCount = count;

                // Create the dialog adapter and set it on the ViewModel
                var dialogAdapter = new AnimatedDialogWindowAdapter(
                    dialog,
                    AnimationHelpers.AnimationType.FadeIn,  // Show animation
                    AnimationHelpers.AnimationType.FadeOut          // Close animation
                );

                // Set the adapter on the ViewModel
                (viewModel as IDialogAware)?.SetDialogWindow(dialogAdapter);

                // Set DataContext
                dialog.DataContext = viewModel;

                // Handle dialog closed event to restore main window opacity
                EventHandler closedHandler = null;
                closedHandler = (sender, args) =>
                {
                    mainWindow.RestoreBackground(originalOpacity);
                    dialog.Closed -= closedHandler; // Remove handler to prevent memory leaks
                };
                dialog.Closed += closedHandler;

                // Show the dialog with animation
                return dialog.ShowDialogWithAnimation(AnimationHelpers.AnimationType.FadeIn);
            }
            catch (Exception ex)
            {
                // If exception occurs, restore opacity immediately
                mainWindow.Opacity = originalOpacity;
                throw;
            }
        }

        public async Task<bool?> ShowDialogAsync<TDialogViewModel, TView>(object? param = null)
            where TDialogViewModel : class
            where TView : Window, new()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            double originalOpacity = mainWindow.DimBackground();

            try
            {
                // Create the dialog
                var dialog = new TView
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                };

                // Get the ViewModel
                var viewModel = _serviceProvider.GetRequiredService<TDialogViewModel>();

                if(viewModel is IInitializableViewModel initializableViewModel)
                {
                    await initializableViewModel.InitializeAsync(param);
                    await initializableViewModel.Initialized;

                }

                // Create the dialog adapter and set it on the ViewModel
                var dialogAdapter = new AnimatedDialogWindowAdapter(
                    dialog,
                    AnimationHelpers.AnimationType.FadeIn,  // Show animation
                    AnimationHelpers.AnimationType.FadeOut          // Close animation
                );

                // Set the adapter on the ViewModel
                (viewModel as IDialogAware)?.SetDialogWindow(dialogAdapter);

                // Set DataContext
                dialog.DataContext = viewModel;

                // Handle dialog closed event to restore main window opacity
                EventHandler closedHandler = null;
                closedHandler = (sender, args) =>
                {
                    mainWindow.RestoreBackground(originalOpacity);
                    dialog.Closed -= closedHandler; // Remove handler to prevent memory leaks
                };
                dialog.Closed += closedHandler;

                // Show the dialog with animation
                return dialog.ShowDialogWithAnimation(AnimationHelpers.AnimationType.FadeIn);
            }
            catch (Exception ex)
            {
                // If exception occurs, restore opacity immediately
                mainWindow.Opacity = originalOpacity;
                throw;
            }
        }

        public async Task<TResult?> ShowDialogWithResultsAsync<TDialogViewModel, TView, TResult>(object? param = null)
            where TDialogViewModel : class, IDialogViewModelWithResult<TResult>
            where TView : Window, new()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            double originalOpacity = mainWindow.DimBackground();

            try
            {
                // Create the dialog
                var dialog = new TView
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                };

                // Get the ViewModel
                var viewModel = _serviceProvider.GetRequiredService<TDialogViewModel>();

                if (viewModel is InitializableViewModel<TDialogViewModel> initializableViewModel)
                {
                    await initializableViewModel.InitializeAsync(param);

                }

                // Create the dialog adapter and set it on the ViewModel
                var dialogAdapter = new AnimatedDialogWindowAdapter(
                    dialog,
                    AnimationHelpers.AnimationType.FadeIn,  // Show animation
                    AnimationHelpers.AnimationType.FadeOut          // Close animation
                );

                // Set the adapter on the ViewModel
                (viewModel as IDialogAware)?.SetDialogWindow(dialogAdapter);

                // Set DataContext
                dialog.DataContext = viewModel;

                // Handle dialog closed event to restore main window opacity
                EventHandler closedHandler = null;
                closedHandler = (sender, args) =>
                {
                    mainWindow.RestoreBackground(originalOpacity);
                    dialog.Closed -= closedHandler; // Remove handler to prevent memory leaks
                };
                dialog.Closed += closedHandler;

                // Show the dialog with animation
                var dialogShowResults = dialog.ShowDialogWithAnimation(AnimationHelpers.AnimationType.FadeIn);

                if (dialogShowResults.HasValue && dialogShowResults.Value == true)
                {
                    return viewModel.GetResult();
                }

                return default(TResult); // like returning null

            }
            catch (Exception ex)
            {
                // If exception occurs, restore opacity immediately
                mainWindow.Opacity = originalOpacity;
                throw;
            }
        }

        #endregion

    }
}
