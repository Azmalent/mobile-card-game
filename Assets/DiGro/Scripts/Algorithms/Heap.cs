using System;
using System.Collections.Generic;

namespace DiGro.Algorithms {

    public static class Heap {
        /// <summary>
        /// Создает двоичную кучу (Min Heap) в указанном списке.
        /// O(logN)
        /// </summary>
        public static void Make<T>(ref List<T> data, Func<T, T, int> compareTo) {
            int begin = 0, end = data.Count;
            int size = end - begin;
            if (size < 2)
                return;
            int iMidle = size / 2 - 1;
            for (int midle = begin + iMidle; ; midle--) {
                Make(ref data, begin, midle, end, compareTo);
                if (midle == begin)
                    break;
            }
        }

        private static void Make<T>(ref List<T> data, int begin, int midle, int end, Func<T, T, int> compareTo) {
            int size = end - begin;
            int iMidle = midle - begin;
            if (iMidle > size / 2 - 1)
                return;
            int iLeft = iMidle * 2 + 1;
            int left = begin + iLeft;
            int right = left + 1;
            int max = midle;
            if (compareTo(data[left], data[max]) < 0)
                max = left;

            if (right != end && compareTo(data[right], data[max]) < 0)
                max = right;
            if (max != midle) {
                var t = data[midle];
                data[midle] = data[max];
                data[max] = t;
                Make(ref data, begin, max, end, compareTo);
            }
        }

    }

}