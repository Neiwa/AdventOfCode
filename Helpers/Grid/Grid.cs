using Helpers.Grid;
using System.Collections;

namespace Helpers.Grid
{
    public abstract class Grid<TValue> : IEnumerable<GridCellReference<TValue>>
    {
        public virtual GridRowReference<TValue> this[long index]
        {
            get
            {
                return new GridRowReference<TValue>(this, index);
            }
        }

        public abstract long Width { get; }
        public abstract long Height { get; }

        public abstract IEnumerator<GridCellReference<TValue>> GetEnumerator();
        public virtual void SetAt(long x, long y, TValue value)
        {
            SetAt(new LongPoint(x, y), value);
        }
        public abstract void SetAt(LongPoint pos, TValue value);
        public virtual TValue ValueAt(long x, long y)
        {
            return ValueAt(new LongPoint(x, y));
        }
        public abstract TValue ValueAt(LongPoint pos);

        public virtual bool Valid(LongPoint pos)
        {
            return Valid(pos.X, pos.Y);
        }
        public abstract bool Valid(long x, long y);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

namespace Helpers
{
    public class GridRowReference<TValue>
    {
        internal Grid<TValue> _grid;
        private long _row;

        public GridRowReference(Grid<TValue> grid, long row)
        {
            _grid = grid;
            _row = row;
        }

        public GridCellReference<TValue> this[long index]
        {
            get
            {
                return new GridCellReference<TValue>(_grid, _row, index);
            }
        }
    }

    public class GridCellReference<TValue>
    {
        internal Grid<TValue> _grid;

        public long X { get; init; }
        public long Y { get; init; }

        public GridCellReference(Grid<TValue> grid, long x, long y)
        {
            _grid = grid;
            X = x;
            Y = y;
        }
        public GridCellReference(Grid<TValue> grid, LongPoint point)
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

        public LongPoint Point => new(X, Y);

        public IEnumerable<GridCellReference<TValue>> GetNeighbours(bool includeDiagonal = false)
        {
            var shifts = new List<LongPoint>();
            if (includeDiagonal)
            {
                shifts = new List<LongPoint>
                {
                    new (0, -1),
                    new (1, -1),
                    new (1, 0),
                    new (1, 1),
                    new (0, 1),
                    new (-1, 1),
                    new (-1, 0),
                    new (-1, -1),
                };
            }
            else
            {
                shifts = new List<LongPoint>
                {
                    new (0, -1),
                    new (1, 0),
                    new (0, 1),
                    new (-1, 0)
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
            return _grid.Valid(X, Y);
        }

        public static GridCellReference<TValue> operator +(GridCellReference<TValue> left, GridCellReference<TValue> right)
        {
            return new GridCellReference<TValue>(left._grid, left.Point + right.Point);
        }

        public static GridCellReference<TValue> operator -(GridCellReference<TValue> left, GridCellReference<TValue> right)
        {
            return new GridCellReference<TValue>(left._grid, left.Point - right.Point);
        }

        public static GridCellReference<TValue> operator +(GridCellReference<TValue> left, LongPoint right)
        {
            return new GridCellReference<TValue>(left._grid, left.Point + right);
        }

        public static GridCellReference<TValue> operator -(GridCellReference<TValue> left, LongPoint right)
        {
            return new GridCellReference<TValue>(left._grid, left.Point - right);
        }

        public static bool operator ==(GridCellReference<TValue> left, GridCellReference<TValue> right)
        {
            return left.Point == right.Point && left._grid == right._grid;
        }

        public static bool operator !=(GridCellReference<TValue> left, GridCellReference<TValue> right)
        {
            return left.Point != right.Point || left._grid != right._grid;
        }

        public static bool operator ==(GridCellReference<TValue> left, LongPoint right)
        {
            return left.Point == right;
        }

        public static bool operator !=(GridCellReference<TValue> left, LongPoint right)
        {
            return left.Point != right;
        }

        public static implicit operator LongPoint(GridCellReference<TValue> p) => p.Point;
    }
}