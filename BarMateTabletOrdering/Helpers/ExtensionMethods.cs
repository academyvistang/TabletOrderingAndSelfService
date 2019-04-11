using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarMateTabletOrdering.Helpers
{
    public static class ExtensionMethods
    {
        public static string ToDelimitedString(this IEnumerable<string> list, string delimiter)
        {
            return list == null ? string.Empty : string.Join(delimiter, list.ToArray());
        }
    }
}