using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows;

namespace AutoFusionPro.UI.Resources.Converters
{
     public class EmptyStringToNotFoundConverter : BaseValueConverter<EmptyStringToNotFoundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string)value) || string.IsNullOrEmpty((string)value))
            {
                var res = System.Windows.Application.Current.Resources;
                var NotFoundStr = res["NotFoundStr"] as String ?? "Not Found";

                return NotFoundStr;
            }
            return (string)value;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
