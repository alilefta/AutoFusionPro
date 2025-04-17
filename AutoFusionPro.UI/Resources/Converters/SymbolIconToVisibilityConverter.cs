using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class SymbolIconToVisibilityConverter : BaseValueConverter<SymbolIconToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SymbolRegular)value == SymbolRegular.Empty ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
