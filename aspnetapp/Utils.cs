using Ganss.Xss;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

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
                error = $"{name} is too short, it needs to be at least {minLength} characters!";
            else if (maxLength > -1 && s.Length > maxLength)
                error = $"{name} is too long, it can't be longer that {maxLength} characters!";
            else if (specialFormat != 0)
                switch (specialFormat)
                {
                    case 1:
                        if (!emailRegex.IsMatch(s))
                            error = $"\"{s}\" isn't valid email address!";
                        break;
                    default:
                        throw new ArgumentException($"Invalid value \"{specialFormat}\"", nameof(specialFormat));
                }

            return string.IsNullOrEmpty(error);
        }

        public static bool SendEmail(string toMail, string toDispName, string subject, string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect("smtp.seznam.cz", 465, SecureSocketOptions.SslOnConnect);

                    Log.Debug($"Authenticating mail with username: teacherdigitalagency, Pass: {Database.emailPass.Substring(0, 6)}...");
                    client.Authenticate("teacherdigitalagency", Database.emailPass);

                    MimeMessage email = new MimeMessage();
                    email.From.Add(new MailboxAddress("Teacher Digital Agency", "teacherdigitalagency@seznam.cz"));
                    email.To.Add(new MailboxAddress(toDispName, toMail));

                    email.Subject = subject;
                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    Log.Debug($"Sending auth email to {toMail}");

                    client.Send(email);
                    Log.Debug("Sent email");

                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to send email to: {toMail}");
                Log.Exception(ex);
                return false;
            }
        }

        public static string Sanitize(this string s)
        {
            if (s == null) return null;

            HtmlSanitizer sanitizer = new HtmlSanitizer();

            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("b"); // bold
            sanitizer.AllowedTags.Add("em"); // italic
            sanitizer.AllowedTags.Add("i"); // italic

            return sanitizer.Sanitize(s);
        }

        public static void Sanitize(ref string s)
        {
            if (s == null) return;

            HtmlSanitizer sanitizer = new HtmlSanitizer();

            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("b"); // bold
            sanitizer.AllowedTags.Add("em"); // italic
            sanitizer.AllowedTags.Add("i"); // italic

            s = sanitizer.Sanitize(s);
        }

        public static int ToInt(this DayOfWeek value)
            => value switch
            {
                DayOfWeek.Monday =>    0,
                DayOfWeek.Tuesday =>   1,
                DayOfWeek.Wednesday => 2,
                DayOfWeek.Thursday =>  3,
                DayOfWeek.Friday =>    4,
                DayOfWeek.Saturday =>  5,
                DayOfWeek.Sunday =>    6,
                _ =>                   0
            };
    }
}
