using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows.Data;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class IsNullOrEmptyConverter : BaseValueConverter<IsNullOrEmptyConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;

            return string.IsNullOrEmpty(input);
        
    }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
    
    }
    }
}
