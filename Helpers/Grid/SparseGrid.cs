namespace Helpers.Grid
{
    public class SparseGrid : SparseGrid<char>
    {
        public SparseGrid(char defaultValue) : base(defaultValue)
        {

        }
    }

    public class SparseGrid<TValue> : Grid<TValue>
    {
        private Dictionary<Point, TValue> _grid = new();
        private long minX, minY, maxX, maxY;
        private readonly Func<Point, TValue> _valueFactory;

        public long MinX => minX;
        public long MaxX => maxX;
        public long MinY => minY;
        public long MaxY => maxY;

        public override long Width => maxX - minX;
        public override long Height => maxY - minY;

        public SparseGrid(TValue defaultValue) : this(_ => defaultValue)
        {
        }

        public SparseGrid(Func<Point, TValue> valueFactory)
        {
            _valueFactory = valueFactory;
        }

        public override void SetAt(Point pos, TValue value)
        {
            _grid[pos] = value;

            if (minX > pos.X)
            {
                minX = pos.X;
            }
            else if (maxX < pos.X)
            {
                maxX = pos.X;
            }

            if (minY > pos.Y)
            {
                minY = pos.Y;
            }
            else if (maxY < pos.Y)
            {
                maxY = pos.Y;
            }
        }

        public override TValue ValueAt(Point pos)
        {
            if (_grid.TryGetValue(pos, out var value))
            {
                return value;
            }
            return _valueFactory(pos);
        }

        public override IEnumerator<GridCellReference<TValue>> GetEnumerator()
        {
            for (long y = minY; y < maxY; y++)
            {
                for (long x = minX; x < maxX; x++)
                {
                    yield return new GridCellReference<TValue>(this, x, y);
                }
            }
        }

        public override bool Valid(long x, long y)
        {
            return x >= minX && x <= maxX && y >= minY && y <= maxY;
        }
    }
}
