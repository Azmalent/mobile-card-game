using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SJ = SpiralJumper;

namespace SpiralRunner.View {

    [AddComponentMenu("Player (SR)")]
    public class Player : MonoBehaviour {

        public event Action<SJ.View.PlatformEffector, int, bool> PlatformEnterEvent;

        [SerializeField] private GameObject m_rigidbody = null;


        private void Awake() {
            if (!m_rigidbody)
                Debug.LogError("Not all set in Player");
        }


        //private void OnCollisionEnter(Collision collision) {
        //    var effector = collision.collider.gameObject.GetComponent<PlatformEffector>();
        //    bool centerOnEffector = false;

        //    if (effector != null) {
        //        var contactPoint = collision.contacts[0].point;

        //        RaycastHit hit;
        //        var mask = LayerMask.GetMask(new string[] { "Platform Effector" });
        //        if (Physics.Raycast(new Ray(m_rigidbody.position, Vector3.down), out hit)) {
        //            var bottomEffector = hit.collider.gameObject.GetComponent<PlatformEffector>();
        //            centerOnEffector = bottomEffector == effector;
        //        }

        //        PlatformEnterListener?.Invoke(effector, effector.GetSectorByContactPoint(contactPoint), centerOnEffector);
        //    }
        //}

        private void OnTriggerEnter(Collider collider) {
            bool isPlatform = collider.gameObject.CompareTag("Platform");
            if (!isPlatform)
                return;

            var effector = collider.gameObject.GetComponent<SJ.View.PlatformEffector>();
            bool centerOnEffector = false;

            if (effector != null) {
                var contactPoint = collider.ClosestPoint(m_rigidbody.transform.position);

                RaycastHit hit;
                var mask = LayerMask.GetMask(new string[] { "Platform Effector" });

                Debug.DrawLine(m_rigidbody.transform.position + Vector3.down * 0.5f, m_rigidbody.transform.position + Vector3.up * 0.5f, Color.red, 1);

                if (Physics.Raycast(m_rigidbody.transform.position + Vector3.down * 0.5f, Vector3.up, out hit, 1, mask, QueryTriggerInteraction.Collide)) {
                    var bottomEffector = hit.collider.gameObject.GetComponent<SJ.View.PlatformEffector>();
                    centerOnEffector = bottomEffector == effector;
                }

                if (centerOnEffector)
                    PlatformEnterEvent?.Invoke(effector, effector.GetSectorByContactPoint(contactPoint), centerOnEffector);
            }
        }

    }
}
