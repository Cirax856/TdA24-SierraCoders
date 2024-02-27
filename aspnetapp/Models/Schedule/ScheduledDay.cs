namespace aspnetapp.Models.Schedule
{
    public struct ScheduledDay
    {
        // Times and names
        public HourInfo[] HourInfos;

        public ScheduledDay(HourInfo[] _hourInfos)
        {
            HourInfos = _hourInfos;
        }
    }
}
