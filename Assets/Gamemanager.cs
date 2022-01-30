using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gamemanager : MonoBehaviour
{
    GameObject[] m_KidsList;

    MonsterMode m_Monstermode;

    public int Kidsmunched = 0;

    public TMP_Text m_KidsMuchedText;
    public TMP_Text m_CandyCollectedText;

    private void Start()
    {
        m_KidsList = GameObject.FindGameObjectsWithTag("NPC");
        m_Monstermode = FindObjectOfType<MonsterMode>();
    }

    // Update is called once per frame
    void Update()
    {
        m_KidsMuchedText.text = Kidsmunched.ToString();
        m_CandyCollectedText.text = m_Monstermode.CandyCount.ToString();
    }
}
