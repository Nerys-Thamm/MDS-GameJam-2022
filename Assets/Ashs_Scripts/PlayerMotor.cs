using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("Internal Components")]
    [SerializeField] Rigidbody m_Rigid;
    Animator m_animator;

    [Header("Info")]
    [SerializeField] float m_MoveSpeed = 10.0f;
    [SerializeField] float m_TurnSpeed = 360.0f;
    [SerializeField] bool m_IsMovementLocked = false;

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

    void ProcessInput()
    {
        m_MovementDelta = Movement.ReadValue<Vector2>();

            

        if (m_MovementDelta != Vector3.zero)
            m_Rigid.MovePosition(transform.position + direction * m_MoveSpeed * Time.deltaTime);
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
    }

    void FixedUpdate()
    {
        if (!m_IsMovementLocked)
            ProcessInput();
    }

    void Update()
    {
        Look();
    }
}
