using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeAI_Agent))]
public class NodeAI_AgentEditor : Editor
{
    NodeAI_Agent agent;
    SerializedObject serializedAgent;
    AIController controller;

    bool showParameters = false;

    void OnEnable()
    {
        agent = (NodeAI_Agent)target;
        controller = agent.controller;
        serializedAgent = new SerializedObject(agent);
        if(controller != null)
        {
            controller.parameters.Clear();
            foreach (Node n in controller.nodes)
            {
                if(n.type == Node.NodeType.Parameter)
                {
                    controller.parameters.Add(n.parameter);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        agent.NodeAIController = EditorGUILayout.ObjectField(agent.NodeAIController, typeof(AIController), true) as AIController;
        switch(agent.currentState)
        {
            case Node.StateType.Flee:
                EditorGUILayout.LabelField("Current State: Flee");
                break;
            case Node.StateType.Idle:
                EditorGUILayout.LabelField("Current State: Idle");
                break;
            case Node.StateType.Seek:
                EditorGUILayout.LabelField("Current State: Seek");
                break;
            case Node.StateType.Wander:
                EditorGUILayout.LabelField("Current State: Wander");
                break;
        }
        // SerializedProperty actionsProperty = serializedAgent.FindProperty("actions");
        // EditorGUILayout.PropertyField(actionsProperty, true);
        // serializedAgent.ApplyModifiedProperties();
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
