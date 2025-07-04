using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows.Media;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class CurrentStockToForegroundBrushConverter : BaseValueConverter<CurrentStockToForegroundBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentStock)
            {
                if (currentStock <= 0)
                {
                    return Brushes.Red;

                }
                else
                {
                    return Brushes.Black;
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
