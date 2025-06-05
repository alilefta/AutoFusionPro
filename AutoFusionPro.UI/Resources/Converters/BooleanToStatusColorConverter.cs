using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows.Media;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class BooleanToStatusColorConverter : BaseValueConverter<BooleanToStatusColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAdmin)
            {
                if (isAdmin == true)
                {
                    return Brushes.Green;

                }
                else
                {
                    return Brushes.Orange;
                }
            }
            return Brushes.Black; // Default color if no match
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
