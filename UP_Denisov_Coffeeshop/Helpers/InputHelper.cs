using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP_Denisov_Coffeeshop.Helpers
{
    public static class InputHelper
    {
        public static void Restrict(TextBox box, Func<char, bool> isAllowed)
        {
            box.PreviewTextInput += (s, e) => e.Handled = !e.Text.All(isAllowed);

            box.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Space && !isAllowed(' ')) e.Handled = true;
            };

            DataObject.AddPastingHandler(box, (s, e) =>
            {
                var text = e.DataObject.GetData(DataFormats.UnicodeText) as string;
                if (text == null || !text.All(isAllowed)) e.CancelCommand();
            });
        }

        public static void DigitsOnly(TextBox box)
        {
            Restrict(box, c => c >= '0' && c <= '9');
        }

        public static void DecimalOnly(TextBox box)
        {
            Restrict(box, c => (c >= '0' && c <= '9') || c == ',' || c == '.');
        }

        public static void Letters(TextBox box)
        {
            Restrict(box, c => char.IsLetter(c) || c == ' ' || c == '-');
        }

        public static void Latin(TextBox box)
        {
            Restrict(box, c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')
                               || (c >= '0' && c <= '9') || c == '_');
        }
    }
}
