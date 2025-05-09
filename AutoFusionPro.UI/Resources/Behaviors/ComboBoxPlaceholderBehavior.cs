using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace AutoFusionPro.UI.Resources.Behaviors
{
    public class ComboBoxPlaceholderBehavior : Behavior<ComboBox>
    {
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ComboBoxPlaceholderBehavior),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register(nameof(PlaceholderForeground), typeof(Brush), typeof(ComboBoxPlaceholderBehavior),
                new PropertyMetadata(Brushes.Gray));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public Brush PlaceholderForeground
        {
            get => (Brush)GetValue(PlaceholderForegroundProperty);
            set => SetValue(PlaceholderForegroundProperty, value);
        }

        private TextBlock _placeholderTextBlock;
        private Grid _mainGrid;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
            AssociatedObject.DropDownClosed += AssociatedObject_DropDownClosed;
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;

            // Add TextChanged event handler for editable ComboBoxes
            if (AssociatedObject.IsEditable)
            {
                AssociatedObject.AddHandler(TextBoxBase.TextChangedEvent,
                    new TextChangedEventHandler(AssociatedObject_TextChanged));
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
            AssociatedObject.DropDownClosed -= AssociatedObject_DropDownClosed;
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;

            if (AssociatedObject.IsEditable)
            {
                AssociatedObject.RemoveHandler(TextBoxBase.TextChangedEvent,
                    new TextChangedEventHandler(AssociatedObject_TextChanged));
            }

            base.OnDetaching();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _mainGrid = new Grid();
            var parent = VisualTreeHelper.GetParent(AssociatedObject) as UIElement;
            var index = (parent as Panel)?.Children.IndexOf(AssociatedObject) ?? 0;

            if (parent is Panel panel)
            {
                panel.Children.Remove(AssociatedObject);
                panel.Children.Insert(index, _mainGrid);
                _mainGrid.Children.Add(AssociatedObject);
            }

            _placeholderTextBlock = new TextBlock
            {
                Text = PlaceholderText,
                Foreground = PlaceholderForeground,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 0, 0),
                IsHitTestVisible = false
            };

            _mainGrid.Children.Add(_placeholderTextBlock);
            UpdatePlaceholderVisibility();
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void AssociatedObject_DropDownClosed(object sender, EventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            if (_placeholderTextBlock == null)
                return;

            var comboBox = AssociatedObject;
            bool hasValue = false;

            // Check for both SelectedItem and Text
            if (comboBox.SelectedItem != null)
            {
                hasValue = true;
            }
            else if (comboBox.IsEditable)
            {
                // Access the TextBox within the ComboBox
                var textBox = GetChildOfType<TextBox>(comboBox);
                if (textBox != null && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    hasValue = true;
                }
                // Alternatively, we can check the Text property directly
                else if (!string.IsNullOrWhiteSpace(comboBox.Text))
                {
                    hasValue = true;
                }
            }

            _placeholderTextBlock.Visibility = hasValue ? Visibility.Collapsed : Visibility.Visible;
        }

        // Helper method to find child elements of a specific type
        private static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T result)
                    return result;

                var descendant = GetChildOfType<T>(child);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }
    }
}
