using AutoFusionPro.UI.Resources.Converters.Base;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class FirstValidationErrorConverter : BaseValueConverter<FirstValidationErrorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<ValidationError> errors && errors.Any())
            {
                return errors[0].ErrorContent;
            }
            return null;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
