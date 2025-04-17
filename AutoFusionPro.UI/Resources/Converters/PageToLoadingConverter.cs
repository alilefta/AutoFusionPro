using AutoFusionPro.Core.Enums;
using AutoFusionPro.Core.Enums.NavigationPages;
using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class PageToLoadingConverter : BaseValueConverter<PageToLoadingConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ApplicationPage page && parameter is ApplicationPage buttonPage)
            {
                return page == buttonPage;
            }
            return false;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
