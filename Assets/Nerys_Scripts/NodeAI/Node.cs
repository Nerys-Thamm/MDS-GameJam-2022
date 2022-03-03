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

    public LinkPoint miscOutput;

    public Action<Node> OnRemove;

    public List<NodeField> fields;


    public enum NodeType
    {
        State,
        Condition,
        Action,
        Logic,
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

        public LinkPoint input;
        public LinkPoint output;
        public NodeField(Node parent, string name, string value)
        {
            this.name = name;
            this.svalue = value;
            this.type = FieldType.String;
            
        }
        public NodeField(Node parent, string name, int value, bool hasInput, Action<LinkPoint> OnClickInput, GUIStyle inputStyle, bool hasOutput, Action<LinkPoint> OnClickOutput, GUIStyle outputStyle)
        {
            this.name = name;
            this.ivalue = value;
            this.type = FieldType.Int;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            if(hasInput)
            {
                this.input = new LinkPoint(parent, LinkType.Input, LinkDataType.Int, inputStyle, OnClickInput);
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(parent, LinkType.Output, LinkDataType.Int, outputStyle, OnClickOutput);
            }
        }
        
        public NodeField(Node parent, string name, float value, bool hasInput, Action<LinkPoint> OnClickInput, GUIStyle inputStyle, bool hasOutput, Action<LinkPoint> OnClickOutput, GUIStyle outputStyle)
        {
            this.name = name;
            this.fvalue = value;
            this.type = FieldType.Int;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            if(hasInput)
            {
                this.input = new LinkPoint(parent, LinkType.Input, LinkDataType.Int, inputStyle, OnClickInput);
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(parent, LinkType.Output, LinkDataType.Int, outputStyle, OnClickOutput);
            }
        }
        public NodeField(Node parent, string name, bool value, bool hasInput, Action<LinkPoint> OnClickInput, GUIStyle inputStyle, bool hasOutput, Action<LinkPoint> OnClickOutput, GUIStyle outputStyle)
        {
            this.name = name;
            this.bvalue = value;
            this.type = FieldType.Int;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            if(hasInput)
            {
                this.input = new LinkPoint(parent, LinkType.Input, LinkDataType.Int, inputStyle, OnClickInput);
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(parent, LinkType.Output, LinkDataType.Int, outputStyle, OnClickOutput);
            }
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
            EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)), 80, 20), name + ":");
                
            if(type == FieldType.String)
            {
                svalue = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)), rect.width - 90, 20), svalue);
            }
            else if(type == FieldType.Int)
            {
                ivalue = EditorGUI.IntField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)),rect.width - 90, 20), ivalue);
            }
            else if(type == FieldType.Float)
            {
                fvalue = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)),rect.width - 90, 20), fvalue);
            }
            else if(type == FieldType.Bool)
            {
                bvalue = EditorGUI.Toggle(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)),rect.width - 90, 20), bvalue);
            }

            if(input != null)
            {
                input.Draw(line+1);
            }
            if(output != null)
            {
                output.Draw(line+1);
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
            seqInput = new LinkPoint(this, LinkType.Input, LinkDataType.Sequence, inputStyle, OnClickInput);
            seqOutput = new LinkPoint(this, LinkType.Output,  LinkDataType.Sequence, outputStyle, OnClickOutput);
        }

        switch(type)
        {
            case NodeType.State:
                this.type = NodeType.State;
                this.title = "State";
                break;
            case NodeType.Condition:
                this.type = NodeType.Condition;
                this.title = "Condition";
                break;
            case NodeType.Action:
                this.type = NodeType.Action;
                this.title = "Action";
                break;
            case NodeType.Logic:
                this.type = NodeType.Logic;
                this.title = "Logic";
                break;
            case NodeType.Delay:
                this.type = NodeType.Delay;
                this.title = "Delay";
                this.fields = new List<NodeField>()
                {
                    new NodeField(this, "Delay", 0, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle)
                };
                break;
            case NodeType.Parameter:
                if(this.parameter == null)
                {
                    this.parameter = new AIController.Parameter();
                    this.parameter.name = "Parameter";
                    this.parameter.type = AIController.Parameter.ParameterType.Float;
                    this.parameter.fvalue = 0;
                }
                this.type = NodeType.Parameter;
                this.miscOutput = new LinkPoint(this, LinkType.Output, (LinkDataType)this.parameter.type, outputStyle, OnClickOutput);
                this.title = "Parameter";
                break;
            case NodeType.Entry:
                this.type = NodeType.Entry;
                this.title = "Entry";
                break;
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
        seqOutput = new LinkPoint(this, LinkType.Output, LinkDataType.Sequence, outputStyle, OnClickOutput);

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
        GUI.Box(rect, "", style);
        EditorGUI.LabelField(new Rect(rect.x + 15, rect.y + 5, 80, 20), title);
        switch(type)
        {
            case NodeType.State:
                title = EditorGUI.TextField(new Rect(rect.x + rect.width - 60, rect.y + 5, 60, 20), title);
                break;
            case NodeType.Parameter:
                
                EditorGUI.LabelField(new Rect(rect.x + 15 , rect.y + 5 + EditorGUIUtility.singleLineHeight, 80, 20), "Type:");
                parameter.type = (AIController.Parameter.ParameterType)EditorGUI.EnumPopup(new Rect(rect.x + 85, rect.y + 5 + EditorGUIUtility.singleLineHeight, rect.width - 100, 20), parameter.type);
                miscOutput.dataType = (LinkDataType)parameter.type;
                EditorGUI.LabelField(new Rect(rect.x + 15, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), 80, 20), "Name:");
                parameter.name = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 2), rect.width - 100, 20), parameter.name);
                EditorGUI.LabelField(new Rect(rect.x + 15, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), 80, 20), "Value:");
                switch(parameter.type)
                {
                    case AIController.Parameter.ParameterType.Float:
                        parameter.fvalue = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20), parameter.fvalue);
                        break;
                    case AIController.Parameter.ParameterType.Int:
                        parameter.ivalue = EditorGUI.IntField(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20), parameter.ivalue);
                        break;
                    case AIController.Parameter.ParameterType.Bool:
                        parameter.bvalue = EditorGUI.Toggle(new Rect(rect.x + 85, rect.y + 5 + ( EditorGUIUtility.singleLineHeight * 3), rect.width - 100, 20), parameter.bvalue);
                        break;
                }
                miscOutput.Draw(2);
                break;
        }
        if(fields != null)
        {
            for(int i = 0; i < fields.Count; i++)
            {
                fields[i].Draw(i, rect);
            }
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

