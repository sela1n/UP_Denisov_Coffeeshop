using System.Windows;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += (s, args) =>
            {
                MessageHelper.Error("Непредвиденная ошибка: " + args.Exception.Message);
                args.Handled = true;
            };

            ThemeManager.LoadSaved();
            new MainWindow().Show();
        }
    }
}
