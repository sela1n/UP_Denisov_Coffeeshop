using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
            InputHelper.Latin(TbLogin);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();

            if (login.Length == 0 || PbOld.Password.Length == 0 ||
                PbNew.Password.Length == 0 || PbConfirm.Password.Length == 0)
            {
                MessageHelper.Warning("Все поля обязательны к заполнению!");
                return;
            }

            var db = Entities.GetContext();
            string oldHash = AuthHelper.GetHash(PbOld.Password);
            var user = db.User.FirstOrDefault(u => u.Login == login && u.Password == oldHash);
            if (user == null)
            {
                MessageHelper.Error("Неверный логин или текущий пароль!");
                return;
            }

            string passError = AuthHelper.ValidatePassword(PbNew.Password);
            if (passError != null)
            {
                MessageHelper.Warning(passError);
                return;
            }
            if (PbNew.Password == PbOld.Password)
            {
                MessageHelper.Warning("Новый пароль должен отличаться от текущего!");
                return;
            }
            if (PbNew.Password != PbConfirm.Password)
            {
                MessageHelper.Warning("Пароли не совпадают!");
                return;
            }

            user.Password = AuthHelper.GetHash(PbNew.Password);
            string error;
            if (!db.TrySave(out error))
            {
                MessageHelper.Error(error);
                return;
            }

            MessageHelper.Info("Пароль успешно изменён!");
            NavigationService.GoBack();
        }
    }
}
