using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiGro.MeshGenerator
{
    public class MeshGeneratorTest : MonoBehaviour
    {
        public GameObject pointPrefab;
        public MeshFilter meshFilter;
        [Space]
        public bool generate = false;
        [Range(0.01f, 1f)]
        public float width = 0.6f;
        public List<GameObject> sourcePoints = new List<GameObject>();
        private List<Vector3> LastPositions;
        private float m_lastWidth;

        private List<GameObject> pointsObj = new List<GameObject>();


        private void Awake()
        {
            if (!GameObjectsPool.HasPool("Points"))
                GameObjectsPool.CreatePool("Points", pointPrefab, new SimplePoolListener());

            for (int i = 0; i < transform.childCount; i++)
            {
                var obj = transform.GetChild(i).gameObject;
                if (obj.activeSelf)
                    sourcePoints.Add(obj);
            }

            LastPositions = new List<Vector3>(sourcePoints.Count);
            foreach (var obj in sourcePoints)
                LastPositions.Add(obj.transform.position);

            m_lastWidth = width;
        }

        private void Update()
        {
            for (int i = 0; i < LastPositions.Count; i++)
            {
                if (LastPositions[i] != sourcePoints[i].transform.position)
                {
                    LastPositions[i] = sourcePoints[i].transform.position;
                    generate = true;
                    break;
                }
            }

            if(m_lastWidth != width)
            {
                m_lastWidth = width;
                generate = true;
            }

            if (generate)
            {
                generate = false;

                var points = new List<Vector3>(sourcePoints.Count);
                foreach (var obj in sourcePoints)
                    points.Add(obj.transform.localPosition);

                var lineGen = new LineMeshGenerator2D(new LineMeshGenerator2D.Parameters
                {
                    points = points,
                    width = width
                });
                lineGen.Generate();

                for (int i = 0; i < pointsObj.Count; i++)
                    GameObjectsPool.Push("Points", pointsObj[i]);

                pointsObj.Clear();
                pointsObj = new List<GameObject>(lineGen.Points.Count);

                for(int i = 0; i < lineGen.Points.Count; i++)
                {
                    var obj = GameObjectsPool.Pop("Points");
                    obj.name = "Point " + i.ToString();
                    obj.transform.parent = transform;
                    obj.transform.localPosition = lineGen.Points[i];
                    pointsObj.Add(obj);
                }

                var mesh = new Mesh();
                //var mesh = meshFilter.mesh;
                //if (mesh == null)
                //    mesh = new Mesh();

                mesh.vertices = lineGen.Points.ToArray();
                mesh.triangles = lineGen.Triangles.ToArray();
                mesh.uv = lineGen.Uv.ToArray();
                mesh.normals = lineGen.Normals.ToArray();

                meshFilter.mesh = mesh;
            }
        }

    }
}