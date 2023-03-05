using System;
using System.Collections.Generic;

using UnityEngine;


namespace DiGro.MeshGenerator
{
    
    public interface IMeshGenerator
    {
        void Generate();
        List<Vector3> Points { get; }
        List<Vector3> Normals { get; }
        List<Vector2> Uv { get; }
        List<int> Triangles { get; }
    }
}