using AutoFusionPro.Core.Enums.NavigationPages;
using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows;

namespace AutoFusionPro.UI.Resources.Converters
{
     public class ViewNameToUserFriendlyConverter : BaseValueConverter<ViewNameToUserFriendlyConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? viewName = string.Empty;

            ResourceDictionary resourceDictionary = System.Windows.Application.Current.Resources;

            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Dashboard:
                    viewName = resourceDictionary["DashboardStr"] as string ?? "لوحة التحكم";
                    break;
                case ApplicationPage.Settings:
                    viewName = resourceDictionary["SettingsStr"] as string ?? "الاعدادات";
                    break;
                case ApplicationPage.Account:
                    viewName = resourceDictionary["MyAccountStr"] as string ?? "حسابي";
                    break;
                default:
                    var name = $"{((ApplicationPage)value)}Str";
                    viewName = resourceDictionary[$"{name}"] as string ?? name;
                    break;

            }

            return viewName;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
