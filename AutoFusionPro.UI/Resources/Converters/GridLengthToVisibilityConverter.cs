using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class GridLengthToVisibilityConverter : BaseValueConverter<GridLengthToVisibilityConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridLength gl)
            {
                // If width is 0 (either pixel or effectively 0 star), then collapse
                return (gl.IsAbsolute && gl.Value == 0) || (gl.IsStar && gl.Value == 0)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
            return Visibility.Collapsed;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
