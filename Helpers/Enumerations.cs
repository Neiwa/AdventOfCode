namespace Helpers
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

        /// <summary>
        /// Produces a sequence of results by applying a specified function to each pair of elements in the source
        /// sequence, where the first element in the pair comes before the second.
        /// </summary>
        /// <remarks>This method iterates over the source sequence and applies the <paramref
        /// name="selector"/> function to each pair of elements, ensuring that the first element in the pair always
        /// precedes the second in the sequence. The resulting sequence contains one result for each such
        /// pair.</remarks>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TOut">The type of the elements in the resulting sequence.</typeparam>
        /// <param name="source">The sequence of elements to process. Cannot be <see langword="null"/>.</param>
        /// <param name="selector">A function that takes two elements from the source sequence and returns a result. The first parameter
        /// represents the earlier element in the sequence, and the second parameter represents the later element.</param>
        /// <returns>An <see cref="IEnumerable{TOut}"/> containing the results of applying the <paramref name="selector"/>
        /// function to each pair of elements in the source sequence, where the first element in the pair comes before
        /// the second.</returns>
        public static IEnumerable<TOut> SelfJoin<T, TOut>(this IEnumerable<T> source, Func<T, T, TOut> selector)
        {
            using var e1 = source.GetEnumerator();
            int counter = 0;
            while (e1.MoveNext())
            {
                counter++;
                using var e2 = source.Skip(counter).Select(r => selector(e1.Current, r)).GetEnumerator();
                while (e2.MoveNext())
                {
                    yield return e2.Current;
                }
            }
        }
    }
}
