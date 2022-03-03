using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeAI
{
    public enum LinkType
    {
        Input,
        Output
    }

    public class LinkPoint 
    {
        public Rect rect;

        public LinkType type;

        public Node node;

        public GUIStyle style;

        public Action<LinkPoint> OnClick;

        public LinkPoint(Node node, LinkType type, GUIStyle style, Action<LinkPoint> OnClick)
        {
            this.node = node;
            this.type = type;
            this.style = style;
            this.OnClick = OnClick;
            rect = new Rect(0, 0, 10, 20);
        }

        public void Draw()
        {
            rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

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
}