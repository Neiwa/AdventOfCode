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
            char[,] grid = new char[lines.Count(), lines.First().Length];
            long x = 0;
            long y = 0;
            foreach (var line in lines)
            {
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
