namespace aspnetapp.Models.Schedule
{
	public class ScheduleInfo
	{
		public Dictionary<DateOnly, Schedule> Schedules = new Dictionary<DateOnly, Schedule>();
		public List<Subject> Subjects = new List<Subject>();
		public List<WeekPreset> Presets = new List<WeekPreset>();
	}
}
