using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static aspnetapp.Models.Lecturer;

namespace aspnetapp.Models
{
    public class Lecturer
    {
        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                return (string.IsNullOrWhiteSpace(title_before) ? string.Empty : title_before)
                    + first_name
                    + (string.IsNullOrWhiteSpace(middle_name) ? string.Empty : " " + middle_name)
                    + " " + last_name
                    + (string.IsNullOrWhiteSpace(title_after) ? string.Empty : " " + title_after);
            }
        }

        public Guid UUID { get; set; }
        public string? title_before { get; set; }
        public required string first_name { get; set; }
        public string? middle_name { get; set; }
        public required string last_name { get; set; }
        public string? title_after { get; set; }
        public string? picture_url { get; set; }
        public string? location { get; set; }
        public string? claim { get; set; }
        public string? bio { get; set; }
        public Tag[] tags { get; set; }
        public uint? price_per_hour { get; set; }
        public Contact contact { get; set; }

        public static bool IsValid(Lecturer lecturer)
        {
            if (lecturer.tags == null)
                lecturer.tags = new Tag[0];

            // wtf is api specification is shit
            /*if (lecturer.contact.emails.Length < 1)
            {
                Log.Info($"Lecturer invalid because: Not enought emails, Lecturer: {lecturer}");
                return false;
            }
            else*/
                lecturer.contact.emails = lecturer.contact.emails.Distinct().ToArray();

            /*if (lecturer.contact.telephone_numbers.Length < 1)
            {
                Log.Info($"Lecturer invalid because: Not enought telephone numbers, Lecturer: {lecturer}");
                return false;
            }
            else*/
                lecturer.contact.telephone_numbers = lecturer.contact.telephone_numbers.Distinct().ToArray();

            for (int i = 0; i < lecturer.tags.Length; i++)
            {
                Tag tag = lecturer.tags[i];
                if (tag.uuid == default)
                    tag.uuid = tag.name.GetHash();
                if (!Program.dbContext.tags.Contains(tag))
                {
                    Program.dbContext.tags.Add(tag);
                    Program.dbContext.SaveChanges();
                }
            }

            return true;
        }

        public override string ToString()
            => $"[Guid: {UUID}, Name: {DisplayName}, Picture url: {picture_url}, Location: {location}, Claim: {claim}, Price per hour: {price_per_hour}, Contact: {contact}]";

        public class Tag
        {
            [Key]
            public Guid uuid { get; set; }
            public required string name { get; set; }

            public static bool operator ==(Tag a, Tag b)
                => a.Equals(b);
            public static bool operator !=(Tag a, Tag b)
                => !a.Equals(b);

            public override bool Equals(object? obj)
            {
                if (obj is null)
                    return false;
                else if (obj is Tag other)
                    return Equals(other);
                else
                    return false;
            }

            public bool Equals(Tag other)
                => uuid.Equals(other.uuid);

            public override int GetHashCode() => uuid.GetHashCode();
        }

        public class Contact
        {
            public required string[] telephone_numbers { get; set; }
            public required string[] emails { get; set; }

            public override string ToString()
                => $"[Phone numbers: {string.Join(',', telephone_numbers)}, Emails: {string.Join(',', emails)}]";
        }

        public Lecturer Clone()
        {
            Tag[] tags = new Tag[this.tags.Length];
            for (int i = 0; i < tags.Length; i++)
                tags[i] = new Tag()
                {
                    uuid = this.tags[i].uuid,
                    name = this.tags[i].name,
                };

            return new Lecturer()
            {
                UUID = this.UUID,
                title_before = this.title_before,
                first_name = this.first_name,
                middle_name = this.middle_name,
                last_name = this.last_name,
                title_after = this.title_after,
                picture_url = this.picture_url,
                location = this.location,
                claim = this.claim,
                bio = this.bio,
                tags = tags,
                price_per_hour = this.price_per_hour,
                contact = new Contact()
                {
                    telephone_numbers = this.contact.telephone_numbers,
                    emails = this.contact.emails
                }
            };
        }


        public static implicit operator DbLecturer(Lecturer lecturer)
            => new DbLecturer()
            {
                UUID = lecturer.UUID,
                title_before = lecturer.title_before,
                first_name = lecturer.first_name,
                middle_name = lecturer.middle_name,
                last_name = lecturer.last_name,
                title_after = lecturer.title_after,
                picture_url = lecturer.picture_url,
                location = lecturer.location,
                claim = lecturer.claim,
                bio = lecturer.bio,
                tags = lecturer.tags.ToList(),
                price_per_hour = lecturer.price_per_hour,
                emails = lecturer.contact.emails,
                telephone_numbers = lecturer.contact.emails
            };
    }

    public class DbLecturer
    {
        [Key]
        public Guid UUID { get; set; }
        public string? title_before { get; set; }
        public string first_name { get; set; }
        public string? middle_name { get; set; }
        public string last_name { get; set; }
        public string? title_after { get; set; }
        public string? picture_url { get; set; }
        public string? location { get; set; }
        public string? claim { get; set; }
        public string? bio { get; set; }
        public ICollection<Tag> tags { get; set; }
        public uint? price_per_hour { get; set; }
        public string[] telephone_numbers { get; set; }
        public string[] emails { get; set; }

        public static implicit operator Lecturer(DbLecturer dbLecturer)
            => new Lecturer()
            {
                UUID = dbLecturer.UUID,
                title_before = dbLecturer.title_before,
                first_name = dbLecturer.first_name,
                middle_name = dbLecturer.middle_name,
                last_name = dbLecturer.last_name,
                title_after = dbLecturer.title_after,
                picture_url = dbLecturer.picture_url,
                location = dbLecturer.location,
                claim = dbLecturer.claim,
                bio = dbLecturer.bio,
                tags = dbLecturer.tags.ToArray(),
                price_per_hour = dbLecturer.price_per_hour,
                contact = new Contact()
                {
                    emails = dbLecturer.emails,
                    telephone_numbers = dbLecturer.telephone_numbers
                }
            };
    }
}
