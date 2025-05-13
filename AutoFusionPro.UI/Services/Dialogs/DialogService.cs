using AutoFusionPro.Application.Interfaces.Dialogs;
using AutoFusionPro.UI.Extensions;
using AutoFusionPro.UI.Services.Animations;
using AutoFusionPro.UI.ViewModels.VehicleCompatibilityManagement.Dialogs;
using AutoFusionPro.UI.ViewModels.Vehicles.Dialogs;
using AutoFusionPro.UI.Views.VehicleCompatibilityManagement.Dialogs;
using AutoFusionPro.UI.Views.Vehicles.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace AutoFusionPro.UI.Services.Dialogs
{
    // Without Animations
    //public class DialogService : IDialogService
    //{
    //    private readonly IServiceProvider _serviceProvider;

    //    public DialogService(IServiceProvider serviceProvider)
    //    {
    //        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    //    }

    //    public bool? ShowAddVehicleDialog()
    //    {
    //        var mainWindow = System.Windows.Application.Current.MainWindow;
    //        double originalOpacity = mainWindow.Opacity;

    //        try
    //        {
    //            // Create the dialog first
    //            var dialog = new AddVehicleDialog
    //            {
    //                Owner = System.Windows.Application.Current.MainWindow,
    //            };

    //            // Adapt the dialog to the interface if it doesn't already implement it
    //            IDialogWindow dialogWindow = new DialogWindowAdapter(dialog);

    //            // Get the ViewModel and pass the dialog to it
    //            var viewModel = _serviceProvider.GetRequiredService<AddVehicleDialogViewModel>();

    //            // Set the dialog on the ViewModel (assuming your ViewModel has a property or method to set the dialog)
    //            (viewModel as IDialogAware)?.SetDialogWindow(dialogWindow);

    //            // Set DataContext
    //            dialog.DataContext = viewModel;

    //            //System.Windows.Application.Current.MainWindow.Opacity = 0.3;

    //            // Animate opacity change (fade out)
    //            var fadeOutAnimation = new System.Windows.Media.Animation.DoubleAnimation
    //            {
    //                From = originalOpacity,
    //                To = 0.3,
    //                Duration = TimeSpan.FromMilliseconds(250)
    //            };
    //            mainWindow.BeginAnimation(System.Windows.Window.OpacityProperty, fadeOutAnimation);

    //            // Create a handler to animate opacity back when dialog closes
    //            dialog.Closed += (sender, args) =>
    //            {
    //                var fadeInAnimation = new System.Windows.Media.Animation.DoubleAnimation
    //                {
    //                    From = 0.3,
    //                    To = originalOpacity,
    //                    Duration = TimeSpan.FromMilliseconds(250)
    //                };
    //                mainWindow.BeginAnimation(System.Windows.Window.OpacityProperty, fadeInAnimation);
    //            };


    //            // Show the dialog and return result
    //            return dialog.ShowDialog();

    //        }
    //        catch (Exception ex)
    //        {
    //            // If exception occurs, restore opacity immediately (no animation)
    //            mainWindow.Opacity = originalOpacity;
    //            throw;
    //        }
    //    }
    //}

    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public bool? ShowAddMakeDialog()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            double originalOpacity = mainWindow.DimBackground();

            try
            {
                // Create the dialog
                var dialog = new AddMakeDialog
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                };

                // Get the ViewModel
                var viewModel = _serviceProvider.GetRequiredService<AddMakeDialogViewModel>();


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

        public async Task<bool?> ShowAddModelDialog(int makeId)
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            double originalOpacity = mainWindow.DimBackground();

            try
            {
                // Create the dialog
                var dialog = new AddModelDialog
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                };

                // Get the ViewModel
                var viewModel = _serviceProvider.GetRequiredService<AddModelDialogViewModel>();

                await viewModel.InitializeAsync(makeId);


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

        public async Task<bool?> ShowAddTrimLevelDialog(int modelId)
        {
            throw new NotImplementedException();
        }

        public bool? ShowAddVehicleDialog()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            double originalOpacity = mainWindow.DimBackground();

            try
            {
                // Create the dialog
                var dialog = new AddVehicleDialog
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                };

                // Get the ViewModel
                var viewModel = _serviceProvider.GetRequiredService<AddVehicleDialogViewModel>();

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
    }
}
