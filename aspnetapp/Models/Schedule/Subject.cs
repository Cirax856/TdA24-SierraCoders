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
    }
}
