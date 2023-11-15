namespace aspnetapp
{
    public static class Searcher
    {
        public static RatedString[] Search<T>(T[] array, Func<T, string> getValue, string input)
        {
            RatedString[] ratedArray = new RatedString[array.Length];
            for (int i = 0; i < array.Length; i++)
                ratedArray[i] = Rate(getValue(array[i]), i, input);

            Array.Sort(ratedArray);

            return ratedArray;
        }

        public static RatedString Rate(string s, int ogIndex, string input)
        {
            float score = 2.5f;
            bool found = false;
            int index = 0;

            int i;
            for (i = 0; i < input.Length && index < s.Length; i++)
            {
                if (input[i] == s[index])
                {
                    score += 2f;
                    index++;
                    found = true;
                } else if (found)
                    score -= 1f;
                else
                    score -= 0.3f;
            }

            score -= 0.2f * (input.Length - index);

            return new RatedString(s, ogIndex, score);
        }

        public struct RatedString : IComparable<RatedString>
        {
            public string Value;
            public int OgIndex;
            public float Score;

            public RatedString(string _value, int _ogIndex, float _score)
            {
                Value = _value;
                OgIndex = _ogIndex;
                Score = _score;
            }

            public int CompareTo(RatedString other) => Score.CompareTo(other.Score);
        }
    }
}
