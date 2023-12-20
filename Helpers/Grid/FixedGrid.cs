using System.Collections;

namespace Helpers.Grid
{

    public class FixedGrid : FixedGrid<char>
    {
        public FixedGrid(char[][] values) : base(values)
        {
        }

        public FixedGrid(int width, int height, char value) : base(width, height, value)
        {
        }
    }

    public class FixedGrid<TValue> : IEnumerable<FixedGridCellReference<TValue>>
    {
        private TValue[][] _grid;

        public int Height { get; init; }
        public int Width { get; init; }

        public FixedGrid(TValue[][] values)
        {
            _grid = values;
            Height = _grid.Length;
            Width = _grid[0].Length;
        }

        public FixedGrid(int width, int height, TValue value) : this(width, height, _ => value)
        {
        }

        public FixedGrid(int width, int height, Func<Point, TValue> valueFactory)
        {
            Width = width;
            Height = height;

            _grid = Enumerable.Range(0, height).Select(y => Enumerable.Range(0, width).Select(x => valueFactory(new Point(x, y))).ToArray()).ToArray();
        }

        public FixedGridRowReference<TValue> this[int index]
        {
            get
            {
                return new FixedGridRowReference<TValue>(this, index);
            }
        }

        public TValue ValueAt(int x, int y)
        {
            return _grid[y][x];
        }

        public void SetAt(int x, int y, TValue value)
        {
            _grid[y][x] = value;
        }

        public FixedGridCellReference<TValue> At(int x, int y)
        {
            return new FixedGridCellReference<TValue>(this, x, y);
        }

        public FixedGridCellReference<TValue> At(Point point)
        {
            return new FixedGridCellReference<TValue>(this, point);
        }

        public IEnumerable<FixedGridCellReference<TValue>> Iterate(Point direction)
        {
            if (direction.X == 0 ^ direction.Y != 0)
            {
                throw new ArgumentException("Diagonal directions not supported.");
            }

            if (direction.X > 0)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        yield return new FixedGridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.X < 0)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = Width - 1; x >= 0; x--)
                    {
                        yield return new FixedGridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.Y > 0)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        yield return new FixedGridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.Y < 0)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = Height - 1; y >= 0; y--)
                    {
                        yield return new FixedGridCellReference<TValue>(this, x, y);
                    }
                }
            }
        }

        public IEnumerator<FixedGridCellReference<TValue>> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return new FixedGridCellReference<TValue>(this, x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public FixedGrid<TValue> Clone()
        {
            return new FixedGrid<TValue>(_grid.Select(r => (TValue[])r.Clone()).ToArray());
        }
    }

    public class FixedGridRowReference<TValue>
    {
        internal FixedGrid<TValue> _grid;
        private int _row;

        public FixedGridRowReference(FixedGrid<TValue> grid, int row)
        {
            _grid = grid;
            _row = row;
        }

        public FixedGridCellReference<TValue> this[int index]
        {
            get
            {
                return new FixedGridCellReference<TValue>(_grid, _row, index);
            }
        }
    }

    public class FixedGridCellReference<TValue>
    {
        internal FixedGrid<TValue> _grid;

        public int X { get; init; }
        public int Y { get; init; }

        public FixedGridCellReference(FixedGrid<TValue> grid, int x, int y)
        {
            _grid = grid;
            X = x;
            Y = y;
        }
        public FixedGridCellReference(FixedGrid<TValue> grid, Point point)
        {
            _grid = grid;
            X = point.X;
            Y = point.Y;
        }

        public TValue Value
        {
            get => _grid.ValueAt(X, Y);
            set
            {
                _grid.SetAt(X, Y, value);
            }
        }

        public Point Point => new(X, Y);

        public IEnumerable<FixedGridCellReference<TValue>> GetNeighbours(bool includeDiagonal = false)
        {
            List<Point>? shifts;
            if (includeDiagonal)
            {
                shifts = new List<Point>
                {
                    new Point(0, -1),
                    new Point(1, -1),
                    new Point(1, 0),
                    new Point(1, 1),
                    new Point(0, 1),
                    new Point(-1, 1),
                    new Point(-1, 0),
                    new Point(-1, -1),
                };
            }
            else
            {
                shifts = new List<Point>
                {
                    new Point(0, -1),
                    new Point(1, 0),
                    new Point(0, 1),
                    new Point(-1, 0)
                };
            }

            foreach (var shift in shifts)
            {
                var @ref = this + shift;
                if (@ref.IsValid())
                {
                    yield return @ref;
                }
            }
        }

        public bool IsValid()
        {
            if (X < 0 || Y < 0) return false;
            if (X + 1 > _grid.Width) return false;
            if (Y + 1 > _grid.Height) return false;
            return true;
        }

        public static FixedGridCellReference<TValue> operator +(FixedGridCellReference<TValue> left, FixedGridCellReference<TValue> right)
        {
            return new FixedGridCellReference<TValue>(left._grid, left.Point + right.Point);
        }

        public static FixedGridCellReference<TValue> operator -(FixedGridCellReference<TValue> left, FixedGridCellReference<TValue> right)
        {
            return new FixedGridCellReference<TValue>(left._grid, left.Point - right.Point);
        }

        public static FixedGridCellReference<TValue> operator +(FixedGridCellReference<TValue> left, Point right)
        {
            return new FixedGridCellReference<TValue>(left._grid, left.Point + right);
        }

        public static FixedGridCellReference<TValue> operator -(FixedGridCellReference<TValue> left, Point right)
        {
            return new FixedGridCellReference<TValue>(left._grid, left.Point - right);
        }

        public static bool operator ==(FixedGridCellReference<TValue> left, FixedGridCellReference<TValue> right)
        {
            return left.Point == right.Point && left._grid == right._grid;
        }

        public static bool operator !=(FixedGridCellReference<TValue> left, FixedGridCellReference<TValue> right)
        {
            return left.Point != right.Point || left._grid != right._grid;
        }

        public static bool operator ==(FixedGridCellReference<TValue> left, Point right)
        {
            return left.Point == right;
        }

        public static bool operator !=(FixedGridCellReference<TValue> left, Point right)
        {
            return left.Point != right;
        }

        public static implicit operator Point(FixedGridCellReference<TValue> p) => p.Point;
    }
}
