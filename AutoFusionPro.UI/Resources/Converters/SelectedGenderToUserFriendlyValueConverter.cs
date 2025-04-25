using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class SelectedGenderToUserFriendlyValueConverter : BaseValueConverter<SelectedGenderToUserFriendlyValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = System.Windows.Application.Current.Resources;
            var maleStr = res["MaleStr"] as String ?? "Male";
            var femaleStr = res["FemaleStr"] as String ?? "Female";

            switch((Gender) value)
            {
                case Gender.Male:
                    return maleStr;
                case Gender.Female:
                    return femaleStr;
                default:
                    return value;

            }

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
