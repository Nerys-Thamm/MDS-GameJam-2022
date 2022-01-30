using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("Internal Components")]
    MonsterMode m_MonsterMode;
    [SerializeField] Rigidbody m_Rigid;
    Animator m_animator;

    [Header("Info")]
    [SerializeField] float m_ChildSpeed = 10.0f;
    [SerializeField] float m_MonsterSpeed = 4.0f;
    [SerializeField] float m_TurnSpeed = 360.0f;
    [SerializeField] bool m_IsMovementLocked = true;

    Vector3 m_MovementDelta;

    public Camera cam;
    public Transform camTransform;

    Vector3 direction;

    //InputActions
    InputAction Movement;

    // Might be useful for the Transformation animation
    public void ToggleMovementLock()
    {
        m_IsMovementLocked = !m_IsMovementLocked;
    }

    public void SetMovementLock(bool _isLocked)
    {
        m_IsMovementLocked = _isLocked;
    }

    /*public IEnumerator ToggleMovementLock(float _Time)
    {
        m_IsMovementLocked = true;
        yield return new WaitForSeconds(_Time);
        m_IsMovementLocked = false;
    }*/
    void ProcessInput()
    {
        m_MovementDelta = Movement.ReadValue<Vector2>();
        m_animator.SetBool("IfInput", Movement.ReadValue<Vector2>() != Vector2.zero);


        // Switch Between walkSpeeds depending on if the pumpkin guy is a monster or not
        if (m_MovementDelta != Vector3.zero && m_MonsterMode.GetMode())
            m_Rigid.MovePosition(transform.position + direction * m_MonsterSpeed * Time.deltaTime);
        else if (m_MovementDelta != Vector3.zero && !m_MonsterMode.GetMode())
            m_Rigid.MovePosition(transform.position + direction * m_ChildSpeed * Time.deltaTime);
    }

    void Look()
    {
        if (m_MovementDelta != Vector3.zero)
        {

            var forward = camTransform.forward;
            var right = camTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            direction = Vector3.Lerp(direction, (forward * m_MovementDelta.y + right * m_MovementDelta.x), Time.deltaTime * m_TurnSpeed);
            direction = direction.normalized * (forward * m_MovementDelta.y + right * m_MovementDelta.x).magnitude;
            

            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }


  

    // Start is called before the first frame update
    void Start()
    {
        // Set up Movement Input
        Movement = new InputAction("PlayerMovement", binding: "<Gamepad>/leftStick");
        Movement.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");

    
        Movement.Enable();

        m_MonsterMode = GetComponent<MonsterMode>();
        m_animator = GetComponentInChildren<Animator>();

      /* //StartCoroutine(SpawnCoroutine());*/
    }

    void FixedUpdate()
    {
       
    }

    void Update()
    {
        if (!m_IsMovementLocked)
            ProcessInput();
        Look();
    }
}
