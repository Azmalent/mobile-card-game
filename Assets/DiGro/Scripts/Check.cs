using System;
using UnityEngine;

namespace DiGro {
	public static class Check {

		public static void Index(int index, int end, int begin = 0) {
			if (index < begin || index >= end)
				throw new IndexOutOfRangeException();
		}

        public static void CheckComponent<T>(GameObject obj) {
            if (!obj)
                throw new Exception("Check.CheckComponent: Null object.");

            T component = obj.GetComponent<T>();
            if (component == null)
                throw new Exception("Check.CheckComponent: " + typeof(T) + " component missing.");
        }

        public static void NotNull(object obj)
        {
            if (obj == null)
                throw new Exception("Check.NotNull: Null object.");
        }

    }
}