using System.Collections.Generic;

namespace CodeBuilder.Helpers
{
    public static class CollectionHelper
    {
        public static bool Contains<T>(IEnumerable<T> array, T value)
        {
            foreach (var v in array)
                if (Equals(v, value))
                    return true;
            return false;
        }
    }
}
