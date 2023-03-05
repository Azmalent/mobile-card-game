using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralJumper.View
{
    public class Player : MonoBehaviour
    {
        public event Action<PlatformEffector, int, bool> PlatformEnterListener;
        public event Action<RushSphere> RushSphereEnterListener;

        [SerializeField] private Rigidbody m_rigidbody = null;


        private void Awake()
        {
            if (!m_rigidbody)
                Debug.LogError("Not all set in Player");
        }


        private void OnCollisionEnter(Collision collision)
        {
            var effector = collision.collider.gameObject.GetComponent<PlatformEffector>();
            bool centerOnEffector = false;

            if (effector != null)
            {
                var contactPoint = collision.contacts[0].point;
                
                RaycastHit hit;
                var mask = LayerMask.GetMask(new string[] { "Platform Effector" });
                if (Physics.Raycast(new Ray(m_rigidbody.position, Vector3.down), out hit))
                {
                    var bottomEffector = hit.collider.gameObject.GetComponent<PlatformEffector>();
                    centerOnEffector = bottomEffector == effector;
                }
                
                PlatformEnterListener?.Invoke(effector, effector.GetSectorByContactPoint(contactPoint), centerOnEffector);
            }
        }

        private void OnTriggerEnter(Collider collider) { 
            var rushSphere = collider.gameObject.GetComponent<RushSphere>();
            if(rushSphere != null)
                RushSphereEnterListener?.Invoke(rushSphere);

        }

    }
}