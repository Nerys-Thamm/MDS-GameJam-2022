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
    InputAction Input_Attack;
    float m_maxTrasnformCooldown = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Bind Action Inputs
        Input_SwitchMode = new InputAction("MonsterMode", binding: "<Gamepad>/rightTrigger");
        Input_SwitchMode.AddBinding("<Keyboard>/leftShift");

        Input_DropCandy = new InputAction("DropCandy", binding: "<Gamepad>/leftTrigger");
        Input_DropCandy.AddBinding("<Keyboard>/space");

        Input_Attack = new InputAction("Attack", binding: "<Gamepad>/leftTrigger");
        Input_Attack.AddBinding("<Keyboard>/space");

        //Enable Inputs
        Input_DropCandy.Enable();
        Input_SwitchMode.Enable();
        Input_Attack.Enable();

        //Get Internal Components
        m_PlayerMovement = GetComponent<PlayerMotor>();
 
        m_animator = GetComponentInChildren<Animator>();
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
            m_animator.SetTrigger("TriggerTransform");
            TransformIntoEffect.Play();
        }
        else
        {
            m_animator.SetTrigger("TriggerChild");
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

    public bool GetMode()
    {
        return m_IsInMonsterMode;
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (m_IsInMonsterMode)
        {
            StartCoroutine(m_PlayerMovement.ToggleMovementLock(1.0f));
            Collider[] hitNPCs = Physics.OverlapSphere(m_SpherePoint.position, m_SphereCastRadius, m_EatableLayerMask);
            Debug.Log(hitNPCs.Length);
            foreach (Collider NPC in hitNPCs)
            {
                TrickOrTreaterAI thisNPC = NPC.GetComponent<TrickOrTreaterAI>();
                thisNPC.Death();
            }
            m_animator.SetTrigger("Attack");
        }
    }

    public void TriggerEndAnimation(InputAction.CallbackContext context)
    {
        m_PlayerMovement.ToggleMovementLock();
        m_animator.SetTrigger("TriggerEnd");
    }
    private void FixedUpdate()
    {
        Input_SwitchMode.performed += ToggleMonsterMode;
        Input_DropCandy.performed += DropCandy;
        Input_Attack.performed += Attack;
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_SpherePoint.position, m_SphereCastRadius);
    }
}
