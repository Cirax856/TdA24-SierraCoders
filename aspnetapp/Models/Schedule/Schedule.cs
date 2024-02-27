namespace aspnetapp.Models.Schedule
{
    public class Schedule
    {
        public readonly ScheduledDay[] Days;

        public Schedule(ScheduledDay[] _days)
        {
            if (_days is null) throw new ArgumentNullException(nameof(_days));
            else if (_days.Length != 7) throw new ArgumentException($"{nameof(_days)}.length must be 7", nameof(_days));

            Days = _days;
        }
    }
}
