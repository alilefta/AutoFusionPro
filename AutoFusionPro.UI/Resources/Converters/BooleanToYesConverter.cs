using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;

namespace AutoFusionPro.UI.Resources.Converters 
{ 
    public class BooleanToYesConverter : BaseValueConverter<BooleanToYesConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return String.Empty;

            // Check if value is a boolean
            if (value is bool boolValue)
            {
                return boolValue
                    ? System.Windows.Application.Current.Resources["YesStr"] as string ?? "Yes"
                    : System.Windows.Application.Current.Resources["NoStr"] as string ?? "No";
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
