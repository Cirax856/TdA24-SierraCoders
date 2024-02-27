namespace aspnetapp.Models
{
    public class PerWeekSchedule : Schedule
    {
        public override ScheduleType ScheduleType => ScheduleType.PerWeek;

        public Dictionary<DateOnly, ScheduledDay> Days = new Dictionary<DateOnly, ScheduledDay>();
    }
}
