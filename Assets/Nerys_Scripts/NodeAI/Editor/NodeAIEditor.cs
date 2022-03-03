using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NodeAI
{
    public class NodeAIEditor : EditorWindow
    {
        private List<Node> nodes;
        private List<Link> links;

        private GUIStyle style;
        private GUIStyle inputStyle, outputStyle, selectedStyle;

        private LinkPoint selectedInput, selectedOutput;

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
        }

        private void OnGUI()
        {
            DrawNodes();
            DrawLinks();
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if(GUI.changed) Repaint();
        }

        private void DrawNodes()
        {
            if(nodes != null)
            {
                foreach(Node node in nodes)
                {
                    node.Draw();
                }
            }
        }

        private void DrawLinks()
        {
            if(links != null)
            {
                foreach(Link link in links)
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
                    break;
            }
        }

        private void ProcessNodeEvents(Event e)
        {
            if(nodes != null)
            {
                for(int i = nodes.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = nodes[i].ProcessEvents(e);

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
            genericMenu.AddItem(new GUIContent("Add node"), false, () =>
            {
                if(nodes == null) nodes = new List<Node>();
                nodes.Add(new Node(mousePosition, 200, 50, style, selectedStyle, inputStyle, outputStyle, OnClickInput, OnClickOutput, OnRemoveNode));
            });
            genericMenu.ShowAsContext();
        }

        private void OnClickInput(LinkPoint linkPoint)
        {
            selectedInput = linkPoint;

            if(selectedOutput != null && selectedInput != selectedOutput)
            {
                if(links == null) links = new List<Link>();
                MakeLink();
                selectedOutput = null;
                selectedInput = null;
            }
            
        }

        private void OnClickOutput(LinkPoint linkPoint)
        {
            selectedOutput = linkPoint;

            if(selectedInput != null && selectedInput != selectedOutput)
            {
                if(links == null) links = new List<Link>();
                MakeLink();
                selectedInput = null;
                selectedOutput = null;
            }
            
        }

        private void MakeLink()
        {
            if(selectedOutput != null)
            {
                if(links == null) links = new List<Link>();
                links.Add(new Link(selectedOutput, selectedInput, RemoveLink));
                selectedOutput = null;
            }
        }

        private void RemoveLink(Link link)
        {
            if(links.Contains(link))
            {
                links.Remove(link);
            }
        }

        private void OnRemoveNode(Node node)
        {
            if(links != null)
            {
                List<Link> linksToRemove = new List<Link>();
                for(int i = links.Count - 1; i >= 0; i--)
                {
                    if(links[i].input == node.input || links[i].output == node.output)
                    {
                        linksToRemove.Add(links[i]);
                    }
                    
                }
                foreach(Link link in linksToRemove)
                {
                    links.Remove(link);
                }
                linksToRemove = null;
            }
            if(nodes.Contains(node))
            {
                nodes.Remove(node);
            }
        }
        


        
    }
}
