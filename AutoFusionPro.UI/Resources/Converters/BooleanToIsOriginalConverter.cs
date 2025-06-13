using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class BooleanToIsOriginalConverter : BaseValueConverter<BooleanToIsOriginalConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if value is a boolean
            if (value is bool boolValue)
            {
                return boolValue
                    ? (string)System.Windows.Application.Current.Resources["OriginalPartStr"] ?? "Original Part"
                    : (string)System.Windows.Application.Current.Resources["AftermarketPartStr"] ?? "Aftermarket Part"; 
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
