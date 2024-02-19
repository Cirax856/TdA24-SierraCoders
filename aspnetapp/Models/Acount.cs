using Org.BouncyCastle.Crypto;

namespace aspnetapp.Models
{
    public class Acount : ISaveLoad<Acount>
    {
        public static string FileName => throw new NotImplementedException();

        public bool Verified;
        public DateTime TimeCreated;

        public string Username;
        public string Email;
        public string PasswordHash;
        public Guid LecturerGuid;

        public Acount(string _username, string _email, string _passwordHash)
            : this(false, DateTime.UtcNow, _username, _email, _passwordHash, default)
        { }
        public Acount(bool _verified, DateTime _timeCreated, string _username, string _email, string _passwordHash, Guid _lecturerGuid)
        {
            Verified = _verified;
            TimeCreated = _timeCreated;
            Username = _username;
            Email = _email;
            PasswordHash = _passwordHash;
            LecturerGuid = _lecturerGuid;
        }

        public void Save(SaveWriter writer)
        {
            writer.WriteBool(Verified);
            writer.WriteDateTime(TimeCreated);
            writer.WriteString(Username);
            writer.WriteString(Email);
            writer.WriteString(PasswordHash);
            writer.WriteGuid(LecturerGuid);
        }

        public static Acount Load(SaveReader reader)
            => new Acount(reader.ReadBool(), reader.ReadDateTime(), reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadGuid());
    }
}
