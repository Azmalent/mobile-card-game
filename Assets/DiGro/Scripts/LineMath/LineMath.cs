using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiGro
{
	public static class LineMath
	{

		/**
		 * Принимает начальную и конечную координату линии.
		 * Возвращает среднюю точку линии.
		**/
		public static Vector2 MidLinePoint(Vector2 a, Vector2 b)
		{
			return (a + b) / 2;
		}

		/**
		 * Принимает точку, начальную и конечную координату линии.
		 * Возвращает ближайшую точку на линии к заданной точке.
		**/
		public static Vector2 ClosestPointOnLine(Vector2 p0, Vector2 p1, Vector2 p2)
		{
			Vector2 lineVec = p2 - p1;
			Vector2 pointVec = p0 - p1;

			Vector2 nLineVec = lineVec.normalized;
			float dot = Vector2.Dot(pointVec, nLineVec);
			Vector2 p = p1 + dot * nLineVec;

			return p;
		}

		/**
		 * Принимает точку, начальную и конечную точку отрезка.
		 * Возвращает точку на отрезке, ближайшую к заданной точке.
		**/
		public static Vector2 ClosestPointOnLineSegment(Vector2 p0, Vector2 p1, Vector2 p2)
		{
			float dot = Vector2.Dot(p0 - p2, p1 - p2);
			if (dot < 0)
				return p2;

			Vector2 lineVec = p2 - p1;
			Vector2 pointVec = p0 - p1;
			dot = Vector2.Dot(pointVec, lineVec);
			if (dot < 0)
				return p1;

			Vector2 nLineVec = lineVec.normalized;
			dot = Vector2.Dot(pointVec, nLineVec);
			Vector2 p = p1 + dot * nLineVec;

			return p;
		}

		/// <summary>
		///  Принимает точку, начальную и конечную координату линии.
		///  Возвращает косое произведение между заданными линиями и точкой.
		///  result < 0 - точка лежит под линией (справа)
		///  result > 0 - точка лежит над линией (слева)
		///  result = 0 - точка лежит на линии
		/// </summary>
		public static float PseudoDotProduct(Vector2 p0, Vector2 p1, Vector2 p2)
		{
			return (p2.x - p1.x) * (p0.y - p1.y) - (p2.y - p1.y) * (p0.x - p1.x);
		}

		/**
		 * Принимает точку и две точки на прямой.
		 * Возвращает растояние от точки до прямой.
		**/
		public static float DistanceToLine(Vector2 p0, Vector2 p1, Vector2 p2)
		{
			float dx = (float)Math.Pow(p2.x - p1.x, 2);
			float dy = (float)Math.Pow(p2.y - p1.y, 2);
			return Math.Abs(PseudoDotProduct(p0, p1, p2)) / (float)Math.Sqrt(dx + dy);
		}

		/**
		 * Принимает точку, начальную и конечную точку отрезка.
		 * Возвращает минимальное растояние между заданной точкой и отрезком.
		**/
		public static float DistanceToLineSegment(Vector2 p0, Vector2 p1, Vector2 p2)
		{
			return (ClosestPointOnLineSegment(p0, p1, p2) - p0).magnitude;
		}

		/**
		 * Ищет точку пересечения заданных прямых.
		 * Возвращает true или false, если пересечение есть или нет, соответственно.
		**/
		public static bool Intersection(Vector2 p11, Vector2 p12, Vector2 p21, Vector2 p22, out Vector2 intersect)
		{
			intersect = Vector2.zero;
			float dx1 = p11.x - p12.x;
			float dx2 = p21.x - p22.x;
			float dy1 = p11.y - p12.y;
			float dy2 = p21.y - p22.y;
			float d = dx1 * dy2 - dx2 * dy1;
			if (d > 0 || d < 0)
			{
				float x1 = p11.x * p12.y - p11.y * p12.x;
				float x2 = p21.x * p22.y - p21.y * p22.x;
				float x = x1 * dx2 - x2 * dx1;
				float y = x1 * dy2 - x2 * dy1;
				intersect = new Vector2(x / d, y / d);
				return true;
			}
			return false;
		}

		/**
		 * Ищет точку пересечения между заданной прямой и отрезком.
		 * Возвращает true или false, если пересечение есть или нет, соответственно.
		**/
		public static bool IntersectionLineWithSegment(Vector2 p11, Vector2 p12, Vector2 s21, Vector2 s22, out Vector2 intersect)
		{
			intersect = Vector2.zero;
			if (Intersection(p11, p12, s21, s22, out intersect))
			{
				float dot = Vector2.Dot(intersect - s21, s22 - s21);
				if (dot < 0)
					return false;
				dot = Vector2.Dot(intersect - s22, s21 - s22);
				if (dot < 0)
					return false;
				return true;
			}
			return false;
		}

		public static bool IntersectionLineSegments(Vector2 s11, Vector2 s12, Vector2 s21, Vector2 s22, out Vector2 intersect)
		{
			intersect = Vector2.zero;
			if (Intersection(s11, s12, s21, s22, out intersect))
			{
				float dot = Vector2.Dot(intersect - s11, s12 - s11);
				if (dot < 0)
					return false;
				dot = Vector2.Dot(intersect - s12, s11 - s12);
				if (dot < 0)
					return false;

				dot = Vector2.Dot(intersect - s21, s22 - s21);
				if (dot < 0)
					return false;
				dot = Vector2.Dot(intersect - s22, s21 - s22);
				if (dot < 0)
					return false;

				return true;
			}
			return false;
		}


		/**
		 * Определяет находится ли точка в пределах линии, в т.ч. сверху или снизу.
		 * **/
		public static bool IsClamped(Vector2 p, Vector2 s1, Vector2 s2)
		{
			float dot = Vector2.Dot(p - s1, s2 - s1);
			if (dot < 0)
				return false;
			dot = Vector2.Dot(p - s2, s1 - s2);
			if (dot < 0)
				return false;
			return true;
		}

		/**
		 * Определяет пересекаются ли заданные отрезки.
		 * **/
		public static bool IsIntersectedSegments(Vector2 p1, Vector2 p2, Vector2 m1, Vector2 m2)
		{
			float a = PseudoDotProduct(m2, p1, p2);
			float b = PseudoDotProduct(m1, p1, p2);
			if (a * b >= 0)
			{
				Debug.Log("IS INTERSECTED: false");
				return false;
			}
			a = PseudoDotProduct(p2, m1, m2);
			b = PseudoDotProduct(p1, m1, m2);
			if (a * b >= 0)
			{
				Debug.Log("IS INTERSECTED: false");
				return false;
			}

			Debug.Log("IS INTERSECTED: true");
			return true;
		}


	}
}