using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Unity.VisualScripting;

namespace DiGro {

    [Inspectable]
    public class EventDispatcher {

        public LinkedList<MachineAction> actions = new LinkedList<MachineAction>();

        public void AddListener(MachineAction action) {
            actions.AddLast(action);
        }

        public void RemoveListener(MachineAction action) {
            var node = actions.Last;

            while (node != null) {
                var prev = node.Previous;

                if (node.Value.GetHashCode() == action.GetHashCode()) {
                    actions.Remove(node);
                    break;
                }
                node = prev;
            }
        }

        public void Invoke() {
            foreach (var action in actions)
                CustomEvent.Trigger(action.target, action.method);
        }

    }
}