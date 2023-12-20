namespace Helpers.Grid
{
    public static class GridExtensions
    {
        public static FixedGrid ToGrid(this string[] lines)
        {
            return new FixedGrid(lines.Select(l => l.ToArray()).ToArray());
        }

        public static FixedGrid ToGrid(this IEnumerable<string> lines)
        {
            return new FixedGrid(lines.Select(l => l.ToArray()).ToArray());
        }

        public static FixedGrid<T> ToGrid<T>(this IEnumerable<string> lines, Func<char, T> transform)
        {
            return new FixedGrid<T>(lines.Select(l => l.Select(transform).ToArray()).ToArray());
        }
    }
}
