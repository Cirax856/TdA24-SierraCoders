namespace aspnetapp.Models.Schedule
{
	public class Subject
	{
		public string Name;
		public string Description;

        public Subject(string _name, string _description)
        {
            Name = _name;
            Description = _description;
        }

        public override int GetHashCode()
            => Name.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            else if (obj is Subject other) return Equals(other);
            else return false;
        }

        public bool Equals(Subject other)
            => Name == other.Name;
    }
}
