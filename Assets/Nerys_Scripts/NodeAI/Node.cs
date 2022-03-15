using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;


[System.Serializable]
public class NodeEvent : UnityEvent<Node> { };
//namespace NodeAI;
[System.Serializable]
public class Node 
{
    public string ID;
    [SerializeField]
    public Rect rect;
    public string title;
    public bool isDragging;
    public bool selected;
    [SerializeField]
    public GUIStyle style;
    [SerializeField]
    public GUIStyle defaultStyle;
    [SerializeField]
    public GUIStyle selectedStyle;
    [SerializeField]
    public LinkPoint seqInput;
    [SerializeField]
    public LinkPoint seqOutput;
    [SerializeField]
    public LinkPoint miscOutput;
    [SerializeField]
    public LinkPoint conditionTrueOutput, conditionFalseOutput;
    [SerializeField]
    public NodeEvent OnRemove;
    [SerializeField]
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
    [Serializable]
    public class NodeField
    {
        public FieldType type;
        public int index = 0;
        public string name;
        public string svalue;
        public int ivalue;
        public float fvalue;
        public bool bvalue;

        public bool hasInput;
        public bool hasOutput;
        [SerializeField]
        public LinkPoint input;
        [SerializeField]
        public LinkPoint output;
        public NodeField(string name, string value, int index)
        {
            this.name = name;
            this.svalue = value;
            this.type = FieldType.String;
            this.index = index;
            
        }
        public NodeField(string ID, string name, int value, bool hasInput, LinkPointEvent OnClickInput, GUIStyle inputStyle, bool hasOutput, LinkPointEvent OnClickOutput, GUIStyle outputStyle, int index)
        {
            this.name = name;
            this.ivalue = value;
            this.type = FieldType.Int;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.index = index;
            if(hasInput)
            {
                this.input = new LinkPoint(ID, LinkType.Input, LinkDataType.Int, inputStyle, OnClickInput);
                this.input.fieldIndex = index;
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(ID, LinkType.Output, LinkDataType.Int, outputStyle, OnClickOutput);
                this.output.fieldIndex = index;
            }
        }
        
        public NodeField(string ID, string name, float value, bool hasInput, LinkPointEvent OnClickInput, GUIStyle inputStyle, bool hasOutput, LinkPointEvent OnClickOutput, GUIStyle outputStyle, int index)
        {
            this.name = name;
            this.fvalue = value;
            this.type = FieldType.Float;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.index = index;
            if(hasInput)
            {
                this.input = new LinkPoint(ID, LinkType.Input, LinkDataType.Float, inputStyle, OnClickInput);
                this.input.fieldIndex = index;
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(ID, LinkType.Output, LinkDataType.Float, outputStyle, OnClickOutput);
                this.output.fieldIndex = index;
            }
        }
        public NodeField(string ID, string name, bool value, bool hasInput, LinkPointEvent OnClickInput, GUIStyle inputStyle, bool hasOutput, LinkPointEvent OnClickOutput, GUIStyle outputStyle, int index)
        {
            this.name = name;
            this.bvalue = value;
            this.type = FieldType.Bool;
            this.hasInput = hasInput;
            this.hasOutput = hasOutput;
            this.index = index;
            if(hasInput)
            {
                this.input = new LinkPoint(ID, LinkType.Input, LinkDataType.Bool, inputStyle, OnClickInput);
                this.input.fieldIndex = index;
            }
            if(hasOutput)
            {
                this.output = new LinkPoint(ID, LinkType.Output, LinkDataType.Bool, outputStyle, OnClickOutput);
                this.output.fieldIndex = index;
            }
        }
        public void RelinkEvents(LinkPointEvent OnClickInput, LinkPointEvent OnClickOutput)
        {
            this.input.ReconnectEvents(OnClickInput);
            this.output.ReconnectEvents(OnClickOutput);
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
                svalue = EditorGUI.TextField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)), rect.width - 100, 20), svalue);
            }
            else if(type == FieldType.Int)
            {
                ivalue = EditorGUI.IntField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)),rect.width - 100, 20), ivalue);
            }
            else if(type == FieldType.Float)
            {
                fvalue = EditorGUI.FloatField(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)),rect.width - 100, 20), fvalue);
            }
            else if(type == FieldType.Bool)
            {
                bvalue = EditorGUI.Toggle(new Rect(rect.x + 85, rect.y + 5 + ( (EditorGUIUtility.singleLineHeight + 5) * (1+line)),rect.width - 100, 20), bvalue);
            }

            if(input != null)
            {
                input.Draw(line+1, rect);
            }
            if(output != null)
            {
                output.Draw(line+1, rect);
            }
        }
    }


    //TYPE
    public NodeType type;

    //TYPE SPECIFIC VARS:

    //Parameter Node:
    public AIController.Parameter parameter;

    //Logic Node:
    public enum LogicType
    {
        AND,
        OR,
        NOT,
        XOR
    }
    public LogicType logicType;

    //State Node:
    public enum StateType
    {
        Idle,
        Seek,
        Flee,
        Wander
    }

    public StateType stateType;

    public void RelinkEvents(LinkPointEvent OnClickInput, LinkPointEvent OnClickOutput, NodeEvent OnClickRemove)
    {
        if(seqInput != null) seqInput.ReconnectEvents(OnClickInput);
        if(seqOutput != null) seqOutput.ReconnectEvents(OnClickOutput);
        if(miscOutput != null) miscOutput.ReconnectEvents(OnClickOutput);
        if(conditionTrueOutput != null) conditionTrueOutput.ReconnectEvents(OnClickOutput);
        if(conditionFalseOutput != null) conditionFalseOutput.ReconnectEvents(OnClickOutput);
        foreach(NodeField field in fields)
        {
            field.RelinkEvents(OnClickInput, OnClickOutput);
        }
        OnRemove = OnClickRemove;
    }

    public void ReconnectLinks(AIController controller)
    {
        if(seqInput != null) seqInput.ReconnectLinks(controller);
        if(seqOutput != null) seqOutput.ReconnectLinks(controller);
        if(miscOutput != null) miscOutput.ReconnectLinks(controller);
        if(conditionTrueOutput != null) conditionTrueOutput.ReconnectLinks(controller);
        if(conditionFalseOutput != null) conditionFalseOutput.ReconnectLinks(controller);
        foreach(NodeField field in fields)
        {
            if(field.input != null) field.input.ReconnectLinks(controller);
            if(field.output != null) field.output.ReconnectLinks(controller);
        }
    }
    public Node(
        Vector2 position, 
        float width, 
        float height, 
        GUIStyle nodeStyle, 
        GUIStyle selectedStyle,
        GUIStyle inputStyle, 
        GUIStyle outputStyle, 
        LinkPointEvent OnClickInput, 
        LinkPointEvent OnClickOutput,
        NodeEvent OnClickRemove,
        NodeType type,
        bool hasSequenceLinks = true
        )
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        this.ID = AIController.GenerateRandomString(20);
        if(hasSequenceLinks)
        {
            seqInput = new LinkPoint(ID, LinkType.Input, LinkDataType.Sequence, inputStyle, OnClickInput);
            seqOutput = new LinkPoint(ID, LinkType.Output,  LinkDataType.Sequence, outputStyle, OnClickOutput);
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
                conditionTrueOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Sequence, outputStyle, OnClickOutput);
                conditionFalseOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Sequence, outputStyle, OnClickOutput);
                seqOutput = null;
                this.fields = new List<NodeField>()
                {
                    new NodeField(this.ID, "Input", false, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 0)
                };
                break;
            case NodeType.Action:
                this.type = NodeType.Action;
                this.title = "Action";
                this.fields = new List<NodeField>()
                {
                    new NodeField("Name", "New Action", 0)
                };
                break;
            case NodeType.Logic:
                this.type = NodeType.Logic;
                this.title = "Logic";
                this.fields = new List<NodeField>()
                {
                    new NodeField(this.ID, "Input A", false, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 0),
                    new NodeField(this.ID, "Input B", false, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 1)
                };
                this.miscOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Bool, outputStyle, OnClickOutput);
                break;
            case NodeType.Delay:
                this.type = NodeType.Delay;
                this.title = "Delay";
                this.fields = new List<NodeField>()
                {
                    new NodeField(this.ID, "Delay", 0.0f, true, OnClickInput, inputStyle, false, OnClickOutput, outputStyle, 0)
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
                this.seqOutput = null;
                this.seqInput = null;
                this.type = NodeType.Parameter;
                this.miscOutput = new LinkPoint(this.ID, LinkType.Output, (LinkDataType)this.parameter.type, outputStyle, OnClickOutput);
                this.title = "Parameter";
                this.parameter.name = "NewParam";
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
        LinkPointEvent OnClickOutput
        )
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        this.ID = "EntryNode";
        seqInput = null;
        seqOutput = new LinkPoint(this.ID, LinkType.Output, LinkDataType.Sequence, outputStyle, OnClickOutput);

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
            seqInput.Draw(0, rect);
        if(seqOutput != null)
            seqOutput.Draw(0, rect);
        GUI.Box(rect, "", style);
        EditorGUI.LabelField(new Rect(rect.x + 15, rect.y + 5, 80, 20), title);
        switch(type)
        {
            case NodeType.State:
                
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
                miscOutput.Draw(2, rect);
                break;
            case NodeType.Logic:
                miscOutput.Draw(fields.Count, rect);
                switch(logicType)
                {
                    case LogicType.AND:
                    title = "AND | Logic";
                    break;
                    case LogicType.OR:
                    title = "OR | Logic";
                    break;
                    case LogicType.NOT:
                    title = "NOT | Logic";
                    break;
                    case LogicType.XOR:
                    title = "XOR | Logic";
                    break;
                }
                break;
            case NodeType.Condition:
                title = "IF | Condition";
                conditionTrueOutput.Draw(2, rect);
                EditorGUI.LabelField(new Rect(rect.x + (rect.width - 45), (rect.y - 5) + ( EditorGUIUtility.singleLineHeight * 3), 80, 20), "True:");
                conditionFalseOutput.Draw(3, rect);
                EditorGUI.LabelField(new Rect(rect.x + (rect.width - 45), (rect.y - 5) + ( EditorGUIUtility.singleLineHeight * 4), 80, 20), "False:");
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
            OnRemove.Invoke(this);
        }
    }

    

    
}

