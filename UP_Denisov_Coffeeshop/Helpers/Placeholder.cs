using System.Windows;
using System.Windows.Controls;

namespace UP_Denisov_Coffeeshop.Helpers
{
    public static class Placeholder
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Placeholder),
                new PropertyMetadata(string.Empty));

        public static string GetText(DependencyObject o) { return (string)o.GetValue(TextProperty); }
        public static void SetText(DependencyObject o, string value) { o.SetValue(TextProperty, value); }

   
        private static readonly DependencyPropertyKey IsEmptyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsEmpty", typeof(bool), typeof(Placeholder),
                new PropertyMetadata(true));

        public static readonly DependencyProperty IsEmptyProperty = IsEmptyKey.DependencyProperty;

        public static bool GetIsEmpty(DependencyObject o) { return (bool)o.GetValue(IsEmptyProperty); }

        public static readonly DependencyProperty WatchProperty =
            DependencyProperty.RegisterAttached("Watch", typeof(bool), typeof(Placeholder),
                new PropertyMetadata(false, OnWatchChanged));

        public static bool GetWatch(DependencyObject o) { return (bool)o.GetValue(WatchProperty); }
        public static void SetWatch(DependencyObject o, bool value) { o.SetValue(WatchProperty, value); }

        private static void OnWatchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as PasswordBox;
            if (box == null) return;

            if ((bool)e.NewValue)
            {
                box.PasswordChanged += OnPasswordChanged;
                Update(box);
            }
            else
            {
                box.PasswordChanged -= OnPasswordChanged;
            }
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Update((PasswordBox)sender);
        }

        private static void Update(PasswordBox box)
        {
            box.SetValue(IsEmptyKey, box.Password.Length == 0);
        }
    }
}
