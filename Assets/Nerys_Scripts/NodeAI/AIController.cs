using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AIController : ScriptableObject
{
    [System.Serializable]
    public class Parameter
    {
        public string name = "s";
        public float fvalue = 0;
        public int ivalue = 0;
        public bool bvalue = false;
        [System.Serializable]
        public enum ParameterType
        {
            Float = 0,
            Int = 1,
            Bool = 2
        }
        [SerializeField]
        public ParameterType type = ParameterType.Float;

    }
    [SerializeField]
    public List<Node> nodes;
    [SerializeField]
    public List<Link> links;
    [SerializeField]
    public List<Parameter> parameters;
    [SerializeField]
    public Dictionary<string, Node> nodeDictionary;

    public Node GetNodeFromID(string id)
    {
        if(nodeDictionary.ContainsKey(id))
        {
            return nodeDictionary[id];
        }
        return null;
    }

    private string GenerateRandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string result = "";
        for(int i = 0; i < length; i++)
        {
            result += chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return result;
    }

    public void AddNode(Node node)
    {
        nodes.Add(node);
        if(node.ID == null || node.ID == "")
        {
            node.ID = GenerateRandomString(20);
        }
        if(nodeDictionary == null) nodeDictionary = new Dictionary<string, Node>();
        nodeDictionary.Add(node.ID, node);
    }

    public void RemoveNode(Node node)
    {
        nodes.Remove(node);
        if(nodeDictionary == null) nodeDictionary = new Dictionary<string, Node>();
        nodeDictionary.Remove(node.ID);
    }

    public void ReconnectNodes()
    {
        foreach(Link link in links)
        {
            if(link.input.node == null)
            {
                link.input.node = GetNodeFromID(link.input.NodeID);
            }
        }
        
    }

    public void ReconnectLinks()
    {
        foreach(Link link in links)
        {
            if(!link.input.links.Contains(link))
            {
                link.input.links.Add(link);
            }
            if(!link.output.links.Contains(link))
            {
                link.input.links.Add(link);
            }
                
            
        }
    }

    
}
