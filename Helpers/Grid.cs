using System;
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
    }

    public class Grid<TValue>
    {
        private TValue[][] _grid;

        public Grid(TValue[][] values)
        {
            _grid = values;
        }

        public GridRowReference<TValue> this[int index]
        {
            get
            {
                return new GridRowReference<TValue>(this, index);
            }
        }

        public TValue At(int x, int y)
        {
            return _grid[x][y];
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

        public TValue Value => _grid.At(X, Y);

        public Point Point => new(X, Y);

        public static GridCellReference<TValue> operator +(GridCellReference<TValue> left, Point right)
        { 
            return new GridCellReference<TValue>(left._grid, left.Point + right);
        }

        public static GridCellReference<TValue> operator -(GridCellReference<TValue> left, Point right)
        {
            return new GridCellReference<TValue>(left._grid, left.Point - right);
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
