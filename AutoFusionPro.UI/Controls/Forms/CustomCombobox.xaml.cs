using System.Windows;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Controls.Forms
{
    public partial class CustomCombobox : UserControl
    {
        private TextBox editableTextBox;

        public CustomCombobox()
        {
            InitializeComponent();
            Loaded += CustomCombobox_Loaded;
        }

        private void CustomCombobox_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to the inner combobox events
            InnerComboBox.SelectionChanged += InnerComboBox_SelectionChanged;

            // Update the inner combobox from the dependency properties
            InnerComboBox.ItemsSource = (System.Collections.IEnumerable)ItemsSource;
            InnerComboBox.SelectedItem = SelectedItem;
            InnerComboBox.IsEditable = IsEditable;

            if (IsEditable && !string.IsNullOrEmpty(Text))
            {
                InnerComboBox.Text = Text;
            }

            // Find the TextBox when in editable mode
            if (IsEditable)
            {
                InnerComboBox.Loaded += (s, args) =>
                {
                    try
                    {
                        editableTextBox = GetEditableTextBox(InnerComboBox);
                        if (editableTextBox != null)
                        {
                            editableTextBox.TextChanged += EditableTextBox_TextChanged;
                        }
                    }
                    catch
                    {
                        // Handle exception if template changed
                    }
                };
            }
        }

        // Helper method to find the TextBox in an editable ComboBox
        private TextBox GetEditableTextBox(ComboBox comboBox)
        {
            if (comboBox.Template.FindName("PART_EditableTextBox", comboBox) is TextBox textBox)
            {
                return textBox;
            }

            return null;
        }

        // ItemsSource Property
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(CustomCombobox),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCombobox)d;
            control.InnerComboBox.ItemsSource = (System.Collections.IEnumerable)e.NewValue;
        }

        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // SelectedItem Property
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(CustomCombobox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCombobox)d;
            // Avoid circular updates
            if (control.InnerComboBox.SelectedItem != e.NewValue)
                control.InnerComboBox.SelectedItem = e.NewValue;
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Text Property
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomCombobox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCombobox)d;
            if (control.IsEditable && control.InnerComboBox.Text != (string)e.NewValue)
                control.InnerComboBox.Text = (string)e.NewValue;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // IsEditable Property
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(CustomCombobox),
                new PropertyMetadata(false, OnIsEditableChanged));

        private static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomCombobox)d;
            control.InnerComboBox.IsEditable = (bool)e.NewValue;

            // If switching to editable, we need to find the textbox
            if ((bool)e.NewValue)
            {
                control.InnerComboBox.Loaded += (s, args) =>
                {
                    try
                    {
                        control.editableTextBox = control.GetEditableTextBox(control.InnerComboBox);
                        if (control.editableTextBox != null)
                        {
                            control.editableTextBox.TextChanged += control.EditableTextBox_TextChanged;
                        }
                    }
                    catch
                    {
                        // Handle exception if template changed
                    }
                };
            }
            else if (control.editableTextBox != null)
            {
                // If switching from editable, remove the handler
                control.editableTextBox.TextChanged -= control.EditableTextBox_TextChanged;
                control.editableTextBox = null;
            }
        }

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // PlaceholderText Property
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(CustomCombobox),
                new PropertyMetadata("Select or write something..."));

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        private void InnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the SelectedItem property when the inner combobox changes
            if (InnerComboBox.SelectedItem != SelectedItem)
                SelectedItem = InnerComboBox.SelectedItem;
        }

        private void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the Text property when the inner textbox text changes
            if (IsEditable && sender is TextBox textBox && textBox.Text != Text)
                Text = textBox.Text;
        }
    }
}
