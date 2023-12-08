using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if(pos == input.Length)
                {
                    pos = 0;
                }
            }
        }
    }
}
