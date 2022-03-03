using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace NodeAI;

    public class Node 
    {
        public Rect rect;
        public string title;
        public bool isDragging;
        public bool selected;
        public GUIStyle style;
        public GUIStyle defaultStyle;
        public GUIStyle selectedStyle;

        public LinkPoint input;
        public LinkPoint output;

        public Action<Node> OnRemove;

        public enum NodeType
        {
            State,
            Condition,
            Action,
            Sequence,
            Delay,
            Entry
        }

        public NodeType type;

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
            Action<Node> OnClickRemove
            )
        {
            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;
            input = new LinkPoint(this, LinkType.Input, inputStyle, OnClickInput);
            output = new LinkPoint(this, LinkType.Output, outputStyle, OnClickOutput);

            defaultStyle = nodeStyle;
            this.selectedStyle = selectedStyle;

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
            input = null;
            output = new LinkPoint(this, LinkType.Output, outputStyle, OnClickOutput);

            defaultStyle = nodeStyle;
            this.selectedStyle = selectedStyle;

            OnRemove = null;
        }

        public void Move(Vector2 delta)
        {
            rect.position += delta;
        }

        public void Draw()
        {
            if(input != null)
                input.Draw();
            if(output != null)
                output.Draw();
            GUI.Box(rect, title, style);
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

