using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoFusionPro.UI.Resources.Converters
{
     public class EnumToBooleanConverter : BaseValueConverter<EnumToBooleanConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string enumValue = value.ToString();
            string targetValue = parameter.ToString();
            return enumValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            bool boolValue = (bool)value;
            if (!boolValue) // If false, we can't convert back to a specific enum value meaningfully
                return Binding.DoNothing; // Or return the original enum value if it was passed in MultiBinding

            string targetValue = parameter.ToString();
            return Enum.Parse(value.GetType(), targetValue, true); // May not be robust enough for all enum types
        }
    }
}
