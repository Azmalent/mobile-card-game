using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// (-AngleY, AngleX + 180f, 0) - axis transform
/// 
[RequireComponent(typeof(Camera))]
public class CenteredCamera2D : MonoBehaviour
{
    public enum UpdateMethod { LateUpdate, FixedUpdate }

    [Header(nameof(CenteredCamera))]
    [Header("Settings")]
    public float scrollSens = 1f;
    public float rotateSens = 10f;
    public float swipeSense = 0.1f;
    public float minSwipeSense = 0.02f;
    public float axisIdling = 0.001f;
    public float minDistance = 0.001f;
    public bool drawNormals = false;
    public UpdateMethod updateMethod = UpdateMethod.LateUpdate;

    [Header("Values")]
    [SerializeField] protected Vector3 m_Center;
    [SerializeField] protected Vector2 m_Angles;
    [SerializeField] protected float m_Distance = 3f;

    [Header("Debug values")]
    [SerializeField] private float m_CurrentSwipeSense = 0.4f;

    private Camera m_Camera;
    private Vector3 m_LastCenter;
    private Vector2 m_LastAngles;
    private float m_LastDistance;


    public float AngleY
    {
        get { return m_Angles.y; }
        set
        {
            if (value < -90)
                m_Angles.y = -90;
            else if (value > 90)
                m_Angles.y = 90;
            else
                m_Angles.y = value;
        }
    }

    public float AngleX
    {
        get { return m_Angles.x; }
        set { m_Angles.x = value; }
    }

    public float Distance
    {
        get { return m_Distance; }
        set
        {
            m_Distance = value < minDistance ? minDistance : value;
        }
    }

    protected float SwipeSense
    {
        get
        {
            float value = swipeSense * m_Distance;
            return value < minSwipeSense ? minSwipeSense : value;
        }
        set { swipeSense = value; }
    }

    protected float RotateSense
    {
        get { return rotateSens; }
        set { rotateSens = value; }
    }

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();

        m_LastCenter = m_Center;
        Vector3 dir = (transform.position - m_Center).normalized;
        transform.position = dir * m_Distance + m_Center;
    }

    public void LateUpdate()
    {
        if (updateMethod != UpdateMethod.LateUpdate)
            return;

        UpdateCamera();
    }

    private void FixedUpdate()
    {
        if (updateMethod != UpdateMethod.FixedUpdate)
            return;

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (m_LastCenter != m_Center)
        {
            Vector3 vec = transform.position - m_LastCenter;
            transform.position = vec + m_Center;
            m_LastCenter = m_Center;
        }
        if (Math.Abs(m_Distance - m_LastDistance) >= minDistance)
        {
            Distance = m_Distance;
            m_Camera.orthographicSize = m_Distance;
            m_LastDistance = m_Distance;
        }
        if (m_LastAngles != m_Angles)
        {
            AngleY = m_Angles.y;
            transform.rotation = Quaternion.Euler(-AngleY, AngleX + 180f, 0);
            Vector3 vec = transform.rotation * Vector3.back;
            transform.position = vec * m_Distance + m_Center;
            m_LastAngles = m_Angles;
        }

        Vector3 right = transform.rotation * Vector3.right;
        Vector3 up = transform.rotation * Vector3.up;
        if (drawNormals)
        {
            Debug.DrawLine(transform.position, m_Center);
            Debug.DrawLine(m_Center, m_Center + right * 2, Color.red);
            Debug.DrawLine(m_Center, m_Center + up * 2, Color.green);
        }
        m_CurrentSwipeSense = SwipeSense;
    }
}