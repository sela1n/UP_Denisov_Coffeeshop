using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            UpdateUsers();
        }

        private void UpdateUsers()
        {
            if (!IsInitialized) return;

            try
            {
                IEnumerable<User> users = Entities.GetContext().User.AsNoTracking().ToList();

                string query = TbSearch.Text.Trim().ToLower();
                if (query.Length > 0)
                    users = users.Where(u => u.FIO.ToLower().Contains(query));

                if (CbOnlyAdmin.IsChecked == true)
                    users = users.Where(u => u.Role == "Admin");

                users = CbSort.SelectedIndex == 0
                    ? users.OrderBy(u => u.FIO)
                    : users.OrderByDescending(u => u.FIO);

                var list = users.ToList();
                ListUser.ItemsSource = list;
                TbInfo.Text = list.Count == 0 ? "Ничего не найдено" : "Найдено: " + list.Count;
            }
            catch (Exception ex)
            {
                MessageHelper.Error("Не удалось загрузить данные: " + ex.GetBaseException().Message);
            }
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) { UpdateUsers(); }
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) { UpdateUsers(); }
        private void CbOnlyAdmin_Changed(object sender, RoutedEventArgs e) { UpdateUsers(); }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbSearch.Clear();
            CbSort.SelectedIndex = 0;
            CbOnlyAdmin.IsChecked = false;
        }
    }
}
