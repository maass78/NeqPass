using System.Windows;
using System.Windows.Media;

namespace MaterialControls.Assist
{
    public static class HintAssist 
    {
        #region HintText
        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.RegisterAttached("HintText",
            typeof(string), typeof(HintAssist), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetHintText(DependencyObject target, string value)
            => target.SetValue(HintTextProperty, value);

        public static string GetHintText(DependencyObject target)
         => (string)target.GetValue(HintTextProperty);
        #endregion

        #region HintColor
        public static readonly DependencyProperty HintColorProperty =
            DependencyProperty.RegisterAttached("HintColor",
            typeof(SolidColorBrush), typeof(HintAssist), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkGray), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetHintColor(DependencyObject target, SolidColorBrush value)
           => target.SetValue(HintColorProperty, value);

        public static SolidColorBrush GetHintColor(DependencyObject target)
         => (SolidColorBrush)target.GetValue(HintColorProperty);
        #endregion
    }
}
