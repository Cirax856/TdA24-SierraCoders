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
            float score = 2f;
            bool found = false;
            int index = 0;

            int i;
            for (i = 0; i < input.Length && index < s.Length; i++)
            {
                if (input[i] == s[index]) // letters are the same
                {
                    score += 2f;
                    index++;
                    found = true;
                } else if (found) // letters arent the same and same letters have already been found
                    score -= 0.75f;
                else
                    score -= 0.3f; // letters arent the same
            }

            score -= 0.2f * (input.Length - index);

            float score2 = 2f;
            for (i = 0; i < s.Length && index < input.Length; i++)
            {
                if (s[i] == input[index]) // letters are the same
                {
                    score2 += 2f;
                    index++;
                    found = true;
                }
                else if (found) // letters arent the same and same letters have already been found
                    score2 -= 0.75f;
                else
                    score2 -= 0.3f; // letters arent the same
            }

            score2 -= 0.2f * (s.Length - index);

            return new RatedString(s, ogIndex, (score + score2) / 2f);
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

            public int CompareTo(RatedString other)
                => -Score.CompareTo(other.Score);

            public override string ToString()
                => "{Value: " + Value + ", Score: " + Score + "}";
        }
    }
}
