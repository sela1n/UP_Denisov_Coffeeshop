using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UP_Denisov_Coffeeshop.Helpers
{
    public static class AuthHelper
    {
        public static string GetHash(string password)
        {
            using (var sha = SHA1.Create())
            {
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(password))
                                        .Select(b => b.ToString("X2")));
            }
        }

        public static string ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return "Пароль слишком короткий, должно быть минимум 6 символов!";

            bool hasDigit = false;
            foreach (char c in password)
            {
                if (c >= '0' && c <= '9') hasDigit = true;
                else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                    return "Используйте только английскую раскладку (латинские буквы и цифры)!";
            }
            if (!hasDigit)
                return "Добавьте хотя бы одну цифру!";
            return null;
        }

        public static User Authenticate(string login, string password)
        {
            string hash = GetHash(password);
            using (var db = new Entities())    
            {
                return db.User.AsNoTracking()
                              .FirstOrDefault(u => u.Login == login && u.Password == hash);
            }
        }
    }
}
