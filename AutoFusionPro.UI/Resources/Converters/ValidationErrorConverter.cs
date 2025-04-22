using AutoFusionPro.UI.Resources.Converters.Base;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class ValidationErrorConverter : BaseValueConverter<ValidationErrorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errors = value as ReadOnlyObservableCollection<ValidationError>;
            if (errors != null && errors.Count > 0)
                return errors[0].ErrorContent;
            return string.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
