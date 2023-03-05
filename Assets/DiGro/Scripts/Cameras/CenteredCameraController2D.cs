using System;
using UnityEngine;

public class CenteredCameraController2D : CenteredCamera2D {

    private void Update() {
        HandleInput();
    }

    public void HandleInput() {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > float.Epsilon) 
            Distance -= scroll * scrollSens;

        var delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if(Math.Abs(delta.x) > axisIdling || Math.Abs(delta.y) > axisIdling) {
            if (Input.GetKey(KeyCode.Mouse2))
                OnSwipe(delta, KeyCode.Mouse2);

            else if (Input.GetKey(KeyCode.Mouse1))
                OnSwipe(delta, KeyCode.Mouse1);
        }
    }

    private void OnSwipe(Vector2 delta, KeyCode code) {
        if (code == KeyCode.Mouse2) {
            delta.x *= SwipeSense;
            delta.y *= SwipeSense;
            Vector3 resDeltaH = Vector3.zero;
            Vector3 resDeltaV = Vector3.zero;

            if (Math.Abs(delta.x) > axisIdling) {
                Vector3 right = transform.rotation * Vector3.right;
                resDeltaH = right * delta.x;
            }
            if (Math.Abs(delta.y) > axisIdling) {
                Vector3 up = transform.rotation * Vector3.up;
                resDeltaV = up * delta.y;
            }
            m_Center -= resDeltaH + resDeltaV;
        }
    }
    
}