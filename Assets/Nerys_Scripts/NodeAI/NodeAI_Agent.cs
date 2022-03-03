using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAI_Agent : MonoBehaviour
{
    public Node.StateType currentState;
    public Node currentStateEntryNode;
    public AIController controller;
    

    public Node currentSequenceNode;

    public void HandleNode(Node node)
    {
        switch (node.type)
        {
            
            case Node.NodeType.Action:
                //HandleActionNode(node);
                break;
            case Node.NodeType.Condition:
                HandleConditionNode(node);
                break;
            case Node.NodeType.State:
                currentState = node.stateType;
                currentStateEntryNode = node;
                break;
            
        }
    }

    private void HandleConditionNode(Node node)
    {
        bool A = node.fields[0].bvalue;
    }

    private bool ComputeLogicNode(Node node)
    {
        if(node.fields[0].input.links[0].output.node.type == Node.NodeType.Parameter)
        {
            node.fields[0].bvalue = node.fields[0].input.links[0].output.node.parameter.bvalue;
        }
        else if(node.fields[0].input.links[0].output.node.type == Node.NodeType.Logic)
        {
            node.fields[0].bvalue = ComputeLogicNode(node.fields[0].input.links[0].output.node);
        }
        if(node.fields[1].input.links[0].output.node.type == Node.NodeType.Parameter)
        {
            node.fields[1].bvalue = node.fields[1].input.links[0].output.node.parameter.bvalue;
        }
        else if(node.fields[1].input.links[0].output.node.type == Node.NodeType.Logic)
        {
            node.fields[1].bvalue = ComputeLogicNode(node.fields[1].input.links[0].output.node);
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
