using AutoFusionPro.Core.Enums;
using AutoFusionPro.Core.Enums.NavigationPages;
using System.Globalization;
using System.Windows.Data;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class PageToLoadingMultiConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return false;

            var loadingPage = values[0] as ApplicationPage?;
            var commandParam = values[1] as ApplicationPage?;

            return loadingPage == commandParam; // True if this page is loading

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
