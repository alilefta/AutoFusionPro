using AutoFusionPro.UI.Services.Animations;
using System.Windows;

namespace AutoFusionPro.UI.Extensions
{
    public static class WindowExtensions
    {
        /// <summary>
        /// Shows a dialog window with an entrance animation
        /// </summary>
        /// <param name="window">The dialog window</param>
        /// <param name="animationType">The animation type to use when showing</param>
        /// <returns>The dialog result</returns>
        public static bool? ShowDialogWithAnimation(this Window window, AnimationHelpers.AnimationType animationType = AnimationHelpers.AnimationType.FadeIn)
        {
            AnimationHelpers.ApplyDialogEnterAnimation(window, animationType);
            return window.ShowDialog();
        }

        /// <summary>
        /// Closes a dialog window with an exit animation
        /// </summary>
        /// <param name="window">The dialog window</param>
        /// <param name="animationType">The animation type to use when closing</param>
        /// <param name="dialogResult">The dialog result to set</param>
        public static void CloseWithAnimation(this Window window, AnimationHelpers.AnimationType animationType = AnimationHelpers.AnimationType.FadeOut, bool? dialogResult = null)
        {
            if (dialogResult.HasValue)
            {
                window.DialogResult = dialogResult.Value;
            }

            AnimationHelpers.ApplyDialogExitAnimation(window, animationType);
        }

        /// <summary>
        /// Dims the background window to create a modal effect
        /// </summary>
        /// <param name="window">The window to dim</param>
        /// <param name="opacity">Target opacity (default: 0.3)</param>
        /// <param name="duration">Animation duration in milliseconds (default: 250)</param>
        /// <returns>The original opacity value</returns>
        public static double DimBackground(this Window window, double opacity = 0.3, int duration = 250)
        {
            double originalOpacity = window.Opacity;

            var fadeOutAnimation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = originalOpacity,
                To = opacity,
                Duration = TimeSpan.FromMilliseconds(duration)
            };

            window.BeginAnimation(Window.OpacityProperty, fadeOutAnimation);

            return originalOpacity;
        }

        /// <summary>
        /// Restores the background window to its original opacity
        /// </summary>
        /// <param name="window">The window to restore</param>
        /// <param name="originalOpacity">The original opacity to restore to</param>
        /// <param name="duration">Animation duration in milliseconds (default: 250)</param>
        public static void RestoreBackground(this Window window, double originalOpacity, int duration = 250)
        {
            var fadeInAnimation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = window.Opacity,
                To = originalOpacity,
                Duration = TimeSpan.FromMilliseconds(duration)
            };

            window.BeginAnimation(Window.OpacityProperty, fadeInAnimation);
        }
    }
}
