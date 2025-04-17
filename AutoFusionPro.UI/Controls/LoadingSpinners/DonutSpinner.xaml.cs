using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AutoFusionPro.UI.Controls.LoadingSpinners
{
    /// <summary>
    /// Interaction logic for DonutSpinner.xaml
    /// </summary>
    public partial class DonutSpinner : UserControl
    {
        public DonutSpinner()
        {
            InitializeComponent();
        }

        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { 

                SetValue(DurationProperty, value); 
            }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(
                "Duration",
                typeof(Duration),
                typeof(DonutSpinner),
                new PropertyMetadata(
                    new Duration(TimeSpan.FromSeconds(1.5)), // Default duration
                    OnDurationChanged
                )
            );

        private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spinner = d as DonutSpinner;
            if (spinner == null) return;

            // Find the rotation animation in the path's triggers
            var rotationAnimation = spinner.FindRotationAnimation();
            if (rotationAnimation != null)
            {

                // Update the duration of the animation
                rotationAnimation.Duration = (Duration)e.NewValue;
            }
        }

        public Brush SpinnerColor
        {
            get { return (Brush)GetValue(SpinnerColorProperty); }
            set { SetValue(SpinnerColorProperty, value); }
        }

        public static readonly DependencyProperty SpinnerColorProperty =
            DependencyProperty.Register(
                "SpinnerColor",
                typeof(Brush),
                typeof(DonutSpinner),
                new PropertyMetadata(Brushes.DodgerBlue)
            );

        // Helper method to find the rotation animation
        private DoubleAnimation? FindRotationAnimation()
        {
            // This assumes the animation is in the first path's triggers
            if (PART_SpinnerPath != null)
            {
                foreach (var trigger in PART_SpinnerPath.Triggers)
                {
                    if (trigger is EventTrigger eventTrigger)
                    {
                        var storyboard = eventTrigger.Actions
                            .OfType<BeginStoryboard>()
                            .FirstOrDefault()?.Storyboard;

                        if (storyboard != null)
                        {
                            return rotationDoubleAnimation;
                        }
                    }
                }
            }
            return null;
        }
    }
}

