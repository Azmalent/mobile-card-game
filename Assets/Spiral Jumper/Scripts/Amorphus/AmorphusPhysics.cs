using System;
using System.Collections.Generic;
using UnityEngine;


namespace SpiralJumper.Amorphus
{
    public class AmorphusPhysics : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D center = null;
        [SerializeField] private Rigidbody2D point1 = null;
        [SerializeField] private Rigidbody2D point2 = null;

        public float Height { get; set; }
        public float Scale { get; set; }


        private void Awake()
        {
            if (!center || !point1 || !point2)
                Debug.LogError("Not all set in " + GetType());
        }

        private void FixedUpdate()
        {
            var pos = center.position;
            pos.y = Height;
            center.position = pos;

            Scale = (point1.position - point2.position).y;
        }
    }
}