using System;
using System.Linq;
using System.Net.Mail;

namespace pill_note_client.Handlers
{
    public static class DataValidator
    {
        public static bool ValidatePassword(string password)
        {
            // Минимальная длина пароля
            int minLength = 8;

            // Флаги проверки надежности пароля
            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;

            // Проверяем каждый символ пароля
            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUpperCase = true;
                else if (char.IsLower(c))
                    hasLowerCase = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
                else if (char.IsSymbol(c) || char.IsPunctuation(c))
                    hasSpecialChar = true;
            }

            // Проверяем, соответствует ли пароль заданным критериям
            bool isValid = password.Length >= minLength &&
                           hasUpperCase &&
                           hasLowerCase &&
                           hasDigit &&
                           hasSpecialChar;

            return isValid;
        }
        public static bool ValidateEmail(string email)
        {
            // Проверяем, что строка не является пустой или null
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                // Используем встроенный класс MailAddress для проверки синтаксиса адреса электронной почты
                MailAddress mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                // Если возникает исключение FormatException, это означает, что адрес электронной почты имеет неправильный синтаксис
                return false;
            }
        }
        public static string GeneratePassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }
    }
}