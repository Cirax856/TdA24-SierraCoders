namespace aspnetapp.Models
{
    public class RepeatingSchedule : Schedule
    {
        public override ScheduleType ScheduleType => ScheduleType.Repeating;

        public ScheduledDay[] Days = new ScheduledDay[7];
    }
}
