using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            CbRole.ItemsSource = new[] { "User", "Admin" };
            CbRole.SelectedIndex = 0;
            InputHelper.Latin(TbLogin);
            InputHelper.Letters(TbFio);
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string fio = TbFio.Text.Trim();
            string pass = PbFirst.Password;

            if (login.Length == 0 || fio.Length == 0 || pass.Length == 0 || PbSecond.Password.Length == 0)
            {
                MessageHelper.Warning("Заполните все поля!");
                return;
            }

            var errors = new StringBuilder();
            if (!Regex.IsMatch(login, @"^[A-Za-z0-9_]{3,30}$"))
                errors.AppendLine("Логин: от 3 до 30 символов — латиница, цифры и «_».");
            if (!Regex.IsMatch(fio, @"^[А-Яа-яЁё]+([ \-][А-Яа-яЁё]+){1,3}$"))
                errors.AppendLine("ФИО: от 2 до 4 слов кириллицей (например, Иванов Иван Иванович).");

            string passError = AuthHelper.ValidatePassword(pass);
            if (passError != null)
                errors.AppendLine(passError);
            else if (pass != PbSecond.Password)
                errors.AppendLine("Пароли не совпадают!");

            if (errors.Length > 0)
            {
                MessageHelper.Warning(errors.ToString());
                return;
            }

            var db = Entities.GetContext();
            if (db.User.AsNoTracking().Any(u => u.Login == login))
            {
                MessageHelper.Warning("Пользователь с таким логином уже существует!");
                return;
            }

            db.User.Add(new User
            {
                Login = login,
                Password = AuthHelper.GetHash(pass),
                Role = (string)CbRole.SelectedItem,
                FIO = fio
            });

            string error;
            if (!db.TrySave(out error))
            {
                MessageHelper.Error(error);
                return;
            }

            MessageHelper.Info("Пользователь успешно зарегистрирован!");
            NavigationService.GoBack();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbLogin.Clear();
            TbFio.Clear();
            PbFirst.Clear();
            PbSecond.Clear();
            CbRole.SelectedIndex = 0;
        }
    }
}
