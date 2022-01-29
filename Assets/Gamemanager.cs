using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public int KidsEaten;

    public void OnNPCDeath()
    {
        KidsEaten++;
        FindObjectOfType<TrickOrTreaterAI>().deathEvent -= OnNPCDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<TrickOrTreaterAI>().deathEvent += OnNPCDeath;
        KidsEaten = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
