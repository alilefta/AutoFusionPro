using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.Controls.Buttons
{
    public class XSideMenuButton : System.Windows.Controls.Button
    {
        // --- Default Style Key ---
        static XSideMenuButton()
        {
            // Tells WPF to look for the default style in Themes/Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XSideMenuButton),
                new FrameworkPropertyMetadata(typeof(XSideMenuButton)));
        }

        public static readonly DependencyProperty IsCollapsedModeProperty =
            DependencyProperty.Register(
                nameof(IsCollapsedMode),
                typeof(bool),
                typeof(XSideMenuButton),
                new PropertyMetadata(false, OnCollapsedModeChanged));



        // IsLoading property to control the spinner visibility
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(
                nameof(IsLoading),
                typeof(bool),
                typeof(XSideMenuButton),
                new PropertyMetadata(false));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }


        // IsSelected property for highlighting
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected),
                typeof(bool),
                typeof(XSideMenuButton),
                new PropertyMetadata(false));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }


        // Symbol/Icon property
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(
                nameof(Symbol),
                typeof(SymbolRegular),
                typeof(XSideMenuButton),
                new PropertyMetadata(null));

        public SymbolRegular Symbol
        {
            get { return (SymbolRegular)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        // Symbol Spacing
        public static readonly DependencyProperty SymbolSpacingProperty =
            DependencyProperty.Register(
                nameof(SymbolSpacing),
                typeof(double),
                typeof(XSideMenuButton),
                new PropertyMetadata(8.0));

        public double SymbolSpacing
        {
            get { return (double)GetValue(SymbolSpacingProperty); }
            set { SetValue(SymbolSpacingProperty, value); }
        }

        // Corner Radius property
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(XSideMenuButton),
                new PropertyMetadata(new CornerRadius(4)));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }


        public bool IsCollapsedMode
        {
            get { return (bool)GetValue(IsCollapsedModeProperty); }
            set { SetValue(IsCollapsedModeProperty, value); }
        }

        private static void OnCollapsedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as XSideMenuButton;
            if (button != null)
            {
                // Update ToolTip state based on collapsed mode
                UpdateToolTip(button, (bool)e.NewValue);

                // Trigger a re-evaluation of the button's layout and content visibility
                button.InvalidateVisual();
            }
        }


        // Helper to manage tooltip creation and styling
        private static void UpdateToolTip(XSideMenuButton button, bool isCollapsed)
        {
            if (isCollapsed && button.Content != null) // Only show tooltip if collapsed and has content
            {
                // Try to find the custom style
                var style = button.TryFindResource("ModernSideMenuToolTipStyle") as Style;

                // Create a ToolTip instance
                var customToolTip = new ToolTip
                {
                    Content = button.Content, // Set the content
                    Style = style // Apply the style (if found)
                    // PlacementTarget = button, // Usually not needed, default is correct
                    // Placement = PlacementMode.Right, // Can override here or rely on style
                    // HorizontalOffset = 10 // Can override here or rely on style
                };

                // Set the ToolTip instance on the button
                ToolTipService.SetToolTip(button, customToolTip);
            }
            else
            {
                // Remove tooltip when expanded or if content is null
                ToolTipService.SetToolTip(button, null);
            }
        }

        // Override OnContentChanged to update ToolTip if content changes while collapsed
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (this.IsCollapsedMode)
            {
                UpdateToolTip(this, this.IsCollapsedMode);
            }
        }
    }
}
