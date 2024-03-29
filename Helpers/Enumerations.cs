﻿namespace Helpers
{
    public static class Enumerations
    {
        public static IEnumerable<int> Range(int start, int increment, int count)
        {
            return Range(start, v => v + increment, count);
        }
        public static IEnumerable<long> Range(long start, long increment, long count)
        {
            return Range(start, v => v + increment, count);
        }

        public static IEnumerable<T> Range<T>(T start, Func<T, T> increment, long count)
        {
            for (long i = 0; i < count; i++)
            {
                yield return start;
                start = increment(start);
            }
        }

        public static IEnumerable<T> Enumeration<T>(params T[] values)
        {
            return values;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T value)
        {
            return Enumerable.Empty<T>().Append(value);
        }

        public static IEnumerable<HashSet<T>> GetAllSubSets<T>(IEnumerable<T> set)
        {
            if (!set.Any())
            {
                yield return new HashSet<T>();
                yield break;
            }
            T? first = set.First();

            foreach (var subSet in GetAllSubSets(set.Skip(1)))
            {
                yield return subSet;
                var nSubSet = new HashSet<T>(subSet);
                nSubSet.Add(first);

                yield return nSubSet;
            }
        }

        public static IEnumerable<char> CircularCharFeed(string input)
        {
            int pos = 0;
            while (true)
            {
                yield return input[pos++];
                if (pos == input.Length)
                {
                    pos = 0;
                }
            }
        }

        public static IEnumerable<int> AllIndexesOf(this string str, string searchstring, bool avoidOverlap = true)
        {
            int increment = avoidOverlap ? searchstring.Length : 1;
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + increment);
            }
        }

        public static TOut? MaxOrDefault<T, TOut>(this IEnumerable<T> source, Func<T, TOut> selector, TOut defaultValue = default)
        {
            return source.Any() ? source.Max(selector) : defaultValue;
        }

        public static TOut? MinOrDefault<T, TOut>(this IEnumerable<T> source, Func<T, TOut> selector, TOut defaultValue = default)
        {
            return source.Any() ? source.Min(selector) : defaultValue;
        }
    }
}
