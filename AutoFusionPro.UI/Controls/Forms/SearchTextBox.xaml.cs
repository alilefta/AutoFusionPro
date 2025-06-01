using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoFusionPro.UI.Controls.Forms
{
    public partial class SearchTextBox : UserControl
    {
        public SearchTextBox()
        {
            InitializeComponent();
        }

        // SearchQuery Property - bound to the TextBox Text
        public static readonly DependencyProperty SearchQueryProperty =
            DependencyProperty.Register("SearchQuery", typeof(string), typeof(SearchTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SearchQuery
        {
            get { return (string)GetValue(SearchQueryProperty); }
            set { SetValue(SearchQueryProperty, value); }
        }

        // IsClearButtonEnabled Property - controls whether the clear button is enabled
        public static readonly DependencyProperty IsClearButtonEnabledProperty =
            DependencyProperty.Register("IsClearButtonEnabled", typeof(bool), typeof(SearchTextBox),
                new PropertyMetadata(true));

        public bool IsClearButtonEnabled
        {
            get { return (bool)GetValue(IsClearButtonEnabledProperty); }
            set { SetValue(IsClearButtonEnabledProperty, value); }
        }

        // ButtonCommand Property - the command to execute when clear button is clicked
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(SearchTextBox),
                new PropertyMetadata(null));

        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        // PlaceholderText Property - the placeholder text to show when SearchQuery is empty
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(SearchTextBox),
                new PropertyMetadata("Search..."));

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        // ButtonVisibility Property - controls the visibility of the clear button
        public static readonly DependencyProperty ButtonVisibilityProperty =
            DependencyProperty.Register("ButtonVisibility", typeof(Visibility), typeof(SearchTextBox),
                new PropertyMetadata(Visibility.Visible));

        public Visibility ButtonVisibility
        {
            get { return (Visibility)GetValue(ButtonVisibilityProperty); }
            set { SetValue(ButtonVisibilityProperty, value); }
        }


        // IsEnabled Property override to handle both TextBox and Button
        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set
            {
                SetValue(IsEnabledProperty, value);
                if (InnerSearchTextBox != null)
                    InnerSearchTextBox.IsEnabled = value;
                if (ClearButton != null)
                    ClearButton.IsEnabled = value && IsClearButtonEnabled;
            }
        }

        // Method to focus the search textbox programmatically
        public new bool Focus()
        {
            return InnerSearchTextBox?.Focus() ?? false;
        }

        // Method to select all text in the search textbox
        public void SelectAll()
        {
            InnerSearchTextBox?.SelectAll();
        }

        // Method to clear the search text
        public void Clear()
        {
            SearchQuery = string.Empty;
        }
    }
}
