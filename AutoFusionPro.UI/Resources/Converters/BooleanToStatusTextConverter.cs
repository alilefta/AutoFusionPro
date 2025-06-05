using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class BooleanToStatusTextConverter : BaseValueConverter<BooleanToStatusTextConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if value is a boolean
            if (value is bool boolValue)
            {
                return boolValue
                    ? (string)System.Windows.Application.Current.Resources["ActiveStr"] ?? "Active"
                    : (string)System.Windows.Application.Current.Resources["DisabledStr"] ?? "Disabled"; 
            }

            // If value is not a boolean, return an empty string or some default
            return string.Empty;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
