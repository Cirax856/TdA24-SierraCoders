using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace aspnetapp
{
    public static class Utils
    {
        private static readonly SHA256 sha256 = SHA256.Create();
        private static readonly Regex emailRegex = new Regex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");

        public static Guid GetHash(this string s)
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
            bytes = bytes.Take(16).ToArray(); // take first 16 bytes (hash returns 32)
            return new Guid(bytes);
        }

        /// <summary>
        /// Verifies a string
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="name"></param>
        /// <param name="error"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <param name="specialFormat">
        /// 1 - Email
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool Verify(string s, string name, out string error, int minLength = -1, int maxLength = -1, int specialFormat = 0)
        {
            error = string.Empty;

            if (minLength > -1 && s.Length < minLength)
                error = $"{name} is too short, it needs to be at least {minLength} characters!!!";
            else if (maxLength > -1 && s.Length > maxLength)
                error = $"{name} is too long, it can't be longer that {maxLength} characters!!!";
            else if (specialFormat != 0)
                switch (specialFormat)
                {
                    case 1:
                        if (!emailRegex.IsMatch(s))
                            error = $"\"{s}\" isn't valid email address!!!";
                        break;
                    default:
                        throw new ArgumentException($"Invalid value \"{specialFormat}\"", nameof(specialFormat));
                }

            return string.IsNullOrEmpty(error);
        }
    }
}
