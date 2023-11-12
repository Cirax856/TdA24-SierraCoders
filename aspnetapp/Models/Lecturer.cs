using System.ComponentModel.DataAnnotations;

namespace aspnetapp.Models
{
    public class Lecturer
    {
        [StringLength(36, MinimumLength = 36)]
        public required Guid UUID { get; set; }
        public string? title_before { get; set; }
        public required string first_name { get; set; }
        public string? middle_name { get; set; }
        public required string last_name { get; set; }
        public string? title_after { get; set; }
        public string? picture_url { get; set; }
        public string? location { get; set; }
        public string? claim { get; set; }
        public string? bio { get; set; }
        public required Tag[] tags { get; set; }
        public uint? price_per_hour { get; set; }
        public Contact contact { get; set; }

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
