using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public float CandyRadius = 5.0f;
    public LayerMask NPCmask;

    public float CandyLifeTime = 0.0f;
    public float MaxCandyLifeTime = 40.0f;

    public float Multiplier = 1.0f;
    void CheckAreKidsInRange()
    {
        Collider[] hitNPCS = Physics.OverlapSphere(transform.position, CandyRadius, NPCmask);

        float decayMultiplier = 0.0f;
        if(hitNPCS.Length > 0)
        {
            for (int i = 0; i < hitNPCS.Length; i++)
            {
                decayMultiplier += Multiplier;

            }

            CandyLifeTime -= decayMultiplier * Time.deltaTime;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, CandyRadius);
    }
    // Start is called before the first frame update
    void Start()
    {
        CandyLifeTime = MaxCandyLifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (CandyLifeTime <= 0)
        {
            Destroy(gameObject);
        }
        CheckAreKidsInRange();
    }
}
