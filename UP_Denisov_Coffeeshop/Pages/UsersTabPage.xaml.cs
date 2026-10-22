using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            UpdateGrid();
            IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Entities.GetContext().Refresh();
                UpdateGrid();
            }
        }

        private void UpdateGrid()
        {
            if (!IsInitialized) return;

            IEnumerable<User> items = Entities.GetContext().User.ToList();

            string query = TbSearch.Text.Trim().ToLower();
            if (query.Length > 0)
                items = items.Where(u => u.FIO.ToLower().Contains(query) || u.Login.ToLower().Contains(query));

            if (CbRole.SelectedIndex == 1) items = items.Where(u => u.Role == "Admin");
            else if (CbRole.SelectedIndex == 2) items = items.Where(u => u.Role == "User");

            DataGridUser.ItemsSource = items.ToList();
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e) { UpdateGrid(); }
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e) { UpdateGrid(); }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUserPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUserPage((sender as Button).DataContext as User));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGridUser.SelectedItems.Cast<User>().ToList();
            if (selected.Count == 0)
            {
                MessageHelper.Warning("Выберите записи для удаления!");
                return;
            }

            int payments = selected.Sum(u => u.Payment.Count);
            string text = "Вы точно хотите удалить записей: " + selected.Count + "?";
            if (payments > 0)
                text += "\nКаскадно будут удалены и связанные продажи: " + payments + ".";
            if (!MessageHelper.Confirm(text)) return;

            var db = Entities.GetContext();
            db.User.RemoveRange(selected);
            string error;
            if (!db.TrySave(out error))
            {
                MessageHelper.Error(error);
                return;
            }
            MessageHelper.Info("Данные успешно удалены!");
            UpdateGrid();
        }
    }
}
