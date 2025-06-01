using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AutoFusionPro.UI.Resources.Converters
{
    public class TextWithSelectionForegroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Brushes.Black;

            string text = values[0] as string;
            bool isSelected = values[1] is bool selected && selected;

            var res = System.Windows.Application.Current.Resources;
            bool isEmpty = string.IsNullOrWhiteSpace(text);

            if (isEmpty)
            {
                if (isSelected)
                {
                    // Lighter color for empty text on selected background
                    return res["DataGrid.Row.Selected.Empty.ForegroundBrush"] as SolidColorBrush ??
                           new SolidColorBrush(Color.FromRgb(220, 220, 220));
                }
                else
                {
                    // Gray for empty text on normal background
                    return res["Text.DisabledBrush"] as SolidColorBrush ?? Brushes.Gray;
                }
            }
            else
            {
                if (isSelected)
                {
                    // Normal selected text color
                    return res["DataGrid.Row.Selected.ForegroundBrush"] as SolidColorBrush ?? Brushes.White;
                }
                else
                {
                    // Normal text color
                    return res["DataGrid.Row.ForegroundBrush"] as SolidColorBrush ?? Brushes.Black;
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
