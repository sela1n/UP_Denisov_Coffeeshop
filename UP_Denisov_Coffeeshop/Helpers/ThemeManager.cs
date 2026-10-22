using System;
using System.IO;
using System.Windows;

namespace UP_Denisov_Coffeeshop.Helpers
{
    public static class ThemeManager
    {
        private static readonly string[] Themes = { "Dictionary.xaml", "DictionaryCustom.xaml" };

        private static readonly string SettingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "UP_Denisov_Coffeeshop", "theme.txt");

        private static int _current = -1;

        public static void LoadSaved()
        {
            int index = 0;
            try
            {
                if (File.Exists(SettingsFile))
                    int.TryParse(File.ReadAllText(SettingsFile).Trim(), out index);
            }
            catch {  }

            if (index < 0 || index >= Themes.Length) index = 0;
            Apply(index, false);
        }

        public static void Toggle()
        {
            Apply((_current + 1) % Themes.Length, true);
        }

        private static void Apply(int index, bool save)
        {
            var merged = Application.Current.Resources.MergedDictionaries;
            merged.Clear();

            merged.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Themes/" + Themes[index], UriKind.Absolute)
            });
            _current = index;

            if (!save) return;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile));
                File.WriteAllText(SettingsFile, index.ToString());
            }
            catch {  }
        }
    }
}
