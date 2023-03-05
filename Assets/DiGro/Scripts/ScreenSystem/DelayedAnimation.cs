using System;
using System.Collections.Generic;

using UnityEngine;

namespace DiGro.ScreenSystem {

    public class CachedDelayedAnimation {
        public Animator animator;
        public DelayedAnimation animation;

        public void Invoke() {
            if (animator == null)
                return;

            if (!animator.gameObject.activeSelf && animation.shouldActivate)
                animator.gameObject.SetActive(true);

            switch (animation.type) {
                case DelayedAnimation.ParamType.State:
                    animator.Play(animation.parameter);
                    break;
                case DelayedAnimation.ParamType.Float:
                    animator.SetFloat(animation.parameter, animation.floatValue);
                    break;
                case DelayedAnimation.ParamType.Integer:
                    animator.SetInteger(animation.parameter, animation.integerValue);
                    break;
                case DelayedAnimation.ParamType.Bool:
                    animator.SetBool(animation.parameter, animation.boolValue);
                    break;
                case DelayedAnimation.ParamType.Trigger:
                    animator.SetTrigger(animation.parameter);
                    break;
            }
        }
    }

    [Serializable]
    public class DelayedAnimation {
        public enum ParamType { State, Float, Integer, Bool, Trigger }

        [Tooltip("Путь до компонента Animator, относительно текущего. Иерархия указывается через косую черту. " +
        "Если путь недействителен или в нем нет аниматора, анимация будет пропущена.")]
        public string animator = "";
        [Space]
        [Tooltip("Тип устанавливаемого параметра. State - переход в установленное состояние.")]
        public ParamType type = ParamType.State;

        [Tooltip("Имя устанавливаемого параметра или имя состояния.")]
        public string parameter = "";

        [Space]
        public float floatValue = 0f;
        public int integerValue = 0;
        public bool boolValue = false;

        [Space]
        [Tooltip("Время относительно начала, после которого будет запущена текущая анимация.")]
        public float delay = 0f;

        [Space]
        [Tooltip("Говорит о том, что перед установкой параметра нужно активаировать GameObject, если он не активен.")]
        public bool shouldActivate = false;

    }

}