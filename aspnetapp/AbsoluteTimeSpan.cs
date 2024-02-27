namespace aspnetapp
{
    public readonly struct AbsoluteTimeSpan
    {
        public readonly TimeOnly From;
        public readonly TimeOnly To;

        public TimeSpan Duration => To - From;

        public AbsoluteTimeSpan(TimeOnly _from, TimeOnly _to)
        {
            if (_from > _to)
                throw new ArgumentException($"{nameof(_from)} must be smaller than {nameof(_to)}");

            From = _from;
            To = _to;
        }
        public AbsoluteTimeSpan(TimeOnly _from, TimeSpan _duration)
            : this(_from, _from.Add(_duration))
        { }

        public override string ToString()
            => $"{{ From: {From}, To: {To}, Duration: {Duration} }}";
    }
}
