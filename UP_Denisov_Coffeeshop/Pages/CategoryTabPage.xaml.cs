using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class CategoryTabPage : Page
    {
        public CategoryTabPage()
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

            IEnumerable<Category> items = Entities.GetContext().Category.ToList();
            string query = TbSearch.Text.Trim().ToLower();
            if (query.Length > 0)
                items = items.Where(c => c.Name.ToLower().Contains(query));

            DataGridCategory.ItemsSource = items.ToList();
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) { UpdateGrid(); }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCategoryPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCategoryPage((sender as Button).DataContext as Category));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGridCategory.SelectedItems.Cast<Category>().ToList();
            if (selected.Count == 0)
            {
                MessageHelper.Warning("Выберите записи для удаления!");
                return;
            }

            int payments = selected.Sum(c => c.Payment.Count);
            string text = "Вы точно хотите удалить записей: " + selected.Count + "?";
            if (payments > 0)
                text += "\nКаскадно будут удалены и связанные продажи: " + payments + ".";
            if (!MessageHelper.Confirm(text)) return;

            var db = Entities.GetContext();
            db.Category.RemoveRange(selected);
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
