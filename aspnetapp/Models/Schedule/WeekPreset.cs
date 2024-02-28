namespace aspnetapp.Models.Schedule
{
	public class WeekPreset
	{
        public string Name;
		public ScheduledDay[] Days;

        public WeekPreset(string _name, ScheduledDay[] _days)
        {
            if (string.IsNullOrWhiteSpace(_name)) throw new ArgumentNullException(nameof(_name));
            else if (_days is null) throw new ArgumentNullException(nameof(_days));
            else if (_days.Length != 7) throw new ArgumentException($"{nameof(_days)}.Length must be 7", nameof(_days));

            Name = _name;
            Days = _days;
        }
    }
}
