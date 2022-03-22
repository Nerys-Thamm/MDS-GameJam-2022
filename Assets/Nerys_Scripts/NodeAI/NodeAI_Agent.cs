using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class NodeAI_Agent : MonoBehaviour
{
    [HideInInspector]
    public Node.StateType currentState; // The current state of the agent
    [HideInInspector]
    public Node currentStateEntryNode; // The node that the agent entered the current state from
    public AIController NodeAIController; // The controller for the agent
    [HideInInspector]
    public AIController controller; // The controller for the agent (Local)
    private float delayTimer; // The delay timer for the agent
    [HideInInspector]
    public Node currentSequenceNode; // The current node in the sequence
    [HideInInspector]
    public Node previousSequenceNode; // The previous node in the sequence
    [SerializeField]
    public List<Action> actions; // The list of actions for the agent

    public NavMeshAgent agent; // The navmesh agent for the agent


    //Action
    //Description:
    //The action struct for the agent
    [System.Serializable]
    public struct Action
    {
        public string name; // The name of the action
        public UnityEvent action; // The action
    }
    
    //Start
    void Start()
    {
        controller = Instantiate(NodeAIController); // Create a copy of the controller
        if(controller != null) 
        {
            foreach(Node n in controller.nodes) 
            {
                n.ReconnectLinks(controller); // Reconnect the links for the nodes
            }
            
        }
        currentState = Node.StateType.Idle; // Set the current state to idle
        currentStateEntryNode = controller.nodes[0]; // Set the current state entry node to the first node
        currentSequenceNode = currentStateEntryNode; // Set the current sequence node to the current state entry node
        previousSequenceNode = currentStateEntryNode; // Set the previous sequence node to the current state entry node
        
    }

    //Update
    void Update()
    {
        //AI Update
        if(delayTimer > 0.0f) 
        {
            delayTimer -= Time.deltaTime; // Decrease the delay timer
        }
        else
        {
            if(currentSequenceNode != null) HandleNode(currentSequenceNode); // Handle the current sequence node
        }

        //State Logic
        switch(currentState) 
        {
            case Node.StateType.Idle: // If the agent is idle
                DoIdleState();
                break;
            case Node.StateType.Seek: // If the agent is seeking
                DoSeekState();
                break;
            case Node.StateType.Flee: // If the agent is fleeing
                DoFleeState();
                break;
            case Node.StateType.Wander: // If the agent is wandering
                DoWanderState();
                break;
            default:
                break;
        }
    }

    //DoWanderState
    //Description:
    //Handles the wander state for the agent
    private void DoWanderState()
    {
        //Do Wander State
        if(agent.remainingDistance < 0.1f) 
        {
            agent.SetDestination(GetWanderTarget()); // Set the destination to the wander target
        }
        agent.speed = currentStateEntryNode.stateVars.speed; // Set the speed to the state speed
        
    }

    //GetWanderTarget
    //Description:
    //Gets the wander target for the agent
    private Vector3 GetWanderTarget()
    {
        //Use wander algorithm to find a new position and sample it on the navmesh
        Vector3 wanderTarget = transform.position + (currentStateEntryNode.stateVars.radius * transform.forward) + Random.insideUnitSphere * currentStateEntryNode.stateVars.radius;
        NavMeshHit hit;
        NavMesh.SamplePosition(wanderTarget, out hit, currentStateEntryNode.stateVars.radius, 1);
        return hit.position;
    }

    //DoFleeState
    //Description:
    //Handles the flee state for the agent
    private void DoFleeState()
    {
        //Do Flee State
        if(agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(GetFleeTarget(currentStateEntryNode.stateVars.tag));
        }
        agent.speed = currentStateEntryNode.stateVars.speed;
    }

    //GetFleeTarget
    //Parameters:
    //string tag - The tag of the target to flee
    //Description:
    //Gets the flee target for the agent
    private Vector3 GetFleeTarget(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag); // Get all objects with the tag
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

    //DoIdleState
    //Description:
    //Handles the idle state for the agent
    private void DoIdleState()
    {
        //Do Idle State
        if(agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(transform.position);
        }
    }

    //DoSeekState
    //Description:
    //Handles the seek state for the agent
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
            node.fields[0].bvalue = ComputeLogicNode(controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID));
        }
        else if(node.fields[0].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Comparison)
        {
            node.fields[0].bvalue = ComputeComparisonNode(controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID));
        }
        if(node.fields[1].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Parameter)
        {
            node.fields[1].bvalue = controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).parameter.bvalue;
        }
        else if(node.fields[1].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Logic)
        {
            node.fields[1].bvalue = ComputeLogicNode(controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID));
        }
        else if(node.fields[1].input.linkIDs.Count > 0 && controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Comparison)
        {
            node.fields[1].bvalue = ComputeComparisonNode(controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID));
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
