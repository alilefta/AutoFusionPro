using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows.Media;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class EmptyStringToForegroundConverter : BaseValueConverter<EmptyStringToForegroundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = System.Windows.Application.Current.Resources;

            if (string.IsNullOrWhiteSpace((string)value) || string.IsNullOrEmpty((string)value))
            {
                var gray = res["Text.DisabledBrush"] as SolidColorBrush ?? Brushes.Gray;

                return gray;
            }
            return res["Text.PrimaryBrush"] as SolidColorBrush ?? Brushes.DarkGray;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
