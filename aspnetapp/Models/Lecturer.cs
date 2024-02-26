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
        public string first_name { get; set; }
        public string? middle_name { get; set; }
        public string last_name { get; set; }
        public string? title_after { get; set; }
        public string? picture_url { get; set; }
        public string? location { get; set; }
        public string? claim { get; set; }
        public string? bio { get; set; }
        public Tag[] tags { get; set; }
        public uint? price_per_hour { get; set; }
        public Contact contact { get; set; }

        /// <summary>
        /// Applies non null values of lecturer and returns the result
        /// </summary>
        public Lecturer Apply(Lecturer lecturer)
            => new Lecturer()
            {
                UUID = lecturer.UUID,
                title_before = lecturer.title_before == null ? this.title_before : lecturer.title_before,
                first_name = lecturer.first_name,
                middle_name = lecturer.middle_name == null ? this.middle_name : lecturer.middle_name,
                last_name = lecturer.last_name,
                title_after = lecturer.title_after == null ? this.title_after : lecturer.title_after,
                picture_url = lecturer.picture_url == null ? this.picture_url : lecturer.picture_url,
                location = lecturer.location == null ? this.location : lecturer.location,
                claim = lecturer.claim == null ? this.claim : lecturer.claim,
                bio = lecturer.bio == null ? this.bio : lecturer.bio,
                tags = lecturer.tags == null ? this.tags : lecturer.tags,
                price_per_hour = lecturer.price_per_hour.HasValue ? lecturer.price_per_hour : this.price_per_hour,
                contact = lecturer.contact == null ? this.contact : lecturer.contact,
            };

        public static bool Validate(Lecturer lecturer)
        {
            if (lecturer.UUID == default)
            {
                lecturer.UUID = lecturer.DisplayName.GetHash();
                Log.Info($"Created lecturer uuid: {lecturer.UUID}, Name: {lecturer.DisplayName}");
            }

            if (string.IsNullOrWhiteSpace(lecturer.first_name))
            {
                Log.Info($"Lecturer isn't valid because first name \"{lecturer.first_name}\" is required");
                return false;
            } else if (string.IsNullOrWhiteSpace(lecturer.last_name))
            {
                Log.Info($"Lecturer isn't valid because last name \"{lecturer.last_name}\" is required");
                return false;
            }

            if (lecturer.tags == null)
                lecturer.tags = new Tag[0];

            if (lecturer.contact.emails == null || lecturer.contact.emails.Length < 1)
            {
                Log.Info($"Lecturer isn't valid because at least 1 email is required");
                return false;
            } else if (lecturer.contact.telephone_numbers == null || lecturer.contact.telephone_numbers.Length < 1)
            {
                Log.Info($"Lecturer isn't valid because at least 1 telephone number is required");
                return false;
            }
            lecturer.contact.emails = lecturer.contact.emails.Distinct().Select(email => email.Sanitize()).ToArray();

            lecturer.contact.telephone_numbers = lecturer.contact.telephone_numbers.Distinct().Select(phone_number => phone_number.Sanitize()).ToArray();

            for (int i = 0; i < lecturer.tags.Length; i++)
            {
                Tag tag = lecturer.tags[i];
                tag.name = tag.name.Sanitize();

                if (tag.uuid == default)
                    tag.uuid = tag.name.GetHash();
                if (!Database.ContainsTag(tag))
                    Database.AddTag(tag);
            }

            return true;
        }

        public string TagsToString()
            => tags is null ? string.Empty : string.Join<Tag>(", ", tags);

        public Lecturer Clone()
            => new Lecturer()
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
                tags = this.tags.Select(tag => tag.Clone()).ToArray(),
                price_per_hour = this.price_per_hour,
                contact = this.contact.Clone(),
            };

        public override string ToString()
            => $"[Guid: {UUID}, Name: {DisplayName}, Picture url: {picture_url}, Location: {location}, Claim: {claim}, Price per hour: {price_per_hour}, Contact: {contact}]";

        public class Tag
        {
            public Guid uuid { get; set; }
            public string name { get; set; }

            public static bool operator ==(Tag a, Tag b)
                => a.Equals(b);
            public static bool operator !=(Tag a, Tag b)
                => !a.Equals(b);

            public Tag Clone()
                => new Tag()
                {
                    uuid = this.uuid,
                    name = this.name
                };

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
                => name.Equals(other.name);

            public override int GetHashCode() => name.GetHashCode();

            public override string ToString()
                => name;
        }

        public class Contact
        {
            public string[] telephone_numbers { get; set; }
            public string[] emails { get; set; }

            public Contact Clone()
                => new Contact()
                {
                    telephone_numbers = (string[])this.telephone_numbers.Clone(),
                    emails = (string[])this.emails.Clone()
                };

            public override string ToString()
                => $"[Phone numbers: {string.Join(',', telephone_numbers)}, Emails: {string.Join(',', emails)}]";
        }
    }
}
