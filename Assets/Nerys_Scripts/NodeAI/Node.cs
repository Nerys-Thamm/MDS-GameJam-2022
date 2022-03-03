using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//namespace NodeAI;

public class Node 
{
    public Rect rect;
    public string title;
    public bool isDragging;
    public bool selected;
    public GUIStyle style;
    public GUIStyle defaultStyle;
    public GUIStyle selectedStyle;

    public LinkPoint seqInput;
    public LinkPoint seqOutput;

    public Action<Node> OnRemove;

    public enum NodeType
    {
        State,
        Condition,
        Action,
        Sequence,
        Delay,
        Parameter,
        Entry
    }

    public class NodeField
    {
        public FieldType type;
        public string name;
        public string svalue;
        public int ivalue;
        public float fvalue;
        public bool bvalue;

        public bool hasInput;
        public bool hasOutput;
        public NodeField(string name, string value, bool hasInput, bool hasOutput)
        {
            this.name = name;
            this.svalue = value;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.type = FieldType.String;
        }
        public NodeField(string name, int value, bool hasInput, bool hasOutput)
        {
            this.name = name;
            this.ivalue = value;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.type = FieldType.Int;
        }
        public NodeField(string name, float value, bool hasInput, bool hasOutput)
        {
            this.name = name;
            this.fvalue = value;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.type = FieldType.Float;
        }
        public NodeField(string name, bool value, bool hasInput, bool hasOutput)
        {
            this.name = name;
            this.bvalue = value;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.type = FieldType.Bool;
        }

        public enum FieldType
        {
            String,
            Int,
            Float,
            Bool
        }

        public void Draw(int line, Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x + 5 , rect.y + 5 + ( EditorGUIUtility.singleLineHeight * (1+line)), 80, 20), name + ":");
                
            if(type == FieldType.String)
            {
                svalue = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * (1+line)), rect.width - 90, 20), svalue);
            }
            else if(type == FieldType.Int)
            {
                ivalue = EditorGUI.IntField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * (1+line)),rect.width - 90, 20), ivalue);
            }
            else if(type == FieldType.Float)
            {
                fvalue = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * (1+line)),rect.width - 90, 20), fvalue);
            }
            else if(type == FieldType.Bool)
            {
                bvalue = EditorGUI.Toggle(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * (1+line)),rect.width - 90, 20), bvalue);
            }
        }
    }


    //TYPE
    public NodeType type;

    //TYPE SPECIFIC VARS:

    //Parameter Node:
    public AIController.Parameter parameter;

    public Node(
        Vector2 position, 
        float width, 
        float height, 
        GUIStyle nodeStyle, 
        GUIStyle selectedStyle,
        GUIStyle inputStyle, 
        GUIStyle outputStyle, 
        Action<LinkPoint> OnClickInput, 
        Action<LinkPoint> OnClickOutput,
        Action<Node> OnClickRemove,
        NodeType type,
        bool hasSequenceLinks = true
        )
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        if(hasSequenceLinks)
        {
            seqInput = new LinkPoint(this, LinkType.Input, inputStyle, OnClickInput);
            seqOutput = new LinkPoint(this, LinkType.Output, outputStyle, OnClickOutput);
        }

        defaultStyle = nodeStyle;
        this.selectedStyle = selectedStyle;
        this.type = type;
        
        OnRemove = OnClickRemove;
    }

    public Node(
        Vector2 position, 
        float width, 
        float height, 
        GUIStyle nodeStyle, 
        GUIStyle selectedStyle, 
        GUIStyle outputStyle,  
        Action<LinkPoint> OnClickOutput
        )
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        seqInput = null;
        seqOutput = new LinkPoint(this, LinkType.Output, outputStyle, OnClickOutput);

        defaultStyle = nodeStyle;
        this.selectedStyle = selectedStyle;
        title = "Entry";
        type = NodeType.Entry;
        OnRemove = null;
    }

    public void Move(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        if(seqInput != null)
            seqInput.Draw(0);
        if(seqOutput != null)
            seqOutput.Draw(0);
        GUI.Box(rect, title, style);
        switch(type)
        {
            case NodeType.State:
                title = EditorGUI.TextField(new Rect(rect.x + rect.width - 60, rect.y + 5, 60, 20), title);
                break;
            case NodeType.Parameter:
                if(parameter == null)
                {
                    parameter = new AIController.Parameter();
                    parameter.name = "Parameter";
                    parameter.type = AIController.Parameter.ParameterType.Float;
                    parameter.fvalue = 0;
                }
                EditorGUI.LabelField(new Rect(rect.x + 5, rect.y + 5, 80, 20), "Parameter");
                EditorGUI.LabelField(new Rect(rect.x + 5 , rect.y + 5 + EditorGUIUtility.singleLineHeight, 80, 20), "Type:");
                parameter.type = (AIController.Parameter.ParameterType)EditorGUI.EnumPopup(new Rect(rect.x + 85, rect.y + 5 + EditorGUIUtility.singleLineHeight, rect.width - 90, 20), parameter.type);
                EditorGUI.LabelField(new Rect(rect.x + 5, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), 80, 20), "Name:");
                parameter.name = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), rect.width - 90, 20), parameter.name);
                EditorGUI.LabelField(new Rect(rect.x + 5, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), 80, 20), "Value:");
                switch(parameter.type)
                {
                    case AIController.Parameter.ParameterType.Float:
                        parameter.fvalue = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 90, 20), parameter.fvalue);
                        break;
                    case AIController.Parameter.ParameterType.Int:
                        parameter.ivalue = EditorGUI.IntField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 90, 20), parameter.ivalue);
                        break;
                    case AIController.Parameter.ParameterType.Bool:
                        parameter.bvalue = EditorGUI.Toggle(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 90, 20), parameter.bvalue);
                        break;
                }
                break;
        }
    }

    public bool ProcessEvents(Event e)
    {
        if(e.type == EventType.MouseDown)
        {
            if(e.button == 0)
            {
                if(rect.Contains(e.mousePosition))
                {
                    isDragging = true;
                    GUI.changed = true;
                    selected = true;
                    style = selectedStyle;
                }
                else
                {
                    GUI.changed = true;
                    selected = false;
                    style = defaultStyle;
                }
            }
            if(e.button == 1 && rect.Contains(e.mousePosition))
            {
                ProcessContextMenu();
                e.Use();
            }
        }
        else if(e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
        }
        else if(e.type == EventType.MouseDrag && e.button == 0 && isDragging)
        {
            Move(e.delta);
            e.Use();
            return true;
        }
            
        
        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnRemoveNode()
    {
        if(OnRemove != null)
        {
            OnRemove(this);
        }
    }
    
}

