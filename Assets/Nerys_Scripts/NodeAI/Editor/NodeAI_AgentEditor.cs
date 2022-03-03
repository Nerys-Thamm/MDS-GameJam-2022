using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeAI_Agent))]
public class NodeAI_AgentEditor : Editor
{
    NodeAI_Agent agent;
    AIController controller;

    bool showParameters = false;

    void OnEnable()
    {
        agent = (NodeAI_Agent)target;
        controller = agent.controller;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(controller != null)
        {   

            //Display every parameter in a dropdown menu
            showParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showParameters, "Parameters");
            if(showParameters && controller.parameters != null)
            {
                foreach (AIController.Parameter parameter in controller.parameters)
            {
                EditorGUILayout.LabelField("Name: \"" + parameter.name + "\"");
                switch(parameter.type)
                {
                    case AIController.Parameter.ParameterType.Bool:
                        parameter.bvalue = EditorGUILayout.Toggle(parameter.bvalue);
                        break;
                    case AIController.Parameter.ParameterType.Float:
                        parameter.fvalue = EditorGUILayout.FloatField(parameter.fvalue);
                        break;
                    case AIController.Parameter.ParameterType.Int:
                        parameter.ivalue = EditorGUILayout.IntField(parameter.ivalue);
                        break;
                }
                
            }
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
