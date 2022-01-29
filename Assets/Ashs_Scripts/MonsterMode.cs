#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMode : MonoBehaviour
{
    //Components
    [Header("Internal Components")]
    Animator m_animator;
    PlayerMotor m_PlayerMovement;
    [SerializeField] Light m_SpotLight;
    [SerializeField] SFX_Effect TransformIntoEffect;
    [SerializeField] SFX_Effect TransformOutOfEffect;

    [SerializeField] GameObject CandyPrefab;

    [Header("Info")]
    [SerializeField] bool m_IsInMonsterMode = false;
    [SerializeField] Transform m_SpherePoint;
    [SerializeField] float m_SphereCastRadius;
    [SerializeField] LayerMask m_EatableLayerMask;
    [SerializeField] float m_transformCooldown;
    [SerializeField] float m_DropCandyCooldown;
    [SerializeField] float m_MaxDropCandyCooldown = 1.5f;

    [SerializeField] Transform m_CandyDropPoint;

    //Input Actions
    InputAction Input_SwitchMode;
    InputAction Input_DropCandy;

    float m_maxTrasnformCooldown = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Bind Action Inputs
        Input_SwitchMode = new InputAction("MonsterMode", binding: "<Gamepad>/rightTrigger");
        Input_SwitchMode.AddBinding("<Keyboard>/leftShift");

        Input_DropCandy = new InputAction("DropCandy", binding: "<Gamepad>/leftTrigger");
        Input_DropCandy.AddBinding("<Keyboard>/space");

        //Enable Inputs
        Input_DropCandy.Enable();
        Input_SwitchMode.Enable();

        //Get Internal Components
        m_PlayerMovement = GetComponent<PlayerMotor>();
        m_SpotLight = GetComponentInChildren<Light>();

        m_SpotLight.enabled = false;
    }

    void DropCandy(InputAction.CallbackContext context)
    {
        if (!m_IsInMonsterMode && m_DropCandyCooldown <= 0.0f)
        {
            Instantiate(CandyPrefab, m_CandyDropPoint.position, Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f),0.0f));
            m_DropCandyCooldown = m_MaxDropCandyCooldown;
        }
    }

    IEnumerator TransformCoroutine()
    {
        m_PlayerMovement.ToggleMovementLock();
        if (m_IsInMonsterMode)
        {
            m_SpotLight.enabled = true;
            TransformIntoEffect.Play();
        }
        else
        {
            m_SpotLight.enabled = false;
            TransformOutOfEffect.Play();
        }
       
        yield return new WaitForSeconds(1.5f);
        m_PlayerMovement.ToggleMovementLock();
    }
    void ToggleMonsterMode(InputAction.CallbackContext context)
    {
        if (m_transformCooldown <= 0.0f)
        {
            m_IsInMonsterMode = !m_IsInMonsterMode;
            StartCoroutine(TransformCoroutine());
            m_transformCooldown = m_maxTrasnformCooldown;
        }
        else {
            return;
        }
    }


    private void LateUpdate()
    {
        Input_SwitchMode.performed += ToggleMonsterMode;
        Input_DropCandy.performed += DropCandy;
    }
    // Update is called once per frame
    void Update()
    {
        if (m_transformCooldown > 0.0f)
        {
            m_transformCooldown -= Time.deltaTime;
        }

        if (m_DropCandyCooldown > 0.0f)
        {
            m_DropCandyCooldown -= Time.deltaTime;
        }



        if (m_IsInMonsterMode)
        {
            Collider[] hitNPCs = Physics.OverlapSphere(m_SpherePoint.position, m_SphereCastRadius, m_EatableLayerMask);

            foreach (Collider NPC in hitNPCs)
            {
                TrickOrTreaterAI thisNPC = NPC.GetComponent<TrickOrTreaterAI>();
                thisNPC.Death();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_SpherePoint.position, m_SphereCastRadius);
    }
}
