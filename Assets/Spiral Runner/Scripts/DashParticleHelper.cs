using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SpiralJumper.Audio;
using SpiralJumper.Amorphus;
using SpiralJumper.Controller;

using SJ = SpiralJumper;
using SR = SpiralRunner;

namespace SpiralRunner.View {

    public class DashParticleHelper : MonoBehaviour {

        [HideInInspector] public ParticleSystem ps;
        [HideInInspector] public LinkedListNode<DashParticleHelper> node;

        private void OnParticleSystemStopped() {
            node.List.Remove(node);
        }

    }
}
