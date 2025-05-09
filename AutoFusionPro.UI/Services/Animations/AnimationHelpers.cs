using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;

namespace AutoFusionPro.UI.Services.Animations
{
    public static class AnimationHelpers
    {
        #region Dialog Animations

        /// <summary>
        /// Apply enter animation to a dialog window
        /// </summary>
        public static void ApplyDialogEnterAnimation(Window dialog, AnimationType animationType = AnimationType.FadeIn)
        {
            switch (animationType)
            {
                case AnimationType.FadeIn:
                    ApplyFadeInAnimation(dialog);
                    break;
                case AnimationType.SlideFromRight:
                    ApplySlideFromRightAnimation(dialog);
                    break;
                case AnimationType.SlideFromBottom:
                    ApplySlideFromBottomAnimation(dialog);
                    break;
                case AnimationType.ZoomIn:
                    ApplyZoomInAnimation(dialog);
                    break;
                default:
                    ApplyFadeInAnimation(dialog);
                    break;
            }
        }

        /// <summary>
        /// Apply exit animation to a dialog window
        /// </summary>
        public static void ApplyDialogExitAnimation(Window dialog, AnimationType animationType = AnimationType.FadeOut)
        {
            switch (animationType)
            {
                case AnimationType.FadeOut:
                    ApplyFadeOutAnimation(dialog);
                    break;
                case AnimationType.SlideToRight:
                    ApplySlideToRightAnimation(dialog);
                    break;
                case AnimationType.SlideToBottom:
                    ApplySlideToBottomAnimation(dialog);
                    break;
                case AnimationType.ZoomOut:
                    ApplyZoomOutAnimation(dialog);
                    break;
                default:
                    ApplyFadeOutAnimation(dialog);
                    break;
            }
        }

        #region Dialog Animation Implementations

        private static void ApplyFadeInAnimation(Window dialog)
        {
            dialog.Opacity = 0;

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            dialog.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private static void ApplyFadeOutAnimation(Window dialog)
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            fadeOutAnimation.Completed += (s, e) => dialog.Close();
            dialog.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        private static void ApplySlideFromRightAnimation(Window dialog)
        {
            // Ensure we have a transform group to work with
            if (!(dialog.RenderTransform is TranslateTransform))
            {
                dialog.RenderTransform = new TranslateTransform();
                dialog.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            dialog.Opacity = 0;

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 300,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Apply animations
            dialog.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
            dialog.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplySlideToRightAnimation(Window dialog)
        {
            // Ensure we have a transform to work with
            if (!(dialog.RenderTransform is TranslateTransform))
            {
                dialog.RenderTransform = new TranslateTransform();
                dialog.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 0,
                To = 300,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            // Set completion callback
            fadeAnimation.Completed += (s, e) => dialog.Close();

            // Apply animations
            dialog.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
            dialog.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplySlideFromBottomAnimation(Window dialog)
        {
            // Ensure we have a transform to work with
            if (!(dialog.RenderTransform is TranslateTransform))
            {
                dialog.RenderTransform = new TranslateTransform();
                dialog.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            dialog.Opacity = 0;

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 200,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(350)
            };

            // Apply animations
            dialog.RenderTransform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
            dialog.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplySlideToBottomAnimation(Window dialog)
        {
            // Ensure we have a transform to work with
            if (!(dialog.RenderTransform is TranslateTransform))
            {
                dialog.RenderTransform = new TranslateTransform();
                dialog.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 0,
                To = 200,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            // Set completion callback
            fadeAnimation.Completed += (s, e) => dialog.Close();

            // Apply animations
            dialog.RenderTransform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
            dialog.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyZoomInAnimation(Window dialog)
        {
            // Setup transform
            dialog.RenderTransform = new ScaleTransform(0.7, 0.7);
            dialog.RenderTransformOrigin = new Point(0.5, 0.5);
            dialog.Opacity = 0;

            // Create animations
            var scaleXAnimation = new DoubleAnimation
            {
                From = 0.7,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
            };

            var scaleYAnimation = new DoubleAnimation
            {
                From = 0.7,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            // Apply animations
            (dialog.RenderTransform as ScaleTransform).BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
            (dialog.RenderTransform as ScaleTransform).BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
            dialog.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyZoomOutAnimation(Window dialog)
        {
            // Setup transform if not already set
            if (!(dialog.RenderTransform is ScaleTransform))
            {
                dialog.RenderTransform = new ScaleTransform(1, 1);
                dialog.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Create animations
            var scaleXAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.7,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var scaleYAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.7,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            // Set completion callback
            fadeAnimation.Completed += (s, e) => dialog.Close();

            // Apply animations
            (dialog.RenderTransform as ScaleTransform).BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
            (dialog.RenderTransform as ScaleTransform).BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
            dialog.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        #endregion

        #endregion

        #region User Control Navigation Animations

        /// <summary>
        /// Apply enter animation to a user control
        /// </summary>
        public static void ApplyEnterAnimation(UIElement control, AnimationType animationType = AnimationType.FadeIn)
        {
            switch (animationType)
            {
                case AnimationType.FadeIn:
                    ApplyControlFadeInAnimation(control);
                    break;
                case AnimationType.SlideFromRight:
                    ApplyControlSlideFromRightAnimation(control);
                    break;
                case AnimationType.SlideFromLeft:
                    ApplyControlSlideFromLeftAnimation(control);
                    break;
                case AnimationType.SlideFromTop:
                    ApplyControlSlideFromTopAnimation(control);
                    break;
                case AnimationType.SlideFromBottom:
                    ApplyControlSlideFromBottomAnimation(control);
                    break;
                case AnimationType.ZoomIn:
                    ApplyControlZoomInAnimation(control);
                    break;
                default:
                    ApplyControlFadeInAnimation(control);
                    break;
            }
        }

        /// <summary>
        /// Apply exit animation to a user control
        /// </summary>
        public static void ApplyExitAnimation(UIElement control, AnimationType animationType = AnimationType.FadeOut, Action completedCallback = null)
        {
            switch (animationType)
            {
                case AnimationType.FadeOut:
                    ApplyControlFadeOutAnimation(control, completedCallback);
                    break;
                case AnimationType.SlideToRight:
                    ApplyControlSlideToRightAnimation(control, completedCallback);
                    break;
                case AnimationType.SlideToLeft:
                    ApplyControlSlideToLeftAnimation(control, completedCallback);
                    break;
                case AnimationType.SlideToTop:
                    ApplyControlSlideToTopAnimation(control, completedCallback);
                    break;
                case AnimationType.SlideToBottom:
                    ApplyControlSlideToBottomAnimation(control, completedCallback);
                    break;
                case AnimationType.ZoomOut:
                    ApplyControlZoomOutAnimation(control, completedCallback);
                    break;
                default:
                    ApplyControlFadeOutAnimation(control, completedCallback);
                    break;
            }
        }

        #region User Control Animation Implementations

        private static void ApplyControlFadeInAnimation(UIElement control)
        {
            control.Opacity = 0;

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlFadeOutAnimation(UIElement control, Action completedCallback)
        {
            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            if (completedCallback != null)
            {
                fadeAnimation.Completed += (s, e) => completedCallback();
            }

            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlSlideFromRightAnimation(UIElement control)
        {
            // Ensure we have a transform
            if (!(control.RenderTransform is TranslateTransform))
            {
                control.RenderTransform = new TranslateTransform();
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            control.Opacity = 0;

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 100,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            // Apply animations
            control.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlSlideToRightAnimation(UIElement control, Action completedCallback)
        {
            // Ensure we have a transform
            if (!(control.RenderTransform is TranslateTransform))
            {
                control.RenderTransform = new TranslateTransform();
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 0,
                To = 100,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            if (completedCallback != null)
            {
                fadeAnimation.Completed += (s, e) => completedCallback();
            }

            // Apply animations
            control.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlSlideFromLeftAnimation(UIElement control)
        {
            // Ensure we have a transform
            if (!(control.RenderTransform is TranslateTransform))
            {
                control.RenderTransform = new TranslateTransform();
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            control.Opacity = 0;

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = -100,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            // Apply animations
            control.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlSlideToLeftAnimation(UIElement control, Action completedCallback)
        {
            // Ensure we have a transform
            if (!(control.RenderTransform is TranslateTransform))
            {
                control.RenderTransform = new TranslateTransform();
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 0,
                To = -100,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            if (completedCallback != null)
            {
                fadeAnimation.Completed += (s, e) => completedCallback();
            }

            // Apply animations
            control.RenderTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlSlideFromTopAnimation(UIElement control)
        {
            // Ensure we have a transform
            if (!(control.RenderTransform is TranslateTransform))
            {
                control.RenderTransform = new TranslateTransform();
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            control.Opacity = 0;

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = -50,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            // Apply animations
            control.RenderTransform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlSlideToTopAnimation(UIElement control, Action completedCallback)
        {
            // Ensure we have a transform
            if (!(control.RenderTransform is TranslateTransform))
            {
                control.RenderTransform = new TranslateTransform();
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 0,
                To = -50,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            if (completedCallback != null)
            {
                fadeAnimation.Completed += (s, e) => completedCallback();
            }

            // Apply animations
            control.RenderTransform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlSlideFromBottomAnimation(UIElement control)
        {
            // Ensure we have a transform
            if (!(control.RenderTransform is TranslateTransform))
            {
                control.RenderTransform = new TranslateTransform();
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            control.Opacity = 0;

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 50,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            // Apply animations
            control.RenderTransform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlSlideToBottomAnimation(UIElement control, Action completedCallback)
        {
            // Ensure we have a transform
            if (!(control.RenderTransform is TranslateTransform))
            {
                control.RenderTransform = new TranslateTransform();
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Create animations
            var moveAnimation = new DoubleAnimation
            {
                From = 0,
                To = 50,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            if (completedCallback != null)
            {
                fadeAnimation.Completed += (s, e) => completedCallback();
            }

            // Apply animations
            control.RenderTransform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlZoomInAnimation(UIElement control)
        {
            // Setup transform
            control.RenderTransform = new ScaleTransform(0.8, 0.8);
            control.RenderTransformOrigin = new Point(0.5, 0.5);
            control.Opacity = 0;

            // Create animations
            var scaleXAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new BackEase { Amplitude = 0.2, EasingMode = EasingMode.EaseOut }
            };

            var scaleYAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(350),
                EasingFunction = new BackEase { Amplitude = 0.2, EasingMode = EasingMode.EaseOut }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            // Apply animations
            (control.RenderTransform as ScaleTransform).BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
            (control.RenderTransform as ScaleTransform).BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private static void ApplyControlZoomOutAnimation(UIElement control, Action completedCallback)
        {
            // Setup transform if not already set
            if (!(control.RenderTransform is ScaleTransform))
            {
                control.RenderTransform = new ScaleTransform(1, 1);
                control.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Create animations
            var scaleXAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var scaleYAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.8,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            if (completedCallback != null)
            {
                fadeAnimation.Completed += (s, e) => completedCallback();
            }

           // Apply animations
           (control.RenderTransform as ScaleTransform).BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
            (control.RenderTransform as ScaleTransform).BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
        }

        #endregion

            #endregion

            #region Animation Types

        public enum AnimationType
        {
            // Generic
            FadeIn,
            FadeOut,

            // Slides
            SlideFromLeft,
            SlideToLeft,
            SlideFromRight,
            SlideToRight,
            SlideFromTop,
            SlideToTop,
            SlideFromBottom,
            SlideToBottom,

            // Zooms
            ZoomIn,
            ZoomOut
        }

        #endregion
    }
}
