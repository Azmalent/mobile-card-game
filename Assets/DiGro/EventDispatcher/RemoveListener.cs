using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

namespace DiGro {
    [UnitCategory("DiGro\\EventDispatcher")]
    [UnitSubtitle("EventDispatcher")]
    public class RemoveListener : Unit {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter;

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit;

        [DoNotSerialize]
        public ValueInput targetEvent;

        [DoNotSerialize]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput target;

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput name;

        protected override void Definition() {
            enter = ControlInput("Enter", (flow) => { m_Run(flow); return exit; });
            exit = ControlOutput("Exit");

            targetEvent = ValueInput<EventDispatcher>("Event");
            target = ValueInput<GameObject>("Target", null);
            name = ValueInput<string>("Method", string.Empty);

            Requirement(targetEvent, enter);
            //Requirement(target, enter);

            Succession(enter, exit);
        }

        private void m_Run(Flow flow) {
            var methodName = flow.GetValue<string>(name);
            if (methodName == "") {
                Debug.LogWarning("AddListener: method name is empty");
                return;
            }

            var targetObject = flow.GetValue<GameObject>(target);
            var dispatcher = flow.GetValue<EventDispatcher>(targetEvent);

            dispatcher.RemoveListener(new MachineAction(targetObject, methodName));
        }
    }
}