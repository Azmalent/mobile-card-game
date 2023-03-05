using System;
using System.Collections;
using UnityEngine;

public class ValueChanger
{
    private AnimationCurve m_curve;

    public bool IsChanging => m_timer < m_duration;

    private float m_startValue = 0;
    private float m_targetValue = 0;
    private float m_duration = 1;
    private float m_timer = float.MaxValue;


    public ValueChanger(int currentValue, int targetValue, float duration, AnimationCurve curve)
    {
        m_startValue = currentValue;
        m_targetValue = targetValue;
        m_duration = duration;
        m_curve = curve;
        m_timer = 0;
    }

    public IEnumerator Invoke(Action<int> onValueChanged)
    {
        while (m_timer < m_duration)
        {
            m_timer += Time.deltaTime;

            float t = Mathf.Clamp(m_timer / m_duration, 0, 1);
            float v = m_curve.Evaluate(t);
            int value = Mathf.RoundToInt(Mathf.Lerp(m_startValue, m_targetValue, v));

            onValueChanged?.Invoke(value);

            yield return null;
        }
    }

}
