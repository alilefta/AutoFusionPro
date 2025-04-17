using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class DoubleMarginConverter : BaseValueConverter<DoubleMarginConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double spacing = (double)value;
            if((string)parameter == "LEFT")
            {
                return new System.Windows.Thickness(spacing, 0, 0, 0);

            }
            else
            {
                return new System.Windows.Thickness(0, 0, spacing, 0);

            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
