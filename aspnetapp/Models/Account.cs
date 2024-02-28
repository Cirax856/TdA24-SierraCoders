using Org.BouncyCastle.Crypto;
using System.Security.Principal;

namespace aspnetapp.Models
{
    public class Account : ISaveLoad<Account>
    {
        public static string FileName => throw new NotImplementedException();

        public bool Verified;
        public DateTime TimeCreated;

        public string Username;
        public string Email;
        public string PasswordHash;
        public Guid LecturerGuid { get; private set; }

        public bool HasLecturer => LecturerGuid != default;

        public Account(string _username, string _email, string _passwordHash)
            : this(false, DateTime.UtcNow, _username, _email, _passwordHash, default)
        { }
        public Account(bool _verified, DateTime _timeCreated, string _username, string _email, string _passwordHash, Guid _lecturerGuid)
        {
            Verified = _verified;
            TimeCreated = _timeCreated;
            Username = _username;
            Email = _email;
            PasswordHash = _passwordHash;
            LecturerGuid = _lecturerGuid;
        }

        public void SetLecturer(Lecturer lecturer)
        {
            if (lecturer.UUID == default)
            {
                lecturer.UUID = Guid.NewGuid();
                Database.AddLectuer(lecturer);
                if (!Database.subjects.ContainsKey(lecturer.UUID))
                    lock (Database.subjects)
                        Database.subjects[lecturer.UUID] = new List<Schedule.Subject>();
            }

            LecturerGuid = lecturer.UUID;
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

        public static Account Load(SaveReader reader)
            => new Account(reader.ReadBool(), reader.ReadDateTime(), reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadGuid());
    }
}
