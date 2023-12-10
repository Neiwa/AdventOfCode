using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class Grid : Grid<char>
    {
        public Grid(char[][] values) : base(values)
        {
        }

        public Grid(int width, int height, char value) : base(width, height, value)
        {
        }
    }

    public class Grid<TValue> : IEnumerable<GridCellReference<TValue>>
    {
        private TValue[][] _grid;

        public int Height { get; init; }
        public int Width { get; init; }

        public Grid(TValue[][] values)
        {
            _grid = values;
            Height = _grid.Length;
            Width = _grid[0].Length;
        }

        public Grid(int width, int height, TValue value)
        {
            Width = width;
            Height = height;

            _grid = Enumerable.Repeat(0, height).Select(_ => Enumerable.Repeat(value, width).ToArray()).ToArray();
        }

        public GridRowReference<TValue> this[int index]
        {
            get
            {
                return new GridRowReference<TValue>(this, index);
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

        public GridCellReference<TValue> At(int x, int y)
        {
            return new GridCellReference<TValue>(this, x, y);
        }

        public GridCellReference<TValue> At(Point point)
        {
            return new GridCellReference<TValue>(this, point);
        }

        public IEnumerator<GridCellReference<TValue>> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return new GridCellReference<TValue>(this, x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class GridRowReference<TValue>
    {
        internal Grid<TValue> _grid;
        private int _row;

        public GridRowReference(Grid<TValue> grid, int row)
        {
            _grid = grid;
            _row = row;
        }

        public GridCellReference<TValue> this[int index]
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

        public int X { get; init; }
        public int Y { get; init; }

        public GridCellReference(Grid<TValue> grid, int x, int y)
        {
            _grid = grid;
            X = x;
            Y = y;
        }
        public GridCellReference(Grid<TValue> grid, Point point)
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

        public IEnumerable<GridCellReference<TValue>> GetSurroundingCells()
        {
            var diffs = new List<Point> {
                new Point(0, -1),
                new Point(1, -1),
                new Point(1, 0),
                new Point(1, 1),
                new Point(0, 1),
                new Point(-1, 1),
                new Point(-1, 0),
                new Point(-1, -1),
            };

            foreach (var diff in diffs)
            {
                var @ref = this + diff;
                if(@ref.IsValid())
                    yield return @ref;
            }
        }

        public IEnumerable<GridCellReference<TValue>> GetConnectedCells()
        {
            var diffs = new List<Point> {
                new Point(0, -1),
                new Point(1, 0),
                new Point(0, 1),
                new Point(-1, 0)
            };

            foreach (var diff in diffs)
            {
                var @ref = this + diff;
                if (@ref.IsValid())
                    yield return @ref;
            }
        }

        public bool IsValid()
        {
            if(X < 0 || Y < 0) return false;
            if(X + 1 > _grid.Width) return false;
            if(Y + 1 > _grid.Height) return false;
            return true;
        }

        public static GridCellReference<TValue> operator +(GridCellReference<TValue> left, GridCellReference<TValue> right)
        {
            return new GridCellReference<TValue>(left._grid, left.Point + right.Point);
        }

        public static GridCellReference<TValue> operator -(GridCellReference<TValue> left, GridCellReference<TValue> right)
        {
            return new GridCellReference<TValue>(left._grid, left.Point - right.Point);
        }

        public static GridCellReference<TValue> operator +(GridCellReference<TValue> left, Point right)
        { 
            return new GridCellReference<TValue>(left._grid, left.Point + right);
        }

        public static GridCellReference<TValue> operator -(GridCellReference<TValue> left, Point right)
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

        public static bool operator ==(GridCellReference<TValue> left, Point right)
        {
            return left.Point == right;
        }

        public static bool operator !=(GridCellReference<TValue> left, Point right)
        {
            return left.Point != right;
        }
    }

    public static class GridExtensions
    {
        public static Grid ToGrid(this string[] lines)
        {    
            return new Grid(lines.Select(l => l.ToArray()).ToArray());
        }

        public static Grid ToGrid(this List<string> lines)
        {
            return new Grid(lines.Select(l => l.ToArray()).ToArray());
        }
    }
}
