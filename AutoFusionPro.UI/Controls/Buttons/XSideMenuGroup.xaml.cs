using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.Controls.Buttons
{
    public partial class XSideMenuGroup : UserControl
    {
        private bool _isExpanded = false;

        public XSideMenuGroup()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty GroupHeaderProperty =
            DependencyProperty.Register(
                nameof(GroupHeader),
                typeof(string),
                typeof(XSideMenuGroup),
                new PropertyMetadata(string.Empty));

        public string GroupHeader
        {
            get { return (string)GetValue(GroupHeaderProperty); }
            set { SetValue(GroupHeaderProperty, value); }
        }

        public static readonly DependencyProperty GroupIconProperty =
            DependencyProperty.Register(
                nameof(GroupIcon),
                typeof(SymbolRegular),
                typeof(XSideMenuGroup),
                new PropertyMetadata(SymbolRegular.Empty));

        public SymbolRegular GroupIcon
        {
            get { return (SymbolRegular)GetValue(GroupIconProperty); }
            set { SetValue(GroupIconProperty, value); }
        }

        public static readonly DependencyProperty SubItemsProperty =
            DependencyProperty.Register(
                nameof(SubItems),
                typeof(object),
                typeof(XSideMenuGroup),
                new PropertyMetadata(null));

        public object SubItems
        {
            get { return GetValue(SubItemsProperty); }
            set { SetValue(SubItemsProperty, value); }
        }

        public static readonly DependencyProperty IsCollapsedModeProperty =
            DependencyProperty.Register(
                nameof(IsCollapsedMode),
                typeof(bool),
                typeof(XSideMenuGroup),
                new PropertyMetadata(false, OnIsCollapsedModeChanged));

        public bool IsCollapsedMode
        {
            get { return (bool)GetValue(IsCollapsedModeProperty); }
            set { SetValue(IsCollapsedModeProperty, value); }
        }

        private static void OnIsCollapsedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is XSideMenuGroup group)
            {
                bool isCollapsed = (bool)e.NewValue;

                // When side menu is collapsed, auto-collapse all group submenus
                if (isCollapsed && group._isExpanded)
                {
                    group.CollapseGroup();
                }
            }
        }

        public static readonly DependencyProperty IsGroupSelectedProperty =
            DependencyProperty.Register(
                nameof(IsGroupSelected),
                typeof(bool),
                typeof(XSideMenuGroup),
                new PropertyMetadata(false));

        public bool IsGroupSelected
        {
            get { return (bool)GetValue(IsGroupSelectedProperty); }
            set { SetValue(IsGroupSelectedProperty, value); }
        }

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            // Don't expand/collapse when in collapsed mode, just trigger navigation
            if (IsCollapsedMode)
            {
                // You might want to trigger navigation to a default page for this group
                // Could fire a custom routed event here that your view model handles
                return;
            }

            if (_isExpanded)
            {
                CollapseGroup();
            }
            else
            {
                ExpandGroup();
            }
        }

        private void ExpandGroup()
        {
            _isExpanded = true;
            // Ensure panel is visible before animation
            SubItemsPanel.Visibility = Visibility.Visible;

            var storyboard = this.Resources["ExpandGroupAnimation"] as Storyboard;
            storyboard?.Begin();
        }

        private void CollapseGroup()
        {
            _isExpanded = false;
            var storyboard = this.Resources["CollapseGroupAnimation"] as Storyboard;
            storyboard?.Begin();
        }
    }
}
