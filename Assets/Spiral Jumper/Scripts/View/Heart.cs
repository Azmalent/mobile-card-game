using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour {

    [SerializeField] private Image m_fill;
    [SerializeField] private Image m_outline;
    [SerializeField] private Animator m_animator;

    public Color FullColor;
    public Color EmptyColor;

    [SerializeField] private bool m_filled = false;

    public bool Filled { 
        get { return m_filled; }
        set { 
            m_filled = value;
            m_fill.color = m_filled ? FullColor : EmptyColor;
        } 
    }

    private void Awake() {
        Filled = m_filled;
    }

    public void Fade() {
        m_animator.Play("Fade");
    }

    public void OnEndFade() {
        Filled = false;
    }
}
