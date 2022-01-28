using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PatrolAgent : MonoBehaviour
{
    NavMeshAgent m_agent;
    public MovementPath m_Path;
    public bool m_AlwaysMoving = true;
    bool m_IsMoving;
    public UnityEvent m_OnNodeReach;
    public UnityEvent m_OnReachStartNode;
    public UnityEvent m_OnReachLastNode;
    public float m_Speed;
    public float m_distancetonextnode;
    Vector3 m_direction;
    MovementPath.PathNode m_currentnode;
    MovementPath.PathNode m_nextnode;
    float m_timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_currentnode = m_Path.GetNode(0);
        m_nextnode = m_Path.GetNextNode();
        m_direction = (m_nextnode.Position - this.gameObject.transform.position);
        m_distancetonextnode = m_direction.magnitude;
        m_IsMoving = m_AlwaysMoving;
    }

    // Update is called once per frame
    void Update()
    {
        m_timer -= Time.deltaTime;
        m_direction = (m_nextnode.Position - this.gameObject.transform.position);
        m_distancetonextnode = m_direction.magnitude;
        if (m_distancetonextnode <= 2.0f)
        {
            m_currentnode = m_nextnode;
            m_nextnode = m_Path.GetNextNode();
            m_direction = (m_nextnode.Position - this.gameObject.transform.position);
            m_distancetonextnode = m_direction.magnitude;
            m_OnNodeReach.Invoke();
            if(m_currentnode == m_Path.GetNode(0))
            {
                m_OnReachStartNode.Invoke();
            }
            if (m_currentnode == m_Path.GetNode(m_Path.m_Path.Count-1))
            {
                m_OnReachLastNode.Invoke();
            }
        }
        if (m_IsMoving && m_timer < 0)
        {
            m_agent.destination = m_nextnode.Position;
        }
    }

    public void SetMoving(bool _IsMoving)
    {
        m_IsMoving = _IsMoving;
    }

    public void WaitFor(float _time)
    {
        m_timer = _time;
    }

}
