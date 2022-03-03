using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AIController : ScriptableObject
{
    public class Parameter
    {
        public string name = "";
        public float fvalue = 0;
        public int ivalue = 0;
        public bool bvalue = false;

        public enum ParameterType
        {
            Float = 0,
            Int = 1,
            Bool = 2
        }
        public ParameterType type = ParameterType.Float;

    }
    public List<Node> nodes;
    public List<Link> links;
    public List<Parameter> parameters;

    
}
