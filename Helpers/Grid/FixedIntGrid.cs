using System.Collections;

namespace Helpers.Grid
{

    public class FixedIntGrid : FixedIntGrid<char>
    {
        public FixedIntGrid(char[][] values) : base(values)
        {
        }

        public FixedIntGrid(int width, int height, char value) : base(width, height, value)
        {
        }
    }

    public class FixedIntGrid<TValue> : IEnumerable<FixedIntGridCellReference<TValue>>
    {
        private TValue[][] _grid;

        public int Height { get; init; }
        public int Width { get; init; }

        public FixedIntGrid(TValue[][] values)
        {
            _grid = values;
            Height = _grid.Length;
            Width = _grid[0].Length;
        }

        public FixedIntGrid(int width, int height, TValue value) : this(width, height, _ => value)
        {
        }

        public FixedIntGrid(int width, int height, Func<IntPoint, TValue> valueFactory)
        {
            Width = width;
            Height = height;

            _grid = Enumerable.Range(0, height).Select(y => Enumerable.Range(0, width).Select(x => valueFactory(new IntPoint(x, y))).ToArray()).ToArray();
        }

        public FixedIntGridRowReference<TValue> this[int index]
        {
            get
            {
                return new FixedIntGridRowReference<TValue>(this, index);
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

        public FixedIntGridCellReference<TValue> At(int x, int y)
        {
            return new FixedIntGridCellReference<TValue>(this, x, y);
        }

        public FixedIntGridCellReference<TValue> At(IntPoint point)
        {
            return new FixedIntGridCellReference<TValue>(this, point);
        }

        public IEnumerable<FixedIntGridCellReference<TValue>> Iterate(IntPoint direction)
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
                        yield return new FixedIntGridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.X < 0)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = Width - 1; x >= 0; x--)
                    {
                        yield return new FixedIntGridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.Y > 0)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        yield return new FixedIntGridCellReference<TValue>(this, x, y);
                    }
                }
            }
            else if (direction.Y < 0)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = Height - 1; y >= 0; y--)
                    {
                        yield return new FixedIntGridCellReference<TValue>(this, x, y);
                    }
                }
            }
        }

        public IEnumerator<FixedIntGridCellReference<TValue>> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return new FixedIntGridCellReference<TValue>(this, x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public FixedIntGrid<TValue> Clone()
        {
            return new FixedIntGrid<TValue>(_grid.Select(r => (TValue[])r.Clone()).ToArray());
        }
    }

    public class FixedIntGridRowReference<TValue>
    {
        internal FixedIntGrid<TValue> _grid;
        private int _row;

        public FixedIntGridRowReference(FixedIntGrid<TValue> grid, int row)
        {
            _grid = grid;
            _row = row;
        }

        public FixedIntGridCellReference<TValue> this[int index]
        {
            get
            {
                return new FixedIntGridCellReference<TValue>(_grid, _row, index);
            }
        }
    }

    public class FixedIntGridCellReference<TValue>
    {
        internal FixedIntGrid<TValue> _grid;

        public int X { get; init; }
        public int Y { get; init; }

        public FixedIntGridCellReference(FixedIntGrid<TValue> grid, int x, int y)
        {
            _grid = grid;
            X = x;
            Y = y;
        }
        public FixedIntGridCellReference(FixedIntGrid<TValue> grid, IntPoint point)
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

        public IntPoint Point => new(X, Y);

        public IEnumerable<FixedIntGridCellReference<TValue>> GetNeighbours(bool includeDiagonal = false)
        {
            List<IntPoint>? shifts;
            if (includeDiagonal)
            {
                shifts = new List<IntPoint>
                {
                    new IntPoint(0, -1),
                    new IntPoint(1, -1),
                    new IntPoint(1, 0),
                    new IntPoint(1, 1),
                    new IntPoint(0, 1),
                    new IntPoint(-1, 1),
                    new IntPoint(-1, 0),
                    new IntPoint(-1, -1),
                };
            }
            else
            {
                shifts = new List<IntPoint>
                {
                    new IntPoint(0, -1),
                    new IntPoint(1, 0),
                    new IntPoint(0, 1),
                    new IntPoint(-1, 0)
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

        public static FixedIntGridCellReference<TValue> operator +(FixedIntGridCellReference<TValue> left, FixedIntGridCellReference<TValue> right)
        {
            return new FixedIntGridCellReference<TValue>(left._grid, left.Point + right.Point);
        }

        public static FixedIntGridCellReference<TValue> operator -(FixedIntGridCellReference<TValue> left, FixedIntGridCellReference<TValue> right)
        {
            return new FixedIntGridCellReference<TValue>(left._grid, left.Point - right.Point);
        }

        public static FixedIntGridCellReference<TValue> operator +(FixedIntGridCellReference<TValue> left, IntPoint right)
        {
            return new FixedIntGridCellReference<TValue>(left._grid, left.Point + right);
        }

        public static FixedIntGridCellReference<TValue> operator -(FixedIntGridCellReference<TValue> left, IntPoint right)
        {
            return new FixedIntGridCellReference<TValue>(left._grid, left.Point - right);
        }

        public static bool operator ==(FixedIntGridCellReference<TValue> left, FixedIntGridCellReference<TValue> right)
        {
            return left.Point == right.Point && left._grid == right._grid;
        }

        public static bool operator !=(FixedIntGridCellReference<TValue> left, FixedIntGridCellReference<TValue> right)
        {
            return left.Point != right.Point || left._grid != right._grid;
        }

        public static bool operator ==(FixedIntGridCellReference<TValue> left, IntPoint right)
        {
            return left.Point == right;
        }

        public static bool operator !=(FixedIntGridCellReference<TValue> left, IntPoint right)
        {
            return left.Point != right;
        }

        public static implicit operator IntPoint(FixedIntGridCellReference<TValue> p) => p.Point;
    }
}
