using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//namespace NodeAI;

    public enum LinkType
    {
        Input,
        Output
    }

    public enum LinkDataType
    {
        Float = 0,
        Int = 1,
        Bool = 2,
        Sequence = 3
    }

    public class LinkPoint 
    {
        public Rect rect;

        public LinkType type;
        public LinkDataType dataType;

        public Node node;

        public GUIStyle style;

        public List<Link> links = new List<Link>();

        public Action<LinkPoint> OnClick;

        public LinkPoint(Node node, LinkType type, LinkDataType dataType, GUIStyle style, Action<LinkPoint> OnClick)
        {
            this.node = node;
            this.type = type;
            this.dataType = dataType;
            this.style = style;
            this.OnClick = OnClick;
            rect = new Rect(0, 0, 10, 20);
        }

        public void Draw(int line)
        {
            rect.y = node.rect.y + 5 + ((EditorGUIUtility.singleLineHeight + 5) * line);

            if(type == LinkType.Input)
            {
                rect.x = node.rect.x - rect.width + 8;
            }
            else if(type == LinkType.Output)
            {
                rect.x = node.rect.x + node.rect.width - 8;
            }

            if(GUI.Button(rect, "", style))
            {
                if(OnClick != null)
                {
                    OnClick(this);
                }
            }
        }
        
    }
