using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class NodeAI_Agent : MonoBehaviour
{
    [HideInInspector]
    public Node.StateType currentState;
    [HideInInspector]
    public Node currentStateEntryNode;
    public AIController NodeAIController;
    [HideInInspector]
    public AIController controller;
    private float delayTimer;
    [HideInInspector]
    public Node currentSequenceNode;
    [HideInInspector]
    public Node previousSequenceNode;
    [SerializeField]
    public List<Action> actions;

    public NavMeshAgent agent;



    [System.Serializable]
    public struct Action
    {
        public string name;
        public UnityEvent action;
    }

    void Start()
    {
        controller = Instantiate(NodeAIController);
        if(controller != null)
        {
            foreach(Node n in controller.nodes)
            {
                n.ReconnectLinks(controller);
            }
            
        }
        currentState = Node.StateType.Idle;
        currentStateEntryNode = controller.nodes[0];
        currentSequenceNode = currentStateEntryNode;
        previousSequenceNode = currentStateEntryNode;
        
    }

    void Update()
    {
        //AI Update
        if(delayTimer > 0.0f)
        {
            delayTimer -= Time.deltaTime;
        }
        else
        {
            if(currentSequenceNode != null) HandleNode(currentSequenceNode);
        }

        //State Logic
        switch(currentState)
        {
            case Node.StateType.Idle:
                DoIdleState();
                break;
            case Node.StateType.Seek:
                DoSeekState();
                break;
            case Node.StateType.Flee:
                DoFleeState();
                break;
            case Node.StateType.Wander:
                DoWanderState();
                break;
            default:
                break;
        }
    }

    private void DoWanderState()
    {
        //Do Wander State
        if(agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(GetWanderTarget());
        }
        agent.speed = currentStateEntryNode.stateVars.speed;
        
    }

    private Vector3 GetWanderTarget()
    {
        Vector3 wanderTarget = transform.position + (currentStateEntryNode.stateVars.radius * transform.forward) + Random.insideUnitSphere * currentStateEntryNode.stateVars.radius;
        NavMeshHit hit;
        NavMesh.SamplePosition(wanderTarget, out hit, currentStateEntryNode.stateVars.radius, 1);
        return hit.position;
    }

    private void DoFleeState()
    {
        //Do Flee State
        if(agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(GetFleeTarget(currentStateEntryNode.stateVars.tag));
        }
        agent.speed = currentStateEntryNode.stateVars.speed;
    }

    //Flee from objects with a given tag
    private Vector3 GetFleeTarget(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        Vector3 fleeTarget = transform.position;
        float closestDistance = float.MaxValue;
        foreach(GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                fleeTarget = obj.transform.position;
            }
        }
        //Get a point away from the flee target
        Vector3 fleeDirection = (transform.position - fleeTarget).normalized;
        Vector3 fleePoint = transform.position + (fleeDirection * currentStateEntryNode.stateVars.radius);
        NavMeshHit hit;
        NavMesh.SamplePosition(fleePoint, out hit, currentStateEntryNode.stateVars.radius, 1);
        return hit.position;
    }

    private void DoIdleState()
    {
        //Do Idle State
        if(agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(transform.position);
        }
    }

    private void DoSeekState()
    {
        
        agent.SetDestination(GetSeekTarget(currentStateEntryNode.stateVars.tag));
        agent.speed = currentStateEntryNode.stateVars.speed;
    }

    //Seek to objects with a given tag
    private Vector3 GetSeekTarget(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        Vector3 seekTarget = transform.position;
        float closestDistance = float.MaxValue;
        foreach(GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                seekTarget = obj.transform.position;
            }
        }
        return seekTarget;
    }

    private void SetCurrentSequenceNode(Node node)
    {
        previousSequenceNode = currentSequenceNode;
        currentSequenceNode = node;
    }

    public void HandleNode(Node node)
    {
        switch (node.type)
        {
            
            case Node.NodeType.Action:
                bool foundAction = false;
                string name = node.fields[0].svalue;
                foreach(Action a in actions)
                {
                    if(a.name == name)
                    {
                        a.action.Invoke();
                        foundAction = true;
                        break;
                    }
                }
                if(!foundAction) Debug.LogWarning("Action of name: \"" + name + "\" could not be found! Please check the NodeAI_Agent");
                SetCurrentSequenceNode(FindNextSequenceNode());
                break;
            case Node.NodeType.Condition:
                HandleConditionNode(node);
                break;
            case Node.NodeType.State:
                currentState = node.stateType;
                currentStateEntryNode = node;
                SetCurrentSequenceNode(FindNextSequenceNode());
                break;
            case Node.NodeType.Entry:
                SetCurrentSequenceNode(FindNextSequenceNode());
                break;
            case Node.NodeType.Delay:
                if(node.fields[0].input.linkIDs.Count > 0)
                {
                    delayTimer = controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).parameter.fvalue;
                }
                else
                {
                    delayTimer = node.fields[0].fvalue;
                }
                SetCurrentSequenceNode(FindNextSequenceNode());
                break;
            
            
        }
    }

    public Node FindNextSequenceNode()
    {
        if(currentSequenceNode.seqOutput.linkIDs.Count > 0)
        {
            return controller.GetNodeFromID(controller.GetLinkFromID(currentSequenceNode.seqOutput.linkIDs[0]).output.NodeID);
        }
        else
        {
            return currentStateEntryNode;
        }
    }

    public void SetBool(string name, bool value)
    {
        foreach(Node n in controller.nodes)
        {
            if(n.type == Node.NodeType.Parameter)
            {
                if(n.parameter.type == AIController.Parameter.ParameterType.Bool)
                {
                    if(n.parameter.name == name)
                    {
                        n.parameter.bvalue = value;
                    }
                }
            }
        }
    }

    public void SetFloat(string name, float value)
    {
        foreach(Node n in controller.nodes)
        {
            if(n.type == Node.NodeType.Parameter)
            {
                if(n.parameter.type == AIController.Parameter.ParameterType.Float)
                {
                    if(n.parameter.name == name)
                    {
                        n.parameter.fvalue = value;
                    }
                }
            }
        }
    }

    public void SetInt(string name, int value)
    {
        foreach(Node n in controller.nodes)
        {
            if(n.type == Node.NodeType.Parameter)
            {
                if(n.parameter.type == AIController.Parameter.ParameterType.Int)
                {
                    if(n.parameter.name == name)
                    {
                        n.parameter.ivalue = value;
                    }
                }
            }
        }
    }

    private void HandleParameterNode(Node node)
    {
        
            foreach(string s in node.miscOutput.linkIDs)
            {
                Link link = controller.GetLinkFromID(s);
                if(link != null)
                {
                    controller.GetNodeFromID(link.output.NodeID).fields[link.output.fieldIndex].bvalue = node.parameter.bvalue;
                    controller.GetNodeFromID(link.output.NodeID).fields[link.output.fieldIndex].fvalue = node.parameter.fvalue;
                    controller.GetNodeFromID(link.output.NodeID).fields[link.output.fieldIndex].ivalue = node.parameter.ivalue;
                }
            }
            
        
    }

    private void HandleConditionNode(Node node)
    {
        if(node.fields[0].input.linkIDs.Count != 0)
        {
            Node n = controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID);
            if(n.type == Node.NodeType.Logic)
            {
                node.fields[0].bvalue = ComputeLogicNode(n);
            }
            else if(n.type == Node.NodeType.Parameter)
            {
                node.fields[0].bvalue = n.parameter.bvalue;
            }
            else if(n.type == Node.NodeType.Comparison)
            {
                node.fields[0].bvalue = ComputeComparisonNode(n);
            }
        }
        if(node.fields[0].bvalue)
        {
            if(node.conditionTrueOutput.linkIDs.Count > 0)
            {
                SetCurrentSequenceNode(controller.GetNodeFromID(controller.GetLinkFromID(node.conditionTrueOutput.linkIDs[0]).output.NodeID));
            }
            else
            {
                currentSequenceNode = previousSequenceNode;
            }
            
        }
        else
        {
            if(node.conditionFalseOutput.linkIDs.Count > 0)
            {
                SetCurrentSequenceNode(controller.GetNodeFromID(controller.GetLinkFromID(node.conditionFalseOutput.linkIDs[0]).output.NodeID));
            }
            else
            {
                currentSequenceNode = previousSequenceNode;
            }
        }

        
    }

    private bool ComputeComparisonNode(Node node)
    {
        if(node.fields[0].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Parameter)
        {
            node.fields[0].fvalue = controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).parameter.fvalue;
        }
        if(node.fields[1].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Parameter)
        {
            node.fields[1].fvalue = controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).parameter.fvalue;
        }
        float A = node.fields[0].fvalue;
        float B = node.fields[1].fvalue;
        

        switch(node.comparisonType)
        {
            case Node.ComparisonType.Equal:
                return A == B;
            case Node.ComparisonType.NotEqual:
                return A != B;
            case Node.ComparisonType.Greater:
                return A > B;
            case Node.ComparisonType.GreaterEqual:
                return A >= B;
            case Node.ComparisonType.Less:
                return A < B;
            case Node.ComparisonType.LessEqual:
                return A <= B;
            default:
                return false;
        }
    }
    private bool ComputeLogicNode(Node node)
    {
        if(node.fields[0].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Parameter)
        {
            node.fields[0].bvalue = controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).parameter.bvalue;
        }
        else if(node.fields[0].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Logic)
        {
            node.fields[0].bvalue = ComputeLogicNode(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.node);
        }
        else if(node.fields[0].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Comparison)
        {
            node.fields[0].bvalue = ComputeComparisonNode(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.node);
        }
        if(node.fields[1].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Parameter)
        {
            node.fields[1].bvalue = controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).parameter.bvalue;
        }
        else if(node.fields[1].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Logic)
        {
            node.fields[1].bvalue = ComputeLogicNode(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.node);
        }
        else if(node.fields[1].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Comparison)
        {
            node.fields[1].bvalue = ComputeComparisonNode(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.node);
        }
        bool A = node.fields[0].bvalue;
        bool B = node.fields[1].bvalue;
        

        switch (node.logicType)
        {
            case Node.LogicType.AND:
                return A && B;
            case Node.LogicType.OR:
                return A || B;
            case Node.LogicType.XOR:
                return A ^ B;
            case Node.LogicType.NOT:
                return !A;
            default:
                return false;
        }
    }



    


}
