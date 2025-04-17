using AutoFusionPro.UI.ViewModels.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AutoFusionPro.UI.Views.Controls.SideMenu
{
    /// <summary>
    /// Interaction logic for SideMenu.xaml
    /// </summary>
    public partial class SideMenuControl : UserControl
    {
        private bool _previousCollapsedState;

        public SideMenuControl()
        {
            InitializeComponent();
            DataContextChanged += SideMenu_DataContextChanged;

        }

        private void SideMenu_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ShellViewModel oldViewModel)
            {
                // Unsubscribe from old view model
                oldViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            if (e.NewValue is ShellViewModel newViewModel)
            {
                // Subscribe to new view model
                newViewModel.PropertyChanged += ViewModel_PropertyChanged;

                // Initialize the previous state
                _previousCollapsedState = newViewModel.Collapsed;
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShellViewModel.Collapsed) && DataContext is ShellViewModel viewModel)
            {
                // Only run animation if state actually changed
                if (_previousCollapsedState != viewModel.Collapsed)
                {
                    if (viewModel.Collapsed)
                    {
                        // Play collapse animation
                        var storyboard = this.Resources["CollapseAnimation"] as Storyboard;
                        storyboard?.Begin(this);
                    }
                    else
                    {
                        // Play expand animation
                        var storyboard = this.Resources["ExpandAnimation"] as Storyboard;
                        storyboard?.Begin(this);
                    }

                    _previousCollapsedState = viewModel.Collapsed;
                }
            }
        }
    }
}
