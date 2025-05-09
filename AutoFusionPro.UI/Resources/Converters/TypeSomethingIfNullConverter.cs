using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class TypeSomethingIfNullConverter : BaseValueConverter<TypeSomethingIfNullConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace((string)value) || string.IsNullOrEmpty((string)value))
            {
                var res = System.Windows.Application.Current.Resources;
                var typeSomething = res["TypeSomethingStr"] as String ?? "Type Something";

                return typeSomething;
            }
            return (string)value;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
