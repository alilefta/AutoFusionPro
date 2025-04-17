using AutoFusionPro.Core.Enums.UI;
using System.Windows;
using Wpf.Ui.Controls;

namespace AutoFusionPro.UI.Controls.Buttons
{
    public class XButton : System.Windows.Controls.Button
    {
        //static XButton()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(XButton),
        //        new FrameworkPropertyMetadata(typeof(XButton)));
        //}

        #region Dependency Properties: IsLoading, SymbolIcon, SymbolPos, CornerRadius, 

        // IsLoading property to control the spinner visibility
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(
                nameof(IsLoading),
                typeof(bool),
                typeof(XButton),
                new PropertyMetadata(false));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Symbol/Icon property
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(
                nameof(Symbol),
                typeof(SymbolRegular),
                typeof(XButton),
                new PropertyMetadata(null));

        public SymbolRegular Symbol
        {
            get { return (SymbolRegular)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        // Symbol Spacing
        public static readonly DependencyProperty SymbolSpacingProperty =
            DependencyProperty.Register(
                nameof(SymbolSpacing),
                typeof(double),
                typeof(XButton),
                new PropertyMetadata(8.0));

        public double SymbolSpacing
        {
            get { return (double)GetValue(SymbolSpacingProperty); }
            set { SetValue(SymbolSpacingProperty, value); }
        }

        // Corner Radius property
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                nameof(CornerRadius),
                typeof(CornerRadius),
                typeof(XButton),
                new PropertyMetadata(new CornerRadius(4)));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Button Type property to support different visual styles
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register(
                nameof(ButtonType),
                typeof(ButtonType),
                typeof(XButton),
                new PropertyMetadata(ButtonType.Primary));

        public ButtonType ButtonType
        {
            get { return (ButtonType)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

     

        #endregion
    }



}