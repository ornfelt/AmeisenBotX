using System.Windows;

namespace AmeisenBotX.Themes
{
    public static class WindowAssist
    {
        // 1. Icon (String/Object for the emoji)
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(object), typeof(WindowAssist), new PropertyMetadata(null));

        public static object GetIcon(DependencyObject obj)
        {
            return obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, object value)
        {
            obj.SetValue(IconProperty, value);
        }

        // 2. HeaderContent (For extra toolbar buttons in the title bar)
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.RegisterAttached("HeaderContent", typeof(object), typeof(WindowAssist), new PropertyMetadata(null));

        public static object GetHeaderContent(DependencyObject obj)
        {
            return obj.GetValue(HeaderContentProperty);
        }

        public static void SetHeaderContent(DependencyObject obj, object value)
        {
            obj.SetValue(HeaderContentProperty, value);
        }
    }
}
