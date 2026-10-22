using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using UP_Denisov_Coffeeshop.Helpers;
using UP_Denisov_Coffeeshop.Pages;

namespace UP_Denisov_Coffeeshop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigated += MainFrame_Navigated;
            MainFrame.Navigate(new AuthPage());
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as Page;
            string title = (page != null && !string.IsNullOrEmpty(page.Title)) ? page.Title : "Сеть кофеен";

            TbPageTitle.Text = title;
            Title = "Сеть кофеен — " + title;
            BtnBack.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, args) => DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            timer.Start();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Toggle();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!MessageHelper.Confirm("Вы действительно хотите выйти из приложения?"))
                e.Cancel = true;
        }
    }
}
