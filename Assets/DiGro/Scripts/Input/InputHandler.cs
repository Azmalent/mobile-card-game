using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace DiGro.Input
{
    public class InputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IInitializePotentialDragHandler
    {

        public int maxPointersCount = 1;
        public bool printDebugString = false;

        [Header("Pass Drag. Если не установлн redirectToHandle, отпраляется родителю.")]
        public bool passDrag = false;
        public bool passOnlyProcessedPointer = true;
        public InputHandler redirectToHandle = null;

        public event EventListener PointerDownListener;
        public event EventListener PointerUpListener;
        public event EventListener DragListener;
        public event EventListener BeginDragListener;
        public event EventListener EndDragListener;


        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId >= maxPointersCount)
                return;

            InvokeListener(PointerDownListener, new EventData(eventData), "OnPointerDown");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId >= maxPointersCount)
                return;

            InvokeListener(PointerUpListener, new EventData(eventData), "OnPointerUp");
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.pointerId >= maxPointersCount && passOnlyProcessedPointer)
                return;

            if (passDrag)
                PassEvent("OnInitializePotentialDrag", eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.pointerId >= maxPointersCount && passOnlyProcessedPointer)
                return;

            InvokeListener(BeginDragListener, new EventData(eventData), "OnBeginDrag");

            if (passDrag)
                PassEvent("OnBeginDrag", eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId >= maxPointersCount)
            {
                if (passDrag && !passOnlyProcessedPointer)
                    PassEvent("OnDrag", eventData);
                return;
            }
            InvokeListener(DragListener, new EventData(eventData), "OnDrag");

            if (passDrag)
                PassEvent("OnDrag", eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerId >= maxPointersCount && passOnlyProcessedPointer)
                return;

            InvokeListener(EndDragListener, new EventData(eventData), "OnEndDrag");

            if (passDrag)
                PassEvent("OnEndDrag", eventData);
        }

        private void InvokeListener(EventListener listener, EventData eventData, string debugString)
        {
            if (printDebugString)
                Debug.Log(debugString);
            listener?.Invoke(eventData);
        }

        private void PassEvent(string message, PointerEventData eventData)
        {
            GameObject obj = null;
            if (redirectToHandle != null)
                obj = redirectToHandle.gameObject;

            if (obj == null && transform.parent != null)
                obj = transform.parent.gameObject;

            if(obj != null)
                obj.SendMessageUpwards(message, eventData, SendMessageOptions.DontRequireReceiver);
        }

    }
}