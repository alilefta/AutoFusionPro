using System.Windows;

namespace AutoFusionPro.UI.Resources.AttachedProps
{
    /// <summary>
    /// A base attached property to replace the vanilla WPF attached property
    /// </summary>
    /// <typeparam name="Parent">The parent class to be the attached property</typeparam>
    /// <typeparam name="Property">The type of this attached property</typeparam>

    // we use where Parent : new () to tell the compiler that the undefined Parent is a class and it can be instanciated with new ()
    public abstract class BaseAttachedProperty<Parent, Property> where Parent : new()
    {

        #region Public Events

        /// <summary>
        /// Fired when the value changes
        /// </summary>
        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, e) => { };

        /// <summary>
        /// Fired when the value changes, event when the value is the same
        /// </summary>
        public event Action<DependencyObject, object> ValueUpdated = (sender, e) => { };

        #endregion

        #region Public Properties

        /// <summary>
        /// Singleton instance of our Parent class
        /// </summary>
        public static Parent Instance { get; private set; } = new Parent();

        #endregion

        #region Attached Property Definitions


        /// <summary>
        /// The attached Property of this class
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value",
            typeof(Property),
            typeof(BaseAttachedProperty<Parent, Property>),
            new PropertyMetadata(
                default(Property),
                new PropertyChangedCallback(OnValuePropertyChanged),
                new CoerceValueCallback(OnValuePropertyUpdated)));


        /// <summary>
        /// The callback event when <see cref="ValueProperty"/> is changed
        /// </summary>
        /// <param name="d">The UI element that has its property changed</param>
        /// <param name="e">The arguments for the event</param>
        /// <exception cref="NotImplementedException"></exception>
        public static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            // Call the parent function
            (Instance as BaseAttachedProperty<Parent, Property>)?.OnValueChanged(d, e);
            // Call event listener
            (Instance as BaseAttachedProperty<Parent, Property>)?.ValueChanged(d, e);
        }

        /// <summary>
        /// The callback event when <see cref="ValueProperty"/> is changed, event if it is the same value
        /// </summary>
        /// <param name="d">The UI element that has its property changed</param>
        /// <param name="e">The arguments for the event</param>
        /// <exception cref="NotImplementedException"></exception>
        public static object OnValuePropertyUpdated(DependencyObject d, object value)
        {

            // Call the parent function
            (Instance as BaseAttachedProperty<Parent, Property>)?.OnValueUpdated(d, value);
            // Call event listener
            (Instance as BaseAttachedProperty<Parent, Property>)?.ValueUpdated(d, value);

            return value;
        }

        /// <summary>
        /// Gets the attached property
        /// </summary>
        /// <param name="d">The element to get the property from</param>
        /// <returns></returns>
        public static Property GetValue(DependencyObject d) => (Property)d.GetValue(ValueProperty);

        /// <summary>
        /// Set the attached Property
        /// </summary>
        /// <param name="d">The element to get the property from</param>
        /// <param name="value">The value to set the property to</param>
        public static void SetValue(DependencyObject d, Property value) => d.SetValue(ValueProperty, value);

        #endregion

        #region Event Methods


        /// <summary>
        /// The method that is called whne any attached property of this type is changed
        /// </summary>
        /// <param name="sender">The UI element that has its property changed</param>
        /// <param name="e">The arguments for the event</param>
        public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// The method that is called whne any attached property of this type is changed, even if the value is the same
        /// </summary>
        /// <param name="sender">The UI element that has its property changed</param>
        /// <param name="e">The arguments for the event</param>
        public virtual void OnValueUpdated(DependencyObject sender, object value) { }

        #endregion

    }
}
