using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UP_Denisov_Coffeeshop.Helpers;

namespace UP_Denisov_Coffeeshop.Pages
{
    public partial class AuthPage : Page
    {
        private int _failedAttempts;
        private string _captcha;

        public AuthPage()
        {
            InitializeComponent();
            InputHelper.Latin(TbLogin);
            DataObject.AddPastingHandler(TbCaptchaInput, (s, e) => e.CancelCommand());
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password;

            if (login.Length == 0 || password.Length == 0)
            {
                MessageHelper.Warning("Введите логин и пароль!");
                return;
            }

            User user;
            try
            {
                user = AuthHelper.Authenticate(login, password);
            }
            catch (Exception ex)
            {
                MessageHelper.Error("Не удалось подключиться к базе данных:\n" + ex.GetBaseException().Message);
                return;
            }

            if (user == null)
            {
                _failedAttempts++;
                PbPassword.Clear();
                MessageHelper.Error("Пользователь с такими данными не найден!");
                if (_failedAttempts >= 3)
                    SetCaptchaMode(true);
                return;
            }

            _failedAttempts = 0;
            MessageHelper.Info("Добро пожаловать, " + user.FIO + "!");
            PbPassword.Clear();

            switch (user.Role)
            {
                case "Admin":
                    NavigationService.Navigate(new AdminPage());
                    break;
                case "User":
                    NavigationService.Navigate(new UserPage());
                    break;
                default:
                    MessageHelper.Error("Для пользователя не определена роль!");
                    break;
            }
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        private void BtnChangePass_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangePassPage());
        }


        private void SetCaptchaMode(bool on)
        {
            LoginPanel.Visibility = on ? Visibility.Collapsed : Visibility.Visible;
            CaptchaPanel.Visibility = on ? Visibility.Visible : Visibility.Collapsed;
            if (on) NewCaptcha();
        }

        private void NewCaptcha()
        {
            _captcha = CaptchaHelper.Generate();
            TbCaptcha.Text = _captcha;
            TbCaptchaInput.Clear();
        }

        private void BtnCaptchaOk_Click(object sender, RoutedEventArgs e)
        {
            if (TbCaptchaInput.Text.Trim() == _captcha)
            {
                MessageHelper.Info("Капча введена верно. Повторите вход.");
                _failedAttempts = 0;
                SetCaptchaMode(false);
            }
            else
            {
                MessageHelper.Error("Капча введена неверно! Символы сгенерированы заново.");
                NewCaptcha();
            }
        }

        private void BtnCaptchaRefresh_Click(object sender, RoutedEventArgs e)
        {
            NewCaptcha();
        }

        private void Clipboard_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
