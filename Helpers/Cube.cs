using System.Diagnostics;

namespace Helpers
{
    [DebuggerDisplay("{FirstCorner}, {SecondCorner}")]
    public class Cube
    {
        public LongPoint3D FirstCorner { get; }

        public LongPoint3D SecondCorner { get; }

        public Cube(LongPoint3D firstCorner, LongPoint3D secondCorner)
        {
            FirstCorner = firstCorner;
            SecondCorner = secondCorner;
        }


    }
}
