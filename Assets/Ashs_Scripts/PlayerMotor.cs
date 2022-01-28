using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] Rigidbody m_Rigid;
    [SerializeField] float m_MoveSpeed = 10.0f;

    private Vector3 m_MovementDelta;

    InputAction Movement;

    void ProcessInput()
    {
        float x = 0.0f, z = 0.0f;

        m_MovementDelta = Movement.ReadValue<Vector2>();

        x = m_MovementDelta.x;
        z = m_MovementDelta.y;
        if (m_MovementDelta !=Vector3.zero)
            m_Rigid.MovePosition(transform.position + transform.forward * m_MoveSpeed * Time.deltaTime);
    }

    void Look()
    {
        if (m_MovementDelta != Vector3.zero)
        {
            var relative = (transform.position + new Vector3(m_MovementDelta.x, 0.0f, m_MovementDelta.y) - transform.position);
            var rot = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = rot;
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
    void Update()
    {
        ProcessInput();
        Look();
    }
}
