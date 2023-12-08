using System.Text;

namespace aspnetapp.Models
{
    public class DbLecturer
    {
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
        public string tags { get; set; }
        public uint? price_per_hour { get; set; }
        public string contact { get; set; }

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
                tags = TagsToString(lecturer.tags),
                price_per_hour = lecturer.price_per_hour,
                contact = ContactToString(lecturer.contact),
            };
        public static implicit operator Lecturer(DbLecturer lecturer)
            => new Lecturer()
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
                tags = StringToTags(lecturer.tags),
                price_per_hour = lecturer.price_per_hour,
                contact = StringToContact(lecturer.contact),
            };

        private static string TagsToString(Lecturer.Tag[] tags)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < tags.Length; i++)
                builder.Append(tags[i].uuid + ":" + tags[i].name + ";");
            return builder.ToString();
        }
        private static string ContactToString(Lecturer.Contact contact)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < contact.telephone_numbers.Length; i++)
                builder.Append(contact.telephone_numbers[i] + ",");
            builder.Append(";");
            for (int i = 0; i < contact.emails.Length; i++)
                builder.Append(contact.emails[i] + ",");
            return builder.ToString();
        }
        private static Lecturer.Tag[] StringToTags(string s)
        {
            string[] split = s.Split(';', StringSplitOptions.RemoveEmptyEntries);
            Lecturer.Tag[] tags = new Lecturer.Tag[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                string[] split2 = split[i].Split(':');
                tags[i] = new Lecturer.Tag()
                {
                    uuid = new Guid(split2[0]),
                    name = split2[1]
                };
            }
            return tags;
        }
        private static Lecturer.Contact StringToContact(string s)
        {
            string[] split = s.Split(';');
            return new Lecturer.Contact()
            {
                telephone_numbers = split[0].Split(',', StringSplitOptions.RemoveEmptyEntries),
                emails = split[1].Split(',', StringSplitOptions.RemoveEmptyEntries),
            };
        }
    }
}
