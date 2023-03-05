using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace DiGro.ScreenSystem {
    [RequireComponent(typeof(Canvas))]
    public class BlockingSplash : MonoBehaviour, IPointerClickHandler {
        [Header(nameof(BlockingSplash))]

        [SerializeField] private Image m_image;
        [Space]
        [SerializeField] private int m_initSortingOrder = 0;
        public event Action OnClickAction;

        private Canvas m_canvas;


        private void Awake() {
            if (!m_image)
                Debug.LogError("Not all set in " + GetType());
            m_canvas = GetComponent<Canvas>();
            m_canvas.overrideSorting = true;
            m_canvas.sortingOrder = m_initSortingOrder;
        }


        public int SortingOrder {
            get { return m_canvas.sortingOrder; }
            set { m_canvas.sortingOrder = value; }
        }

        public void OnPointerClick(PointerEventData eventData) {
            OnClickAction?.Invoke();
        }

    }
}