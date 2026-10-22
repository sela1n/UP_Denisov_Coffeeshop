using System.Windows;
using System.Windows.Controls;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)    { NavigationService.Navigate(new UsersTabPage()); }
        private void BtnCategory_Click(object sender, RoutedEventArgs e) { NavigationService.Navigate(new CategoryTabPage()); }
        private void BtnPayment_Click(object sender, RoutedEventArgs e)  { NavigationService.Navigate(new PaymentTabPage()); }
        private void BtnDiagram_Click(object sender, RoutedEventArgs e)  { NavigationService.Navigate(new DiagrammPage()); }
        private void BtnUserView_Click(object sender, RoutedEventArgs e) { NavigationService.Navigate(new UserPage()); }
    }
}
