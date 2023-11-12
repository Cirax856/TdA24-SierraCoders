namespace aspnetapp.Models
{
    public class Lecturer
    {
        public Guid UUID { get; set; }
        public string title_before { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string title_after { get; set; }
        public string picture_url { get; set; }
        public string location { get; set; }
        public string claim { get; set; }
        public string bio { get; set; }
        public Tag[] tags { get; set; }
        public int price_per_hour { get; set; }
        public Contact contact { get; set; }

        public class Tag
        {
            public Guid uuid { get; set; }
            public string name { get; set; }
        }

        public class Contact
        {
            public string[] telephone_numbers { get; set; }
            public string[] emails { get; set; }
        }
    }
}
