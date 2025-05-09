using AutoFusionPro.Core.Enums;
using AutoFusionPro.Core.Enums.NavigationPages;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class ValidationErrorsConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return null;

            if (values[0] is ReadOnlyObservableCollection<ValidationError> errors && errors.Count > 0)
            {
                // Return the first error message
                return errors[0].ErrorContent.ToString();
            }

            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
