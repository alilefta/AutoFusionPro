using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class NullToBoolConverter : BaseValueConverter<NullToBoolConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) return false;
            return true;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
