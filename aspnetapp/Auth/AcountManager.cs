using aspnetapp.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Immutable;

namespace aspnetapp.Auth
{
    public static class AcountManager
    {
        private static ImmutableArray<char> allowedNameChars;

        private static Dictionary<uint, Acount> acounts => Database.acounts;
        private static List<LoginSession> sessions => Database.sessions;

        public static int AcountCount => acounts.Count;

        static AcountManager()
        {
            List<char> nameChars = new List<char>();
            // upper case letters
            for (byte i = 65; i <= 90; i++)
                nameChars.Add((char)i);
            // lower case letters
            for (byte i = 97; i <= 122; i++)
                nameChars.Add((char)i);
            // numbers
            for (byte i = 48; i <= 57; i++)
                nameChars.Add((char)i);

            nameChars.Add(' ');
            nameChars.Add('!');
            nameChars.Add('"');
            nameChars.Add('#');
            nameChars.Add('$');
            nameChars.Add('%');
            nameChars.Add('(');
            nameChars.Add(')');
            nameChars.Add('+');
            nameChars.Add(',');
            nameChars.Add('-');
            nameChars.Add('.');
            nameChars.Add(':');
            nameChars.Add('<');
            nameChars.Add('>');
            nameChars.Add('[');
            nameChars.Add(']');
            nameChars.Add('_');
            nameChars.Add('|');

            allowedNameChars = nameChars.ToImmutableArray();
        }

        public static bool TryCreateAcount(string username, string email, string password, string password2, out string error)
        {
            if (!Utils.Verify(username, "Username", out error, 4, 30))
                return false;
            else if (!Utils.Verify(email, "Email", out error, 4, 60, 1))
                return false;
            else if (!Utils.Verify(password, "Password", out error, 8, 60))
                return false;
            else if (password != password2)
            {
                error = "Passwords must match!";
                return false;
            }

            foreach (KeyValuePair<uint, Acount> item in acounts)
                if (item.Value.Username == username)
                {
                    error = "User with the username already exists!";
                    return false;
                }

            if (!Utils.SendEmail(email, username, "Email Verification", "Please client this link to verify your acount: [link]"))
            {
                error = "Failed to send confirmation email! Check that the email is valid.";
                return false;
            }

            acounts.Add((uint)acounts.Count, new Acount(username, email, SecretHasher.Hash(password), new Guid()));

            return true;
        }

        public static bool TryLogin(string username, string password, out string sessionOrError)
        {
            foreach (KeyValuePair<uint, Acount> item in acounts)
                if (item.Value.Username == username && SecretHasher.Verify(password, item.Value.PasswordHash))
                {
                    LoginSession session = LoginSession.Create(item.Key, TimeSpan.FromHours(1));
                    sessions.Add(session);
                    sessionOrError = session.Cookie;

                    return true;
                }

            sessionOrError = "Acount or password is invalid";

            return false;
        }

        public static bool TryGetAcount
            (string cookie, out Acount acount)
        {
            acount = null;

            for (int i = 0; i < sessions.Count; i++)
            {
                LoginSession session = sessions[i];
                if (session.IsExpired)
                {
                    sessions.RemoveAt(i);
                    i--;
                }
                else if (sessions[i].Cookie == cookie)
                {
                    if (acounts.TryGetValue(sessions[i].Acount, out acount))
                        return true;
                }
            }

            return false;
        }
    }
}
