using System.Collections.Generic;
public class IntArrayEqualityComparer : IEqualityComparer<int[]>
{
    public bool Equals(int[] x, int[] y)
    {
        if (x == null || y == null)
        {
            return x == y;
        }

        if (x.Length != y.Length)
        {
            return false;
        }

        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(int[] obj)
    {
        unchecked
        {
            int hash = 17;
            foreach (int i in obj)
            {
                hash = hash * 23 + i.GetHashCode();
            }

            return hash;
        }
    }
}