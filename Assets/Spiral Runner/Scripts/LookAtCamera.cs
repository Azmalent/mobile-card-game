using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiralRunner {

    public class LookAtCamera : MonoBehaviour {

        private void LateUpdate() {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up);
        }

    }
}