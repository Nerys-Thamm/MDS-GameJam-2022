using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrickOrTreaterAI : MonoBehaviour
{
    public enum State
    {
        SEEKING_HOUSE,
        WAITING_FOR_TREAT,
        WANDERING,
        SEEKING_BAIT,
        EATING_BAIT,
        FLEEING,
    }

    //AI Parameters
    public float m_HouseSearchRadius = 10.0f;
    public float m_HouseSearchAngle = 30.0f;

    public float m_BaitSearchRadius = 10.0f;
    public float m_BaitSearchAngle = 30.0f;

    public float m_GoalReachRadius = 1.0f;

    public float m_WanderRadius = 10.0f;
    public float m_WanderForwardDistance = 10.0f;
    public int m_WanderUpdateRate = 10;
    int m_currentWanderUpdate = 0;

    public float m_WaitForTreatTime = 10.0f;
    public float m_WaitForTreatTimer = 0.0f;

    public float m_HouseSeekCooldown = 2.0f;
    public float m_HouseSeekTimer = 0.0f;

    //Components
    NavMeshAgent m_agent;
    Animator m_animator;

    //State Variables
    State m_currentState = State.WANDERING;

    Vector3 m_targetHousePosition;
    Vector3 m_targetBaitPosition;

    //Check if a House is within range and within the viewing angle
    bool CheckHouseInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_HouseSearchRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "House")
            {
                Vector3 direction = collider.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);
                if (angle < m_HouseSearchAngle / 2.0f)
                {
                    m_targetHousePosition = collider.transform.position;
                    return true;
                    
                }
            }
        }
        return false;
    }

    //Check if a Bait is within range and within the viewing angle
    bool CheckBaitInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_BaitSearchRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Bait")
            {
                Vector3 direction = collider.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);
                if (angle < m_BaitSearchAngle / 2.0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    Vector3 GetRandomPointInCircle(float radius)
    {
        float angle = Random.Range(0.0f, 360.0f);
        Vector3 randomPoint = new Vector3(Mathf.Sin(angle) * radius, 0.0f, Mathf.Cos(angle) * radius);
        return randomPoint;
    }

    //Get a random point within the wander radius
    Vector3 GetRandomPointInWanderRadius()
    {
        Vector3 randomPoint = GetRandomPointInCircle(m_WanderRadius);
        randomPoint += transform.forward * m_WanderForwardDistance;
        return randomPoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_currentState)
        {
            case State.SEEKING_HOUSE:
                m_agent.SetDestination(m_targetHousePosition);
                if (Vector3.Distance(transform.position, m_targetHousePosition) < m_GoalReachRadius)
                {
                    m_currentState = State.WAITING_FOR_TREAT;
                    //m_animator.SetBool("Walking", false);
                }
                break;
            case State.WAITING_FOR_TREAT:
                m_WaitForTreatTimer += Time.deltaTime;
                if (m_WaitForTreatTimer > m_WaitForTreatTime)
                {
                    m_currentState = State.WANDERING;
                    m_WaitForTreatTimer = 0.0f;
                    m_HouseSeekTimer = 0.0f;
                }
                break;
            case State.SEEKING_BAIT:
                if (CheckBaitInRange())
                {
                    m_currentState = State.EATING_BAIT;
                    //m_animator.SetBool("Walking", false);
                }
                break;
            case State.EATING_BAIT:
                break;
            case State.FLEEING:
                break;
            case State.WANDERING:
                if (m_currentWanderUpdate == m_WanderUpdateRate)
                {
                    m_currentWanderUpdate = 0;
                    Vector3 randomPoint = GetRandomPointInWanderRadius();
                    m_agent.SetDestination(randomPoint);
                }
                else
                {
                    m_currentWanderUpdate++;
                }
                m_HouseSeekTimer += Time.deltaTime;
                if (CheckHouseInRange())
                {
                    if (m_HouseSeekTimer > m_HouseSeekCooldown)
                    {
                        m_currentState = State.SEEKING_HOUSE;
                        
                    }
                    //m_animator.SetBool("Walking", false);
                }
                break;
        }
    }


    private void OnDrawGizmosSelected() {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_HouseSearchRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_BaitSearchRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_GoalReachRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_WanderForwardDistance, m_WanderRadius);
    }
}
