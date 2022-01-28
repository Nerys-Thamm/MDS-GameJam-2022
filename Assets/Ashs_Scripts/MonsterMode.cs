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
    [SerializeField] float m_SphereOffset = 5.0f;
    float m_SpherecastMaxDist = 5.0f;
    [SerializeField] LayerMask m_LayerMask; 

    InputAction Input_SwitchMode;

    [SerializeField]
    SFX_Effect TransformIntoEffect;
    [SerializeField]
    SFX_Effect TransformOutOfEffect;

    // Start is called before the first frame update
    void Start()
    {
        Input_SwitchMode = new InputAction("MonsterMode", binding: "<Keyboard>/space");
        Input_SwitchMode.AddBinding("<Keyboard>/leftShift");

        m_PlayerMovement = GetComponent<PlayerMotor>();

        Input_SwitchMode.Enable();
    }

    void ToggleMonsterMode(InputAction.CallbackContext context)
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
    }


    // Update is called once per frame
    void Update()
    {
        Input_SwitchMode.performed += ToggleMonsterMode;
         
            
        
     
        if (m_IsInMonsterMode)
        {
            Collider[] hitNPCs = Physics.OverlapSphere(m_SpherePoint.position, m_SphereCastRadius, m_LayerMask);


            foreach (Collider NPC in hitNPCs)
            {
                Destroy(NPC.gameObject);
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
