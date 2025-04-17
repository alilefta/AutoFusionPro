using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoFusionPro.UI.Controls.Cards
{
    /// <summary>
    /// Base Card control that provides common properties and styling
    /// </summary>
    public class CardBase : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CardBase), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(CardBase), new PropertyMetadata(null));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CardBase),
                new PropertyMetadata(new CornerRadius(8)));

        public static readonly DependencyProperty CardBackgroundProperty =
            DependencyProperty.Register("CardBackground", typeof(Brush), typeof(CardBase),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(CardBase),
                new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(CardBase),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        static CardBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CardBase),
                new FrameworkPropertyMetadata(typeof(CardBase)));
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Brush CardBackground
        {
            get { return (Brush)GetValue(CardBackgroundProperty); }
            set { SetValue(CardBackgroundProperty, value); }
        }

        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public Brush HeaderForeground
        {
            get { return (Brush)GetValue(HeaderForegroundProperty); }
            set { SetValue(HeaderForegroundProperty, value); }
        }
    }

    /// <summary>
    /// InfoCard for displaying read-only information in a card format
    /// </summary>
    public class InfoCard : CardBase
    {
        static InfoCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoCard),
                new FrameworkPropertyMetadata(typeof(InfoCard)));
        }
    }

    /// <summary>
    /// FormCard for input fields and data entry
    /// </summary>
    public class FormCard : CardBase
    {
        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register("IsRequired", typeof(bool), typeof(FormCard), new PropertyMetadata(false));

        static FormCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormCard),
                new FrameworkPropertyMetadata(typeof(FormCard)));
        }

        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }
    }

    /// <summary>
    /// FieldLabel for use within a FormCard
    /// </summary>
    public class FieldLabel : Control
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FieldLabel), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register("IsRequired", typeof(bool), typeof(FieldLabel), new PropertyMetadata(false));

        static FieldLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FieldLabel),
                new FrameworkPropertyMetadata(typeof(FieldLabel)));
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }
    }
}
