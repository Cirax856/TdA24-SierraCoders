using aspnetapp.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Immutable;

namespace aspnetapp.Auth
{
    public static class AccountManager
    {
        private static ImmutableArray<char> allowedNameChars;

        private static Dictionary<uint, Account> accounts => Database.accounts;
        private static List<LoginSession> sessions => Database.sessions;
        private static Dictionary<string, uint> verifications => Database.emailVerifications;

        public static int AcountCount => accounts.Count;

        static AccountManager()
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

        public static bool TryCreateAcount(string host, string username, string email, string password, string password2, out string error)
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

            uint idToRemove = uint.MaxValue;
            lock (Database.accounts)
                foreach (KeyValuePair<uint, Account> item in accounts)
                    if (item.Value.Username == username)
                    {
                        if (!item.Value.Verified && item.Value.TimeCreated.AddMinutes(2) < DateTime.UtcNow)
                        {
                            List<string> toRemove = new List<string>();
                            lock (Database.emailVerifications)
                            {
                                foreach (var verif in verifications)
                                    if (verif.Value == item.Key)
                                        toRemove.Add(verif.Key);

                                for (int i = 0; i < toRemove.Count; i++)
                                    verifications.Remove(toRemove[i]);
                            }

                            idToRemove = item.Key;
                        }
                        else
                        {
                            error = "User with the username already exists!";
                            return false;
                        }
                    }

            if (idToRemove != uint.MaxValue)
                lock (Database.accounts)
                    accounts.Remove(idToRemove);

            string verification = CookieCreator.Crate(CookieCreator.EmailVerifycationLength);

            if (!Utils.SendEmail(email, username, "Email Verification", $"Please client this link to verify your acount: http://{host}/account/verify?id={verification}"))
            {
                error = "Failed to send confirmation email! Check that the email is valid.";
                return false;
            }

            lock (accounts)
            {
                lock (Database.emailVerifications)
                    verifications.Add(verification, (uint)accounts.Count);
                lock (Database.accounts)
                    accounts.Add((uint)accounts.Count, new Account(username, email, SecretHasher.Hash(password)));
            }

            return true;
        }

        public static bool TryLogin(string username, string password, out string sessionOrError)
        {
            lock (Database.accounts)
                foreach (KeyValuePair<uint, Account> item in accounts)
                    if (item.Value.Username == username && SecretHasher.Verify(password, item.Value.PasswordHash))
                    {
                        if (!item.Value.Verified)
                        {
                            sessionOrError = "Please verify your email first";
                            return false;
                        }

                        LoginSession session = LoginSession.Create(item.Key, TimeSpan.FromHours(1));
                        lock (Database.sessions)
                            sessions.Add(session);
                        sessionOrError = session.Cookie;

                        return true;
                    }

            sessionOrError = "Acount or password is invalid";

            return false;
        }

        public static bool TryGetAcount
            (string cookie, out Account acount)
        {
            acount = null;

            lock (Database.sessions)
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
                        if (accounts.TryGetValue(sessions[i].Acount, out acount))
                            return true;
                    }
                }

            return false;
        }
    }
}
