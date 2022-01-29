using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceAudio : MonoBehaviour
{
    AudioSource m_Audiosource;
    MonsterMode m_PlayerMode;

    [SerializeField] AudioClip m_MonsterAmbience;
    [SerializeField] AudioClip m_ChildAmbience;


    private void Start()
    {
        m_Audiosource = GetComponent<AudioSource>();
        m_PlayerMode = FindObjectOfType<MonsterMode>();
    }
    private void Update()
    {
        if (m_PlayerMode.Input_SwitchMode.triggered)
        {
            m_Audiosource.Stop();
            if (m_PlayerMode.GetMode())
            {
                m_Audiosource.PlayOneShot(m_MonsterAmbience);
            }
            else
            {
                m_Audiosource.PlayOneShot(m_ChildAmbience);
            }
        }
   
    }
}
