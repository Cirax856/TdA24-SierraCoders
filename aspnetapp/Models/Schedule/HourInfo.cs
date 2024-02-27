namespace aspnetapp.Models.Schedule
{
    public struct HourInfo
    {
        public Subject Subject;
		public AbsoluteTimeSpan Time;

        public HourInfo(Subject _subject, AbsoluteTimeSpan _time)
        {
            Subject = _subject;
            Time = _time;
        }
    }
}
