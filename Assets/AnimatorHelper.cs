using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    PlayerMotor m_PlayerMotor;
    MonsterMode m_MosnterMode;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerMotor = GetComponentInParent<PlayerMotor>();
        m_MosnterMode = GetComponentInParent<MonsterMode>();
    }

    public void Attack()
    {
        m_MosnterMode.CalculateAttack();
    }
    public void ToggleMovementLock()
    {
        m_PlayerMotor.ToggleMovementLock();
    }
}
