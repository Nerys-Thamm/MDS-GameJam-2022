using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] Rigidbody m_Rigid;
    [SerializeField] float m_MoveSpeed = 10.0f;
    [SerializeField] float m_TurnSpeed = 360.0f;
    private Vector3 m_MovementDelta;

    Animator m_animator;
    InputAction Movement;

    bool m_IsMovementLocked = false;

    // Might be useful for the Transformation animation
    void ToggleMovementLock()
    {
        m_IsMovementLocked = !m_IsMovementLocked;
    }
    void ProcessInput()
    {
        m_MovementDelta = Movement.ReadValue<Vector2>();

        if (m_MovementDelta != Vector3.zero)
            m_Rigid.MovePosition(transform.position + transform.forward * m_MoveSpeed * Time.deltaTime);
    }

    void Look()
    {
        //transform.rotation = Quaternion.Euler(0.0f, transform.rotation.y, 0.0f);
        if (m_MovementDelta != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.identity);

            var skewedInput = matrix.MultiplyPoint3x4(m_MovementDelta);

            var relative = (transform.position + new Vector3(skewedInput.x, 0.0f, skewedInput.y) - transform.position);
            var rot = Quaternion.LookRotation(relative, Vector3.up);


            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, m_TurnSpeed * Time.deltaTime);
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_IsMovementLocked)
            ProcessInput();
    }

    private void Update()
    {
        Look();
    }
}
