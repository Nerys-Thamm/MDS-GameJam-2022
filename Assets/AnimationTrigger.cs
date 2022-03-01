using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationTrigger : MonoBehaviour
{
    public Animator animator;

    [System.Serializable]
    public struct TriggerBinding
    {
        public InputAction Input;
        public string TriggerName;
    }

    [SerializeField]
    public List<TriggerBinding> m_triggers;

    public void Awake()
    {
        foreach (var trigger in m_triggers)
        {
            trigger.Input.performed += ctx =>
            {
                animator.SetTrigger(trigger.TriggerName);
            };
            trigger.Input.Enable();
        }
    }
}
