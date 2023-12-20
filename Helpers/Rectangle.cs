namespace Helpers
{
    public class Rectangle
    {
        public Rectangle(Point position, int width) : this(position, width, width)
        {
        }
        public Rectangle(Point position, Point bottomRight) : this(position, bottomRight.X - position.X + 1, bottomRight.Y - position.Y + 1)
        {
        }
        public Rectangle(Point position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        public Rectangle(int x, int y, int width) : this(x, y, width, width)
        {
        }

        public Rectangle(int x, int y, int width, int height) : this(new Point(x, y), width, height)
        {
        }

        public Point Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Left
        {
            get
            {
                return Position.X;
            }
        }
        public int Right
        {
            get
            {
                return Position.X + Width;
            }
        }
        public int Top
        {
            get
            {
                return Position.Y;
            }
        }
        public int Bottom
        {
            get
            {
                return Position.Y + Height;
            }
        }

        public bool Contains(Point point)
        {
            return point.X >= Position.X &&
                point.X <= Position.X + Width - 1 &&
                point.Y >= Position.Y &&
                point.Y <= Position.Y + Height - 1;
        }
    }
}
