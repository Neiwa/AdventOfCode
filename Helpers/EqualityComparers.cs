using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;
public static class EqualityComparers
{
    public static readonly EqualityComparer<int[]> IntArrayEqualityComparer = EqualityComparer<int[]>.Create((x, y) =>
    {
        if ((x == null && y == null) || ReferenceEquals(x, y))
        {
            return true;
        }
        if (x == null || y == null)
        {
            return false;
        }

        return x.SequenceEqual(y);
    }, arr =>
    {
        int hash = arr.Length;
        foreach (var val in arr)
        {
            hash = unchecked(hash * 314159 + val);
        }
        return hash;
    });
}
