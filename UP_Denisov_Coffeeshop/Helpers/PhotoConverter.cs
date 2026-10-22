using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UP_Denisov_Coffeeshop.Helpers
{
    public class PhotoConverter : IValueConverter
    {
        private static readonly Uri DefaultUri =
            new Uri("pack://application:,,,/res/images/default.png", UriKind.Absolute);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    path = path.Trim().TrimStart('/', '\\');
                    string full = Path.IsPathRooted(path)
                        ? path
                        : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

                    if (File.Exists(full))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = new Uri(full, UriKind.Absolute);
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                }
                catch { }
            }
            return new BitmapImage(DefaultUri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
