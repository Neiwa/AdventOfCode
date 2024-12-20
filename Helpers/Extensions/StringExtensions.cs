namespace Helpers.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns all true substrings for a given string.
        /// e.g. abc will return a, ab, b, bc, c
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<string> Substrings(this string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                for (int j = i + 1; j <= value.Length; j++)
                {
                    if (j-i == value.Length)
                    {
                        continue;
                    }

                    yield return value[i..j];
                }
            }
        }

        /// <summary>
        /// Returns all possible slicings for a given string.
        /// e.g. abc will return [a, bc], [a, b, c], [ab, c]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<string>> Slicings(this string value, bool trueSlicings = true)
        {
            if (!trueSlicings)
            {
                yield return [value];
            }

            for (int i = 1; i < value.Length; i++)
            {
                var l = value[..i];
                var r = value[i..].Slicings(false);
                foreach (var rSlices in r)
                {
                    yield return [l, .. rSlices];
                }
            }
        }
    }
}
