namespace aspnetapp.Models
{
    public class Acount : ISaveLoad<Acount>
    {
        public static string FileName => throw new NotImplementedException();

        public string Username;
        public string Email;
        public string PasswordHash;
        public Guid LecturerGuid;

        public Acount(string _username, string _email, string _passwordHash, Guid _lecturerGuid)
        {
            Username = _username;
            Email = _email;
            PasswordHash = _passwordHash;
            LecturerGuid = _lecturerGuid;
        }

        public void Save(SaveWriter writer)
        {
            writer.WriteString(Username);
            writer.WriteString(Email);
            writer.WriteString(PasswordHash);
            writer.WriteGuid(LecturerGuid);
        }

        public static Acount Load(SaveReader reader)
            => new Acount(reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadGuid());
    }
}
