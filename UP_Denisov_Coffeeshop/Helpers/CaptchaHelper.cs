using System;
using System.Text;

namespace UP_Denisov_Coffeeshop.Helpers
{
    public static class CaptchaHelper
    {
        private static readonly Random Rnd = new Random();

        private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";

        public static string Generate(int length = 6)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
                sb.Append(Chars[Rnd.Next(Chars.Length)]);
            return sb.ToString();
        }
    }
}
