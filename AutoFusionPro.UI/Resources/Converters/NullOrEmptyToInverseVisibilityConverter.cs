using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class NullOrEmptyToInverseVisibilityConverter : BaseValueConverter<NullOrEmptyToInverseVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
