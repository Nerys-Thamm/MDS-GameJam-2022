using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    PlayerMotor m_PlayerMotor;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerMotor = GetComponentInParent<PlayerMotor>();

    }

    public void ToggleMovementLock()
    {
        m_PlayerMotor.ToggleMovementLock();
    }
}
