using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAI_Agent : MonoBehaviour
{
    public Node.StateType currentState;
    public Node currentStateEntryNode;
    public AIController controller;
    

    public Node currentSequenceNode;
    public Node previousSequenceNode;


    void Start()
    {
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

        if(currentSequenceNode != null) HandleNode(currentSequenceNode);
        
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
                //Do the action
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


    private bool ComputeLogicNode(Node node)
    {
        if(controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Parameter)
        {
            node.fields[0].bvalue = controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.node.parameter.bvalue;
        }
        else if(controller.GetNodeFromID(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Logic)
        {
            node.fields[0].bvalue = ComputeLogicNode(controller.GetLinkFromID(node.fields[0].input.linkIDs[0]).input.node);
        }
        if(controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Parameter)
        {
            node.fields[1].bvalue = controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.node.parameter.bvalue;
        }
        else if(controller.GetNodeFromID(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.NodeID).type == Node.NodeType.Logic)
        {
            node.fields[1].bvalue = ComputeLogicNode(controller.GetLinkFromID(node.fields[1].input.linkIDs[0]).input.node);
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