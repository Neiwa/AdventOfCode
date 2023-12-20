namespace Helpers.Grid
{
    public class FixedGrid : FixedGrid<char>
    {
        public FixedGrid(char[,] values) : base(values)
        {
        }

        public FixedGrid(int width, int height, char value) : base(width, height, value)
        {
        }
    }

    public class FixedGrid<TValue> : Grid<TValue>
    {
        private readonly TValue[,] _grid;

        public override long Height => _grid.GetLongLength(1);
        public override long Width => _grid.GetLongLength(0);

        public FixedGrid(TValue[,] values)
        {
            _grid = values;
        }

        public FixedGrid(long width, long height, TValue value) : this(width, height, _ => value)
        {
        }

        public FixedGrid(long width, long height, Func<Point, TValue> valueFactory)
        {
            _grid = new TValue[height, width];
            for (long y = 0; y < Height; y++)
            {
                for (long x = 0; x < Width; x++)
                {
                    _grid[y, x] = valueFactory(new Point(x, y));
                }
            }
        }

        public override TValue ValueAt(Point pos)
        {
            return _grid[pos.Y, pos.X];
        }

        public override void SetAt(Point pos, TValue value)
        {
            _grid[pos.Y, pos.X] = value;
        }

        public IEnumerable<GridCellReference<TValue>> Iterate(Point direction)
        {
            if (direction.X == 0 ^ direction.Y != 0)
            {
                throw new ArgumentException("Diagonal directions not supported.");
            }

            if (direction.X > 0)
            {
                for (long y = 0; y < Height; y++)
                {
                    for (long x = 0; x < Width; x++)
                    {
                        yield return new GridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.X < 0)
            {
                for (long y = 0; y < Height; y++)
                {
                    for (long x = Width - 1; x >= 0; x--)
                    {
                        yield return new GridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.Y > 0)
            {
                for (long x = 0; x < Width; x++)
                {
                    for (long y = 0; y < Height; y++)
                    {
                        yield return new GridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.Y < 0)
            {
                for (long x = 0; x < Width; x++)
                {
                    for (long y = Height - 1; y >= 0; y--)
                    {
                        yield return new GridCellReference<TValue>(this, x, y);
                    }
                }
            }
        }

        public override IEnumerator<GridCellReference<TValue>> GetEnumerator()
        {
            for (long y = 0; y < Height; y++)
            {
                for (long x = 0; x < Width; x++)
                {
                    yield return new GridCellReference<TValue>(this, x, y);
                }
            }
        }

        public FixedGrid<TValue> Clone()
        {
            return new FixedGrid<TValue>((TValue[,])_grid.Clone());
        }

        public override bool Valid(long x, long y)
        {
            return 0 <= x && x < Width && 0 <= y && y < Height;
        }
    }
}
