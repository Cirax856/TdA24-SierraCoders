namespace aspnetapp.Auth
{
    public class LoginSession : ISaveLoad<LoginSession>
    {
        public static string FileName => throw new NotImplementedException();

        public readonly uint Acount;
        public readonly string Cookie;
        public readonly DateTime Expires;

        public bool IsExpired => DateTime.UtcNow > Expires;

        public LoginSession(uint _acount, string _cookie, DateTime _expires)
        {
            Acount = _acount;
            Cookie = _cookie;
            Expires = _expires;
        }

        public static LoginSession Create(uint acount, TimeSpan lifetime)
            => new LoginSession(acount, CookieCreator.Crate(CookieCreator.AcountLength), DateTime.UtcNow.Add(lifetime));

        public void Save(SaveWriter writer)
        {
            writer.WriteUInt32(Acount);
            writer.WriteString(Cookie);
            writer.WriteDateTime(Expires);
        }

        public static LoginSession Load(SaveReader reader)
            => new LoginSession(reader.ReadUInt32(), reader.ReadString(), reader.ReadDateTime());
    }
}