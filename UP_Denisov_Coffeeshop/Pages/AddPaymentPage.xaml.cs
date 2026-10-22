using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class AddPaymentPage : Page
    {
        private static readonly CultureInfo Ru = new CultureInfo("ru-RU");
        private readonly Payment _payment;

        public AddPaymentPage(Payment payment)
        {
            InitializeComponent();
            _payment = payment ?? new Payment();

            var db = Entities.GetContext();
            CbCategory.ItemsSource = db.Category.OrderBy(c => c.Name).ToList();
            CbUser.ItemsSource = db.User.OrderBy(u => u.FIO).ToList();
            DpDate.DisplayDateStart = new DateTime(2020, 1, 1);

            InputHelper.DigitsOnly(TbNum);
            InputHelper.DecimalOnly(TbPrice);

            if (_payment.ID == 0)
            {
                Title = "Добавление продажи";
                DpDate.SelectedDate = DateTime.Today;
            }
            else
            {
                Title = "Редактирование продажи";
                TbName.Text = _payment.Name;
                CbCategory.SelectedItem = ((System.Collections.Generic.IEnumerable<Category>)CbCategory.ItemsSource)
                                            .FirstOrDefault(c => c.ID == _payment.CategoryID);
                CbUser.SelectedItem = ((System.Collections.Generic.IEnumerable<User>)CbUser.ItemsSource)
                                            .FirstOrDefault(u => u.ID == _payment.UserID);
                DpDate.SelectedDate = _payment.Date;
                TbNum.Text = _payment.Num.ToString();
                TbPrice.Text = _payment.Price.ToString("0.00", Ru);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TbName.Text.Trim();
            var errors = new StringBuilder();

            if (name.Length < 2)
                errors.AppendLine("Позиция: введите название (от 2 символов).");
            if (CbCategory.SelectedItem == null)
                errors.AppendLine("Выберите категорию!");
            if (CbUser.SelectedItem == null)
                errors.AppendLine("Выберите сотрудника!");

            if (DpDate.SelectedDate == null)
                errors.AppendLine("Укажите дату продажи!");
            else if (DpDate.SelectedDate.Value < new DateTime(2020, 1, 1) ||
                     DpDate.SelectedDate.Value > DateTime.Today.AddYears(1))
                errors.AppendLine("Дата вне допустимого диапазона (с 01.01.2020 и не позже чем через год).");

            int num;
            if (!int.TryParse(TbNum.Text, out num) || num < 1 || num > 100)
                errors.AppendLine("Количество: целое число от 1 до 100.");

            decimal price;
            bool priceOk = decimal.TryParse(TbPrice.Text.Trim().Replace('.', ','),
                                            NumberStyles.AllowDecimalPoint, Ru, out price);
            if (!priceOk || price <= 0 || price > 10000)
                errors.AppendLine("Цена: число от 0,01 до 10 000.");

            if (errors.Length > 0)
            {
                MessageHelper.Warning(errors.ToString());
                return;
            }

            _payment.Name = name;
            _payment.CategoryID = ((Category)CbCategory.SelectedItem).ID;
            _payment.UserID = ((User)CbUser.SelectedItem).ID;
            _payment.Date = DpDate.SelectedDate.Value.Date;
            _payment.Num = num;
            _payment.Price = Math.Round(price, 2);

            var db = Entities.GetContext();
            if (_payment.ID == 0)
                db.Payment.Add(_payment);

            string error;
            if (!db.TrySave(out error))
            {
                MessageHelper.Error(error);
                return;
            }
            MessageHelper.Info("Данные успешно сохранены!");
            NavigationService.GoBack();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbName.Clear();
            TbNum.Clear();
            TbPrice.Clear();
            CbCategory.SelectedItem = null;
            CbUser.SelectedItem = null;
            DpDate.SelectedDate = DateTime.Today;
        }
    }
}
