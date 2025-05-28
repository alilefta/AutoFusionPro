using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class EmptyStringToFilterOptionAllConverter : BaseValueConverter<EmptyStringToFilterOptionAllConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = System.Windows.Application.Current.Resources["AllStr"] as string ?? "All";
            if (string.IsNullOrEmpty((string)value) || string.IsNullOrWhiteSpace((string)value)) 
                return res;
            return value;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
