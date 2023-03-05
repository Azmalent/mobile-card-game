using System;
using System.Collections.Generic;

using UnityEngine;


namespace DiGro.MeshGenerator
{
    // s - source point: s1, s2
    // p - actual point: p1, p2
    // i - point index:  i1, i2
    // seg - segment

    class LineMeshGenerator2D : IMeshGenerator
    {

        public enum Axis { XY, XZ }

        public class Parameters
        {
            public List<Vector3> points = null;
            public Axis axis = Axis.XY;

            public float width          = 1;
            public float outerRadius    = 1;
            public float innerRadius    = 1;
            public int   ringSegments   = 36; 
            public bool  loop           = false;
        }


        public List<Vector3> Points     { get; private set; }
        public List<Vector3> Normals    { get; private set; }
        public List<Vector2> Uv         { get; private set; }
        public List<int>     Triangles  { get; private set; }


        private Parameters Args;


        public LineMeshGenerator2D(Parameters args)
        {
            Args = args;

            if(Args.points == null)
                throw new ArgumentNullException();

            if (Args.points.Count < 2)
                throw new ArgumentException("The line must contain more then one point.");

            else if(Args.points.Count == 2 && Args.points[0] == Args.points[1])
                throw new ArgumentException("The line must contain two different points.");

            if(Args.axis == Axis.XZ)
            {
                var tmp = new List<Vector3>(Args.points.Count);
                foreach(var point in Args.points)
                    tmp.Add(new Vector3(point.x, point.z, point.y));
                Args.points = tmp;
            }

            Points = new List<Vector3>();
            Normals = new List<Vector3>();
            Uv = new List<Vector2>();
            Triangles = new List<int>();

            for (int i = 0; i < Args.points.Count; i++)
            {
                var pos = Args.points[i];
                pos.z = 0;
                Args.points[i] = pos;
            }
        }


        public void Generate()
        {
            if (Args.points.Count == 2)
            {
                var p1 = Args.points[0];
                var p2 = Args.points[1];
                float leftSideLength = (p2 - p1).magnitude;
                var rightSideLength = leftSideLength;
                SegmentReverseConnect(ref p1, ref p2, ref leftSideLength, first: true);
            }
            else
            {
                Connect();
            }

            foreach(var point in Points)
            {
                Normals.Add(Args.axis == Axis.XY ? Vector3.forward : Vector3.up);
                //Uv.Add(point);
            }

            if (Args.axis == Axis.XZ)
            {
                var tmp = new List<Vector3>(Points.Count);
                foreach (var point in Points)
                    tmp.Add(new Vector3(point.x, point.z, point.y));
                Points = tmp;
            }
        }

        
        private void Connect()
        {
            float length = 0;
            for (int i = 0; i < Args.points.Count - 2; i++)
            {
                bool first = i == 0;
                var s1 = Args.points[i];
                var s2 = Args.points[i + 1];
                var s3 = Args.points[i + 2];

                var d = LineMath.PseudoDotProduct(s3, s1, s2);
                if (d == 0)
                    SegmentConnect(ref s1, ref s3, ref length, first: first);
                else if (d > 0)
                    LhsConnect(ref s1, ref s2, ref s3, ref length, first: first);
                else
                    RhsConnect(ref s1, ref s2, ref s3, ref length, first: first);
            }
            var p4 = Args.points[Args.points.Count - 2];
            var p5 = Args.points[Args.points.Count - 1];
            SegmentConnect(ref p4, ref p5, ref length, last: true);
        }

        private void ReverseConnect()
        {
            float length = 0;
            for (int i = Args.points.Count - 1; i > 1; i--)
            {
                bool first = i == Args.points.Count - 1;
                var s1 = Args.points[i - 2];
                var s2 = Args.points[i - 1];
                var s3 = Args.points[i];
                var p3 = first ? s3 : Points[Points.Count - 2];

                var d = LineMath.PseudoDotProduct(p3, s1, s2);
                if (d == 0)
                    SegmentReverseConnect(ref s1, ref s3, ref length, first: first);
                else if (d > 0)
                    LhsReverseConnect(ref s1, ref s2, ref s3, ref length, first: first);
                else
                    RhsReverseConnect(ref s1, ref s2, ref s3, ref length, first: first);
            }
            var p4 = Args.points[0];
            var p5 = Args.points[1];
            SegmentReverseConnect(ref p4, ref p5, ref length, last: true);
        }

        private Vector3[] GetSegmentPoints(ref Vector3 p1, ref Vector3 p2)
        {
            Vector3 perp = -Vector2.Perpendicular(p2 - p1).normalized;
            var res = new Vector3[4];
            res[0] = p1;
            res[1] = p1 + perp * Args.width; // p1'
            res[2] = p2;
            res[3] = p2 + perp * Args.width; // p2'
            return res;
        }

        public void SegmentConnect(ref Vector3 s1, ref Vector3 s2, ref float leftSidelength, bool first = false, bool last = false)
        {
            int i0 = first ? 0 : Points.Count - 2;
            var p0 = first ? s1 : Points[i0];
            // Взять фактическую точку
            var seg = GetSegmentPoints(ref p0, ref s2);

            int i1 = first ? 1 : Points.Count - 1;
            int i2 = i1 + 1;
            int i3 = i2 + 1;

            if (first)
            {
                Points.Add(seg[0]);
                Points.Add(seg[1]);
                Uv.Add(new Vector2(0, 0));
                Uv.Add(new Vector2(1, 0));
            }
            if (last)
            {
                Points.Add(seg[2]);
                Points.Add(seg[3]);

                var length = (Points[i2] - Points[i0]).magnitude;
                leftSidelength += length;

                Uv.Add(new Vector2(0, leftSidelength));
                Uv.Add(new Vector2(1, leftSidelength));

                Triangles.AddRange(new int[] {
                    i0, i2, i1,
                    i1, i2, i3
                });
            }
        }

        public void SegmentReverseConnect(ref Vector3 p1, ref Vector3 p2, ref float leftSidelength, bool first = false, bool last = false)
        {
            var points = GetSegmentPoints(ref p1, ref p2);
            int p2i = first ? 0 : Points.Count - 2;
            int p3i = p2i + 1;
            int p0i = p3i + 1;
            int p1i = p0i + 1;

            if (first)
            {
                Points.Add(points[2]);
                Points.Add(points[3]);
                Uv.Add(new Vector2(0, 0));
                Uv.Add(new Vector2(1, 0));
            }
            if (last)
            {
                Points.Add(points[0]);
                Points.Add(points[1]);

                var length = (Points[p2i] - Points[p0i]).magnitude;
                leftSidelength += length;

                Uv.Add(new Vector2(0, leftSidelength));
                Uv.Add(new Vector2(1, leftSidelength));

                Triangles.AddRange(new int[] {
                    p2i, p3i, p1i,
                    p1i, p0i, p2i
                });
            }
        }

        private void RhsConnect(ref Vector3 s1, ref Vector3 s2, ref Vector3 s3, ref float leftSidelength, bool first = false)
        {
            var seg1 = GetSegmentPoints(ref s1, ref s2);
            var seg2 = GetSegmentPoints(ref s2, ref s3);

            Vector2 intersectPoint;
            if (!LineMath.Intersection(seg1[1], seg1[3], seg2[1], seg2[3], out intersectPoint))
                throw new Exception("Not have intersections.");

            var p3 = (Vector3)intersectPoint;
            var p2 = (Vector3)LineMath.ClosestPointOnLine(p3, s1, s2);
            var p5 = (Vector3)LineMath.ClosestPointOnLine(p3, s2, s3);
            var p4 = p3 + (s2 - p3).normalized * Args.width;

            var delta = s2 - p4;
            p2 += delta;
            p3 += delta;
            p4 += delta;
            p5 += delta;

            int p0i = first ? 0 : Points.Count - 2;
            int p1i = first ? 1 : Points.Count - 1;
            int p2i, p3i, p31i, p4i, p5i;

            if (first)
            {
                Points.Add(s1);      // p0
                Points.Add(seg1[1]); // p1
                Uv.Add(new Vector2(0, 0));
                Uv.Add(new Vector2(1, 0));
            }
            p2i = Points.Count;
            Points.Add(p2); // p2

            p31i = Points.Count;
            Points.Add(p3); // p3.1

            p4i = Points.Count;
            Points.Add(p4); // p4

            p5i = Points.Count;
            Points.Add(p5); // p5

            p3i = Points.Count;
            Points.Add(p3); // p3

            leftSidelength += (Points[p2i] - Points[p0i]).magnitude;
            Uv.Add(new Vector2(0, leftSidelength)); // p2
            Uv.Add(new Vector2(1, leftSidelength)); // p3.1

            leftSidelength += (p4 - p2).magnitude;
            Uv.Add(new Vector2(0, leftSidelength)); // p4

            leftSidelength += (p5 - p4).magnitude;
            Uv.Add(new Vector2(0, leftSidelength)); // p5
            Uv.Add(new Vector2(1, leftSidelength)); // p3

            Triangles.AddRange(new int[] {
                p0i, p2i, p1i,
                p1i, p2i, p31i,
                p31i, p2i, p4i,
                p4i, p5i, p3i,
            });
        }

        private void RhsReverseConnect(ref Vector3 s1, ref Vector3 s2, ref Vector3 s3, ref float leftSidelength, bool first = false)
        {
            Debug.Log("RHS");
            var seg1 = GetSegmentPoints(ref s1, ref s2);
            var seg2 = GetSegmentPoints(ref s2, ref s3);

            Vector2 intersectPoint;
            if (!LineMath.Intersection(seg1[1], seg1[3], seg2[1], seg2[3], out intersectPoint))
                throw new Exception("Not have intersections.");

            int i6 = first ? 0 : Points.Count - 2;
            int i7 = i6 + 1;

            var p3 = (Vector3)intersectPoint;
            var p2 = (Vector3)LineMath.ClosestPointOnLine(p3, s1, s2);
            var p5 = (Vector3)LineMath.ClosestPointOnLine(p3, s2, first ? s3 : Points[i6]);
            var p4 = p3 + (s2 - p3).normalized * Args.width;

            var delta = s2 - p4;
            p2 += delta;
            p3 += delta;
            p4 += delta;
            p5 += delta;

            int i2, i3, i31, i4, i5;

            if (first)
            {
                Points.Add(seg2[2]); // p6
                Points.Add(seg2[3]); // p7
                Uv.Add(new Vector2(0, 0));
                Uv.Add(new Vector2(1, 0));
            }

            i5 = Points.Count;
            Points.Add(p5); // p5

            i31 = Points.Count;
            Points.Add(p3); // p3.1

            i4 = Points.Count;
            Points.Add(p4); // p4

            i2 = Points.Count;
            Points.Add(p2); // p2

            i3 = Points.Count;
            Points.Add(p3); // p3

            leftSidelength += (Points[i6] - Points[i5]).magnitude;
            
            Uv.Add(new Vector2(0, leftSidelength)); // p5
            Uv.Add(new Vector2(1, leftSidelength)); // p3.1

            leftSidelength += (p5 - p4).magnitude;
            Uv.Add(new Vector2(0, leftSidelength)); // p4

            leftSidelength += (p4 - p2).magnitude;
            Uv.Add(new Vector2(0, leftSidelength)); // p2
            Uv.Add(new Vector2(1, leftSidelength)); // p3

            Triangles.AddRange(new int[] {
                i5, i6, i7,
                i7, i31, i5,
                i5, i31, i4,
                i4, i3, i2
            });
        }

        private void LhsConnect(ref Vector3 s1, ref Vector3 s2, ref Vector3 s3, ref float leftSidelength, bool first = false)
        {
            var seg1 = GetSegmentPoints(ref s1, ref s2);
            var seg2 = GetSegmentPoints(ref s2, ref s3);

            Vector2 intersectPoint;
            if (!LineMath.Intersection(seg1[1], seg1[3], seg2[1], seg2[3], out intersectPoint))
                throw new Exception("Not have intersections.");

            var p4 = s2 + ((Vector3)intersectPoint - s2).normalized * Args.width;
            int p0i = first ? 0 : Points.Count - 2;
            int p1i = first ? 1 : Points.Count - 1;
            int p2i, p3i, p4i, p5i;

            if (first)
            {
                Points.Add(seg1[0]); // p0
                Points.Add(seg1[1]); // p1
                Uv.Add(new Vector2(0, 0));
                Uv.Add(new Vector2(1, 0));
            }
            p3i = Points.Count;
            Points.Add(seg1[3]); // p3
            
            p4i = Points.Count;
            Points.Add(p4);      // p4
            
            p2i = Points.Count;
            Points.Add(seg1[2]); // p2
           
            p5i = Points.Count;
            Points.Add(seg2[1]); // p5

            var l = (Points[p2i] - Points[p0i]).magnitude;
            leftSidelength += l;

            Uv.Add(new Vector2(1, leftSidelength)); // p3
            Uv.Add(new Vector2(1, leftSidelength)); // p4
            Uv.Add(new Vector2(0, leftSidelength));  // p2
            Uv.Add(new Vector2(1, leftSidelength)); // p5

            Triangles.AddRange(new int[] {
                p0i, p2i, p1i,
                p1i, p2i, p3i,
                p3i, p2i, p4i,
                p4i, p2i, p5i
            });
        }

        private void LhsReverseConnect(ref Vector3 s1, ref Vector3 s2, ref Vector3 s3, ref float leftSidelength, bool first = false)
        {
            Debug.Log("LHS");
            var seg1 = GetSegmentPoints(ref s1, ref s2);
            var seg2 = GetSegmentPoints(ref s2, ref s3);

            Vector2 intersectPoint;
            if (!LineMath.Intersection(seg1[1], seg1[3], seg2[1], seg2[3], out intersectPoint))
                throw new Exception("Not have intersections.");

            var p4 = s2 + ((Vector3)intersectPoint - s2).normalized * Args.width;
            int i6 = first ? 0 : Points.Count - 2;
            int i7 = i6 + 1;
            int i2, i3, i4, i5;

            if (first)
            {
                Points.Add(seg2[2]); // p6
                Points.Add(seg2[3]); // p7
                Uv.Add(new Vector2(0, 0));
                Uv.Add(new Vector2(1, 0));
            }
            i5 = Points.Count;
            Points.Add(seg2[1]); // p5

            i4 = Points.Count;
            Points.Add(p4);      // p4

            i2 = Points.Count;
            Points.Add(seg1[2]); // p2

            i3 = Points.Count;
            Points.Add(seg1[3]); // p3

            leftSidelength += (Points[i6] - Points[i2]).magnitude;

            Uv.Add(new Vector2(1, leftSidelength)); // p5
            Uv.Add(new Vector2(1, leftSidelength)); // p4
            Uv.Add(new Vector2(0, leftSidelength)); // p2            
            Uv.Add(new Vector2(1, leftSidelength)); // p3

            Triangles.AddRange(new int[] {
                i2, i6, i7,
                i7, i5, i2, 
                i2, i5, i4,
                i4, i3, i2
            });
        }

        private void Round() { }
        private void RhsRound() { }
        private void LhsRound() { }
    }
}
