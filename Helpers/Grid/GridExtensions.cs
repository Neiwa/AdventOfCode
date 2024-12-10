namespace Helpers.Grid
{
    public static class GridExtensions
    {
        public static FixedIntGrid ToFixedIntGrid(this string[] lines)
        {
            return new FixedIntGrid(lines.Select(l => l.ToArray()).ToArray());
        }

        public static FixedIntGrid ToFixedIntGrid(this IEnumerable<string> lines)
        {
            return new FixedIntGrid(lines.Select(l => l.ToArray()).ToArray());
        }

        public static FixedIntGrid<T> ToFixedIntGrid<T>(this IEnumerable<string> lines, Func<char, T> transform)
        {
            return new FixedIntGrid<T>(lines.Select(l => l.Select(transform).ToArray()).ToArray());
        }

        public static Grid<char> ToGrid(this IEnumerable<string> lines)
        {
            return ToFixedGrid(lines);
        }

        public static FixedGrid<T> ToFixedGrid<T>(this IEnumerable<string> lines, Func<char, T> transform)
        {
            T[,] grid = new T[lines.Count(), lines.First().Length];
            long y = 0;
            foreach (var line in lines)
            {
                long x = 0;
                foreach (var c in line)
                {
                    grid[y, x++] = transform(c);
                }
                y++;
            }
            return new FixedGrid<T>(grid);
        }

        public static FixedGrid<char> ToFixedGrid(this IEnumerable<string> lines)
        {
            char[,] grid = new char[lines.Count(), lines.First().Length];
            long y = 0;
            foreach (var line in lines)
            {
                long x = 0;
                foreach (var c in line)
                {
                    grid[y, x++] = c;
                }
                y++;
            }
            return new FixedGrid(grid);
        }
    }
}
