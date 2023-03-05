using System;

using UnityEngine;
using UnityEngine.EventSystems;


namespace DiGro.Input {

    public delegate void EventListener(EventData eventData);

    public class EventData
    {
        public PointerEventData eventData;
        public Vector3 worldPosition;
        public Vector3 pressWorldPosition;

        public EventData(PointerEventData ev)
        {
            eventData = ev;
            worldPosition = ToWorldPos(ev.position);
            pressWorldPosition = ToWorldPos(ev.pressPosition);
        }

        private Vector3 ToWorldPos(Vector2 pos)
        {
            var point = Camera.main.ScreenToWorldPoint(
                new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));
            point.z = 0;
            return point;
        }
    }

}