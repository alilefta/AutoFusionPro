using AutoFusionPro.Core.Enums.ModelEnum;
using AutoFusionPro.UI.Resources.Converters.Base;
using System.Globalization;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class UnitOfMeasureSymbolToUserFriendlyNameConverter : BaseValueConverter<UnitOfMeasureSymbolToUserFriendlyNameConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = System.Windows.Application.Current.Resources;
            var pcsStr = res["PcsStr"] as String ?? "Pcs";
            var LStr = res["LiterStr"] as String ?? "Liter";
            var kgStr = res["KGStr"] as String ?? "Kg";
            var boxStr = res["BoxStr"] as String ?? "Box";
            var mStr = res["MeterStr"] as String ?? "Meter";
            var prStr = res["PairStr"] as String ?? "Pair";
            var sStr = res["SetStr"] as String ?? "Set";

            switch ((string)value)
            {
                case "pcs":
                    return pcsStr;
                case "L":
                    return LStr;
                case "kg":
                    return kgStr;
                case "box":
                    return boxStr;
                case "m":
                    return mStr;
                case "s":
                    return sStr;
                case "pr":
                    return prStr;
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
