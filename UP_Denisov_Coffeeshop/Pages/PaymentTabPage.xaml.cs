using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class PaymentTabPage : Page
    {
        public PaymentTabPage()
        {
            InitializeComponent();
            CbCategory.DisplayMemberPath = "Name";
            CbCategory.SelectedValuePath = "ID";
            LoadCategories();
            UpdateGrid();
            IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Entities.GetContext().Refresh();
                LoadCategories();
                UpdateGrid();
            }
        }

        private void LoadCategories()
        {
            int selected = (CbCategory.SelectedValue is int) ? (int)CbCategory.SelectedValue : 0;

            var list = new List<Category> { new Category { ID = 0, Name = "Все категории" } };
            list.AddRange(Entities.GetContext().Category.OrderBy(c => c.Name).ToList());

            CbCategory.ItemsSource = list;
            CbCategory.SelectedValue = list.Any(c => c.ID == selected) ? selected : 0;
        }

        private void UpdateGrid()
        {
            if (!IsInitialized) return;

            IEnumerable<Payment> items = Entities.GetContext().Payment.ToList();

            string query = TbSearch.Text.Trim().ToLower();
            if (query.Length > 0)
                items = items.Where(p => p.Name.ToLower().Contains(query));

            int categoryId = (CbCategory.SelectedValue is int) ? (int)CbCategory.SelectedValue : 0;
            if (categoryId != 0)
                items = items.Where(p => p.CategoryID == categoryId);

            DataGridPayment.ItemsSource = items.OrderByDescending(p => p.Date).ThenByDescending(p => p.ID).ToList();
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) { UpdateGrid(); }
        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e) { UpdateGrid(); }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddPaymentPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddPaymentPage((sender as Button).DataContext as Payment));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGridPayment.SelectedItems.Cast<Payment>().ToList();
            if (selected.Count == 0)
            {
                MessageHelper.Warning("Выберите записи для удаления!");
                return;
            }
            if (!MessageHelper.Confirm("Вы точно хотите удалить записей: " + selected.Count + "?"))
                return;

            var db = Entities.GetContext();
            db.Payment.RemoveRange(selected);
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
