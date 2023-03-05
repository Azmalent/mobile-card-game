using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpiralJumper.Amorphus
{
    public class AmorphusObject : MonoBehaviour
    {
        [SerializeField] private GameObject m_amorphusPhysicsPrefab = null;
        [SerializeField] private Rigidbody m_rigidbody = null;
        [SerializeField] private Transform m_mesh = null;

        private AmorphusPhysics m_amorphusPhysics = null;

        private void Awake()
        {
            if (!m_amorphusPhysicsPrefab || !m_rigidbody || !m_mesh)
                Debug.LogError("Not all set in " + GetType());

            DiGro.Check.CheckComponent<AmorphusPhysics>(m_amorphusPhysicsPrefab);

            m_amorphusPhysics = Instantiate(m_amorphusPhysicsPrefab).GetComponent<AmorphusPhysics>();
        }

        private void OnDestroy()
        {
            try {
                Destroy(m_amorphusPhysics.gameObject);
            }
            catch(MissingReferenceException ex) {

            }
        }

        private void Start()
        {
            m_amorphusPhysics.Height = m_rigidbody.position.y;
        }

        private void FixedUpdate()
        {
            m_amorphusPhysics.Height = m_rigidbody.position.y;

            var scale = m_mesh.transform.localScale;
            scale.y = m_amorphusPhysics.Scale;
            m_mesh.transform.localScale = scale;
        }

    }
}