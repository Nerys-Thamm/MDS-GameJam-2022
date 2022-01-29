using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    Gamemanager m_Gamemanager;
    TMP_Text DeadKidCountText;

    void UpdateUIElements()
    {
        DeadKidCountText.text = m_Gamemanager.KidsEaten.ToString();
    }
  
    // Start is called before the first frame update
    void Start()
    {
        m_Gamemanager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Gamemanager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIElements();
    }
}
