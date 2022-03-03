using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//namespace NodeAI;

public class NodeAIEditor : EditorWindow
{
    //Node UI


    private GUIStyle style, startNodeStyle, startNodeSelectedStyle;
    private GUIStyle inputStyle, outputStyle, selectedStyle;

    private LinkPoint selectedInput, selectedOutput;

    private Vector2 offset;

    //Window UI

    private bool creatingNewObj = false;
    private string newObjName = "";

    //Internal Data
    private AIController controller;

    [MenuItem("Window/NodeAI")]
    private static void OpenWindow()
    {
        NodeAIEditor window = GetWindow<NodeAIEditor>();
        window.titleContent = new GUIContent("NodeAI");
    }

    private void OnEnable()
    {
        style = new GUIStyle();
        style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        style.border = new RectOffset(12, 12, 12, 12);
        style.alignment = TextAnchor.MiddleCenter;

        startNodeStyle = new GUIStyle();
        startNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
        startNodeStyle.border = new RectOffset(12, 12, 12, 12);
        startNodeStyle.alignment = TextAnchor.MiddleCenter;

        startNodeSelectedStyle = new GUIStyle();
        startNodeSelectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
        startNodeSelectedStyle.border = new RectOffset(12, 12, 12, 12);
        startNodeSelectedStyle.alignment = TextAnchor.MiddleCenter;

        inputStyle = new GUIStyle();
        inputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inputStyle.border = new RectOffset(4, 4, 12, 12);
        

        outputStyle = new GUIStyle();
        outputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outputStyle.border = new RectOffset(4, 4, 12, 12);


        selectedStyle = new GUIStyle();
        selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedStyle.border = new RectOffset(12, 12, 12, 12);
        selectedStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnGUI()
    {
        if(controller != null)
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
            DrawNodes();
            DrawLinks();
            DrawLinkLine(Event.current);
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);
        }
        DrawUI();
        if(controller != null)
        {
            if(controller.nodes == null)
            {
                controller.nodes = new List<Node>();
            }
            if(controller.links == null)
            {
                controller.links = new List<Link>();
            }
            if(controller.nodes.Count == 0)
            {
                Node node = new Node(new Vector2(200, 200), 200, 50, startNodeStyle, startNodeSelectedStyle, outputStyle, OnClickOutput);
                controller.nodes.Add(node);
            }
        }

        

        if(GUI.changed) Repaint();
    }

    private void DrawUI()
    {
        EditorGUILayout.BeginHorizontal();
        if(creatingNewObj == false && GUILayout.Button("New"))
        {
            creatingNewObj = true;
        }
        if(creatingNewObj == true)
        {
            GUILayout.Label("Name");
            newObjName = GUILayout.TextField(newObjName);
            if(GUILayout.Button("Create") && newObjName != "")
            {
                creatingNewObj = false;
                CreateNewAIController(newObjName);
                newObjName = "";
                
                
                
            }
            if(GUILayout.Button("Cancel"))
            {
                creatingNewObj = false;
            }
        }
        else
        {
            controller = EditorGUILayout.ObjectField(controller, typeof(AIController), true) as AIController;
        }
        
        EditorGUILayout.EndHorizontal();

        
    }
    
    

    private void CreateNewAIController(string name)
    {
        controller = ScriptableObject.CreateInstance<AIController>();
        
        //AssetDatabase.CreateAsset(controller, "Assets/Resources/AI/" + name + ".asset");
        ProjectWindowUtil.CreateAsset(controller, newObjName + "_AICtrl.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    


//NODE UI CODE
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for(int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for(int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawLinkLine(Event e)
    {
        if(selectedInput != null && selectedOutput == null)
        {
            Handles.DrawBezier(
                selectedInput.rect.center,
                e.mousePosition,
                selectedInput.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if(selectedInput == null && selectedOutput != null)
        {
            Handles.DrawBezier(
                e.mousePosition,
                selectedOutput.rect.center,
                e.mousePosition + Vector2.left * 50f,
                selectedOutput.rect.center - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void DrawNodes()
    {
        if(controller.nodes != null)
        {
            foreach(Node node in controller.nodes)
            {
                node.Draw();
            }
        }
    }

    private void DrawLinks()
    {
        if(controller.links != null)
        {
            foreach(Link link in controller.links)
            {
                link.Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {

        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                if(e.button == 0)
                {
                    selectedInput = null;
                    selectedOutput = null;
                }
                break;
            case EventType.MouseDrag:
                if(e.button == 2)
                {
                    OnMoveCanvas(e.delta);
                }
                break;
        }
    }

    private void OnMoveCanvas(Vector2 delta)
    {
        offset += delta;

        if(controller.nodes != null)
        {
            foreach(Node node in controller.nodes)
            {
                node.Move(delta);
            }
        }

        GUI.changed = true;
    }

    private void ProcessNodeEvents(Event e)
    {
        if(controller.nodes != null)
        {
            for(int i = controller.nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = controller.nodes[i].ProcessEvents(e);

                if(guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Node/State"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            controller.nodes.Add(new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnClickInput, OnClickOutput, OnRemoveNode, Node.NodeType.State));
        });
        genericMenu.AddItem(new GUIContent("Add Node/Condition"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            controller.nodes.Add(new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnClickInput, OnClickOutput, OnRemoveNode, Node.NodeType.Condition));
        });
        genericMenu.AddItem(new GUIContent("Add Node/Action"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            controller.nodes.Add(new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnClickInput, OnClickOutput, OnRemoveNode, Node.NodeType.Action));
        });
        genericMenu.AddItem(new GUIContent("Add Node/Delay"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            controller.nodes.Add(new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnClickInput, OnClickOutput, OnRemoveNode, Node.NodeType.Delay));
        });
        genericMenu.AddItem(new GUIContent("Add Node/Parameter"), false, () =>
        {
            if(controller.nodes == null) controller.nodes = new List<Node>();
            controller.nodes.Add(new Node(mousePosition, 200, 100, style, selectedStyle, inputStyle, outputStyle, OnClickInput, OnClickOutput, OnRemoveNode, Node.NodeType.Parameter, false));
            controller.nodes[controller.nodes.Count - 1].parameter = new AIController.Parameter();
            if(controller.parameters == null) controller.parameters = new List<AIController.Parameter>();
            controller.parameters.Add(controller.nodes[controller.nodes.Count - 1].parameter);
        });
        genericMenu.ShowAsContext();
    }

    private void OnClickInput(LinkPoint linkPoint)
    {
        selectedInput = linkPoint;

        if(selectedOutput != null && selectedInput != selectedOutput && selectedInput.dataType == selectedOutput.dataType)
        {
            if(controller.links == null) controller.links = new List<Link>();
            MakeLink();
            selectedOutput = null;
            selectedInput = null;
        }
        
    }

    private void OnClickOutput(LinkPoint linkPoint)
    {
        selectedOutput = linkPoint;

        if(selectedInput != null && selectedInput != selectedOutput && selectedInput.dataType == selectedOutput.dataType)
        {
            if(controller.links == null) controller.links = new List<Link>();
            MakeLink();
            selectedInput = null;
            selectedOutput = null;
        }
        
    }

    private void MakeLink()
    {
        if(selectedOutput != null)
        {
            if(controller.links == null) controller.links = new List<Link>();
            controller.links.Add(new Link(selectedOutput, selectedInput, RemoveLink));
            selectedOutput = null;
        }
    }

    private void RemoveLink(Link link)
    {
        if(controller.links.Contains(link))
        {
            controller.links.Remove(link);
        }
    }

    private void OnRemoveNode(Node node)
    {
        if(controller.links != null)
        {
            List<Link> linksToRemove = new List<Link>();
            for(int i = controller.links.Count - 1; i >= 0; i--)
            {
                if(controller.links[i].input == node.seqInput || controller.links[i].output == node.seqOutput)
                {
                    linksToRemove.Add(controller.links[i]);
                }
                
            }
            foreach(Link link in linksToRemove)
            {
                controller.links.Remove(link);
            }
            linksToRemove = null;
        }
        if(controller.nodes.Contains(node))
        {
            if(node.type == Node.NodeType.Parameter)
            {
                controller.parameters.Remove(node.parameter);
            }
            controller.nodes.Remove(node);
        }
    }
    


    
}

