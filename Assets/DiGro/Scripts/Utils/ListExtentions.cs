using System;
using System.Collections.Generic;

namespace DiGro.Utils
{
    public static class ListExtentions
    {

        public static string ToString<T>(this IEnumerable<T> enumerable, string sep = " ", string begin = "", string end = "")
        {
            string str = begin;
            var enumerator = enumerable.GetEnumerator();
            bool hasNext = enumerator.MoveNext();
            while (hasNext)
            {
                str += enumerator.Current.ToString();
                hasNext = enumerator.MoveNext();
                if (hasNext)
                    str += sep;
            }
            return str + end;
        }

    }
}