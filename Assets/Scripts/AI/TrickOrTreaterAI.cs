using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrickOrTreaterAI : MonoBehaviour
{
    public enum State
    {
        SEEKING_HOUSE, // Walk
        WAITING_FOR_TREAT, // Idle
        WANDERING, // Walk
        SEEKING_BAIT, // Walk
        EATING_BAIT, // Idle
        FLEEING, // running
    }

    //AI Parameters
    public float m_HouseSearchRadius = 10.0f;
    public float m_HouseSearchAngle = 30.0f;

    public float m_BaitSearchRadius = 10.0f;
    public float m_BaitSearchAngle = 30.0f;

    float m_BaitEatTimer = 0.0f;
    public float m_BaitEatTime = 5.0f;
    public float m_GoalReachRadius = 1.0f;

    public float m_WanderRadius = 10.0f;
    public float m_WanderForwardDistance = 10.0f;
    public int m_WanderUpdateRate = 10;
    int m_currentWanderUpdate = 0;

    public float m_WaitForTreatTime = 10.0f;
    public float m_WaitForTreatTimer = 0.0f;

    public float m_HouseSeekCooldown = 2.0f;
    public float m_HouseSeekTimer = 0.0f;

    public float m_fleeDuration = 5.0f;
    public float m_fleeTimer = 0.0f;

    public bool isAIEnabled = true;
    //Components
    NavMeshAgent m_agent;
    public Animator m_animator;
    public Animator m_indicator;

    //State Variables
    public State m_currentState = State.WANDERING;

    Vector3 m_targetHousePosition;
    Vector3 m_targetBaitPosition;

    Transform m_playerTransform;

    // Added Death SFX - Ash
    [SerializeField]
    SFX_Effect Death_Effect;

    bool isDead = false;
    public delegate void DeathDelegate();
    public DeathDelegate deathEvent;

    public void Death()
    {
        Debug.Log("DIE!");
        Death_Effect.Play();
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = false;
        }
        GetComponent<CapsuleCollider>().enabled = false;
        isAIEnabled = false;
        
        isDead = true;
    }

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
                    m_targetBaitPosition = collider.transform.position;
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

    //Get a random point within the wander radius pointing away from the player
    Vector3 GetRandomPointInFleeRadius()
    {
        Vector3 randomPoint = GetRandomPointInCircle(m_WanderRadius);
        randomPoint += transform.forward * m_WanderForwardDistance;
        randomPoint -= (m_playerTransform.position - transform.position).normalized * m_WanderForwardDistance;
        return randomPoint;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        m_playerTransform = GameObject.FindWithTag("Player").transform;
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_animator.SetTrigger("Walking");
    }

    bool CheckPlayerInViewCone()
    {
        Vector3 direction = m_playerTransform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle < m_HouseSearchAngle / 2.0f)
        {
            return true;
        }
        return false;
    }

    public void Scare()
    {
        if(m_currentState == State.FLEEING)
        {
            m_fleeTimer = 0;
            return;
        }
        m_currentState = State.FLEEING;
        m_animator.SetTrigger("seen monster");
        m_fleeTimer = 0.0f;
        Debug.Log(gameObject.name + m_currentState);
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (isDead && !Death_Effect.m_effectsList[0].m_particle.isPlaying)
        {
            StartCoroutine(DeathCoroutine());
        }
        if (isAIEnabled)
        {
            if(m_currentState == State.FLEEING)
            {
                m_agent.speed = 5.0f;
            }
            else
            {
                m_agent.speed = 3.0f;
            }
            switch (m_currentState)
            {
                case State.SEEKING_HOUSE:
                    
                    m_agent.SetDestination(m_targetHousePosition);
                    if (Vector3.Distance(transform.position, m_targetHousePosition) < m_GoalReachRadius)
                    {
                        m_currentState = State.WAITING_FOR_TREAT;
                        m_animator.SetTrigger("Idle");
                        //m_animator.SetBool("Walking", false);
                        m_agent.isStopped = true;
                    }
                    else if(CheckBaitInRange())
                    {
                        m_currentState = State.SEEKING_BAIT;
                        m_agent.SetDestination(m_targetBaitPosition);
                        m_agent.isStopped = false;
                        //m_animator.SetBool("Walking", false);
                        m_animator.SetTrigger("Walking");
                    }
                    break;
                case State.WAITING_FOR_TREAT:
                    m_WaitForTreatTimer += Time.deltaTime;
                    m_agent.isStopped = true;
                    if (m_WaitForTreatTimer > m_WaitForTreatTime)
                    {
                        m_currentState = State.WANDERING;
                        m_agent.isStopped = false;
                        m_animator.SetTrigger("Walking");
                        m_WaitForTreatTimer = 0.0f;
                        m_HouseSeekTimer = 0.0f;
                    }
                    break;
                case State.SEEKING_BAIT:
                    m_indicator.SetBool("InLureState", true);
                    m_agent.SetDestination(m_targetBaitPosition);
                    m_agent.isStopped = false;
                    if (Vector3.Distance(transform.position, m_targetBaitPosition) < m_GoalReachRadius)
                    {
                        m_currentState = State.EATING_BAIT;
                        m_animator.SetTrigger("Eat candy");
                        m_agent.isStopped = true;
                    }
                    break;
                case State.EATING_BAIT:
                    m_BaitEatTimer += Time.deltaTime;
                    m_agent.isStopped = true;
                    if (m_BaitEatTimer > m_BaitEatTime)
                    {
                        m_indicator.SetBool("InLureState", false);
                        m_currentState = State.WANDERING;
                        m_animator.SetTrigger("Walking");
                        m_agent.isStopped = false;
                        m_BaitEatTimer = 0.0f;
                        m_HouseSeekTimer = 0.0f;
                    }
                    break;
                case State.FLEEING:
                    m_indicator.SetBool("InFearState", true);
                    m_fleeTimer += Time.deltaTime;
                    if (m_fleeTimer > m_fleeDuration)
                    {
                        m_indicator.SetBool("InFearState", false);
                        m_currentState = State.WANDERING;
                        m_animator.SetTrigger("Walking");
                        m_fleeTimer = 0.0f;
                    }
                    else
                    {
                        if (m_currentWanderUpdate == m_WanderUpdateRate)
                        {
                            m_currentWanderUpdate = 0;
                            Vector3 randomPoint = transform.position + GetRandomPointInFleeRadius();
                            m_agent.SetDestination(randomPoint);
                        }
                        else
                        {
                            m_currentWanderUpdate++;
                        }
                    }
                    break;
                case State.WANDERING:
                    if (m_currentWanderUpdate == m_WanderUpdateRate)
                    {
                        m_currentWanderUpdate = 0;
                        Vector3 randomPoint = transform.position + GetRandomPointInWanderRadius();
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
                    else if(CheckBaitInRange())
                    {
                        m_currentState = State.SEEKING_BAIT;
                        m_agent.SetDestination(m_targetBaitPosition);
                        //m_animator.SetBool("Walking", false);
                    }
                    break;
            }
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
