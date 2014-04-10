using System.Collections.Generic;
using System.Text;

namespace CodeBuilder.Helpers
{
    public static class StringFormat
    {
        public static string Join(IEnumerable<object> objects, string separator)
        {
            var sb = new StringBuilder();
            foreach (var o in objects)
            {
                if (sb.Length > 0)
                    sb.Append(separator);
                sb.Append(o);
            }
            return sb.ToString();
        }
    }
}
