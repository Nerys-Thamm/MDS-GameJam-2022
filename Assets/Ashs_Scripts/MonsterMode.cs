#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMode : MonoBehaviour
{
    [SerializeField] PlayerMotor m_PlayerMovement;
    [SerializeField] bool m_IsInMonsterMode = false;

    [SerializeField] Transform m_SpherePoint;
    [SerializeField] float m_SphereCastRadius;
    [SerializeField] LayerMask m_LayerMask; 

    InputAction Input_SwitchMode;
    Animator m_animator;

    [SerializeField]
    float m_transformCooldown;
    float m_maxTrasnformCooldown = 10.0f;

    [SerializeField]
    SFX_Effect TransformIntoEffect;
    [SerializeField]
    SFX_Effect TransformOutOfEffect;

    // Start is called before the first frame update
    void Start()
    {
        Input_SwitchMode = new InputAction("MonsterMode", binding: "<Gamepad>/rightTrigger");
        Input_SwitchMode.AddBinding("<Keyboard>/leftShift");

        m_PlayerMovement = GetComponent<PlayerMotor>();

        Input_SwitchMode.Enable();
    }

    void ToggleMonsterMode(InputAction.CallbackContext context)
    {
        if (m_transformCooldown <= 0.0f)
        {
            m_IsInMonsterMode = !m_IsInMonsterMode;
            if (m_IsInMonsterMode)
            {
                TransformIntoEffect.Play();
            }
            else
            {
                TransformOutOfEffect.Play();
            }
            m_transformCooldown = m_maxTrasnformCooldown;
        }
        else {
            return;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (m_transformCooldown > 0.0f)
        {
            m_transformCooldown -= Time.deltaTime;
        }
        Input_SwitchMode.performed += ToggleMonsterMode;
     
        if (m_IsInMonsterMode)
        {
            Collider[] hitNPCs = Physics.OverlapSphere(m_SpherePoint.position, m_SphereCastRadius, m_LayerMask);


            foreach (Collider NPC in hitNPCs)
            {
                TrickOrTreaterAI thisNPC = NPC.GetComponent<TrickOrTreaterAI>();
                thisNPC.OnDeath();
            }
        }
    }

    private void Input_SwitchMode_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_SpherePoint.position, m_SphereCastRadius);
    }
}
