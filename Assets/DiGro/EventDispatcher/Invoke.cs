using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

namespace DiGro {
    [UnitCategory("DiGro\\EventDispatcher")]
    [UnitSubtitle("EventDispatcher")]
    public class Invoke : Unit {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit;

        [DoNotSerialize]
        public ValueInput targetEvent;

        protected override void Definition() {
            enter = ControlInput("Enter", (flow) => { m_Run(flow); return exit; });
            exit = ControlOutput("Exit");

            targetEvent = ValueInput<EventDispatcher>("Event");

            Requirement(targetEvent, enter);

            Succession(enter, exit);
        }

        private void m_Run(Flow flow) {
            var dispatcher = flow.GetValue<EventDispatcher>(targetEvent);

            //Debug.Log("Invoke");

            dispatcher.Invoke();
        }
    }
}