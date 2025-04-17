using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AutoFusionPro.UI.Resources.Converters.Base
{

    /// <summary>
    /// A base value converter that allows direct XAML usage
    /// </summary>
    /// <typeparam name="T">The type of this value converter</typeparam>


    //  Markup extension used to access this directly in XAMl
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter where T : class, new()
    {

        #region Private Members

        /// <summary>
        /// A single static instance of this value converter
        /// </summary>
        private static T mConverter = null;

        #endregion

        #region Markup Extension Method
        /// <summary>
        /// Define what to return to the XAML when value converter is called.
        /// Provide a static instance of the value converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return mConverter ?? (mConverter = new T());
        }

        #endregion

        #region Value converter Methods

        /// <summary>
        /// Convert one value type to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);


        /// <summary>
        /// Convert value type back to original shape
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);


        #endregion
    }
}
