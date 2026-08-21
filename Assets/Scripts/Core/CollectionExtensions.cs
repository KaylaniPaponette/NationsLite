using System.Collections.Generic;

public static class CollectionsExtensions
{
    public static void RemoveUnordered<T>(this IList<T> @this, int i)
    {
        @this[i] = @this[@this.Count - 1];
        @this.RemoveAt(@this.Count - 1);
    }

    public static bool RemoveUnordered<T>(this IList<T> @this, T item)
    {
        int i = @this.IndexOf(item);
        if (i != -1)
        {
            @this.RemoveUnordered(i);
            return true;
        }
        return false;
    }
}
