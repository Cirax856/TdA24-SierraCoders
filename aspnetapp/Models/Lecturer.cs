using System.ComponentModel.DataAnnotations;

namespace aspnetapp.Models
{
    public class Lecturer
    {
        [StringLength(36, MinimumLength = 36)]
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

            if (lecturer.contact.emails.Length < 1)
                return false;
            else
                lecturer.contact.emails = lecturer.contact.emails.Distinct().ToArray();

            if (lecturer.contact.telephone_numbers.Length < 1)
                return false;
            else
                lecturer.contact.telephone_numbers = lecturer.contact.telephone_numbers.Distinct().ToArray();

            return true;
        }

        public class Tag
        {
            public required Guid uuid { get; set; }
            public required string name { get; set; }
        }

        public class Contact
        {
            public required string[] telephone_numbers { get; set; }
            public required string[] emails { get; set; }
        }
    }
}
