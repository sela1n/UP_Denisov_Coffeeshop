using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;
using Microsoft.Win32;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class AddUserPage : Page
    {
        private readonly User _user;

        public AddUserPage(User user)
        {
            InitializeComponent();
            _user = user ?? new User();

            CbRole.ItemsSource = new[] { "User", "Admin" };
            InputHelper.Latin(TbLogin);
            InputHelper.Letters(TbFio);

            if (_user.ID == 0)
            {
                Title = "Добавление пользователя";
                CbRole.SelectedIndex = 0;
                Placeholder.SetText(PbPassword, "Не менее 6 символов, есть цифра");
            }
            else
            {
                Title = "Редактирование пользователя";
                TbLogin.Text = _user.Login;
                CbRole.SelectedItem = _user.Role;
                TbFio.Text = _user.FIO;
                TbPhoto.Text = _user.Photo;
                Placeholder.SetText(PbPassword, "Оставьте пустым, чтобы не менять");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string fio = TbFio.Text.Trim();
            string photo = TbPhoto.Text.Trim();
            string pass = PbPassword.Password;
            bool isNew = _user.ID == 0;
            var db = Entities.GetContext();

            var errors = new StringBuilder();
            if (!Regex.IsMatch(login, @"^[A-Za-z0-9_]{3,30}$"))
                errors.AppendLine("Логин: от 3 до 30 символов — латиница, цифры и «_».");
            else if (db.User.Any(u => u.Login == login && u.ID != _user.ID))
                errors.AppendLine("Пользователь с таким логином уже существует!");

            if (CbRole.SelectedItem == null)
                errors.AppendLine("Выберите роль!");

            if (!Regex.IsMatch(fio, @"^[А-Яа-яЁё]+([ \-][А-Яа-яЁё]+){1,3}$"))
                errors.AppendLine("ФИО: от 2 до 4 слов кириллицей.");

            if (isNew || pass.Length > 0)        // при редактировании пустой пароль = «не менять»
            {
                string passError = AuthHelper.ValidatePassword(pass);
                if (passError != null) errors.AppendLine(passError);
            }

            if (photo.Length > 0 && !Regex.IsMatch(photo, @"\.(png|jpe?g|bmp)$", RegexOptions.IgnoreCase))
                errors.AppendLine("Фото: допустимы файлы png, jpg, bmp.");

            if (errors.Length > 0)
            {
                MessageHelper.Warning(errors.ToString());
                return;
            }

            _user.Login = login;
            _user.Role = (string)CbRole.SelectedItem;
            _user.FIO = fio;
            _user.Photo = photo.Length == 0 ? null : photo;
            if (pass.Length > 0)
                _user.Password = AuthHelper.GetHash(pass);

            if (isNew)
                db.User.Add(_user);

            string error;
            if (!db.TrySave(out error))
            {
                MessageHelper.Error(error);
                return;
            }
            MessageHelper.Info("Данные успешно сохранены!");
            NavigationService.GoBack();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp" };
            if (dialog.ShowDialog() == true)
                TbPhoto.Text = dialog.FileName;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbLogin.Clear();
            TbFio.Clear();
            TbPhoto.Clear();
            PbPassword.Clear();
            CbRole.SelectedIndex = 0;
        }
    }
}
