namespace aspnetapp.Models
{
    public abstract class Schedule
    {
        public abstract ScheduleType ScheduleType { get; }

        public abstract ScheduledDay GetScheduleForDate(DateOnly date);
    }
}
