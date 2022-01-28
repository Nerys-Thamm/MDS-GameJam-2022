#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMode : MonoBehaviour
{
    [SerializeField] PlayerMovement m_PlayerMovement;
    [SerializeField] bool m_IsInMonsterMode = false;

    [SerializeField] Transform m_SpherePoint;
    [SerializeField] float m_SphereCastRadius;

    [SerializeField] LayerMask m_LayerMask; 

    InputAction Input_SwitchMode;

    // Start is called before the first frame update
    void Start()
    {
        Input_SwitchMode = new InputAction("MonsterMode", binding: "<Keyboard>/space");
        Input_SwitchMode.AddBinding("<Keyboard>/leftShift");

        m_PlayerMovement = GetComponent<PlayerMovement>();

        Input_SwitchMode.Enable();
    }

    void ToggleMonsterMode(InputAction.CallbackContext context)
    {
        m_IsInMonsterMode = !m_IsInMonsterMode;
    }

    // Update is called once per frame
    void Update()
    {
        m_IsInMonsterMode = Mathf.Approximately(Input_SwitchMode.ReadValue<float>(), 1);
       
        if (m_IsInMonsterMode)
        {
            RaycastHit hit;

            if (Physics.SphereCast(m_SpherePoint.position, m_SphereCastRadius, transform.right , out hit,m_LayerMask))
            {
                Debug.Log("Kid Eaten");
                Destroy(hit.transform.gameObject);
                /*if (hit.transform.CompareTag("Kid"))
                {
                   
                }*/
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_SpherePoint.position, m_SphereCastRadius);
    }
}
