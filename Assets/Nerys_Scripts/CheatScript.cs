using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CheatScript : MonoBehaviour
{
    public UnityEvent OnCheatCodeSuccess;
    public float m_nextPressWindow = 0.2f;
    public float m_timer = 0.0f;

    public List<Key> m_sequence;

    public int currIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        OnCheatCodeSuccess.AddListener(() => {
            
        });
    }


    

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current[m_sequence[currIndex]].IsPressed())
        {
            if (currIndex == 0)
            {
                m_timer = m_nextPressWindow;
                currIndex++;
            }
            else if ((m_timer -= Time.deltaTime) > 0.0f)
            {
                m_timer = m_nextPressWindow;
                currIndex++;
                if (currIndex == m_sequence.Count)
                {
                    currIndex = 0;
                    OnCheatCodeSuccess.Invoke();
                    SetCursorState(true);
                }
            }
            else
            {
                m_timer = 0.0f;
                currIndex = 0;
            }
        }
    }

    public void SetCursorState(bool _visible)
    {
        Cursor.visible = _visible;
        Cursor.lockState = _visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}


