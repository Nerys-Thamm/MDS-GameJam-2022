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
    [System.Serializable]
    public class LinkPoint 
    {
        [SerializeField]
        public Rect rect;
        [SerializeField]
        public LinkType type;
        [SerializeField]
        public LinkDataType dataType;
        [SerializeField]
        public string NodeID;
        [NonSerialized]
        public Node node;
        [SerializeField]
        public GUIStyle style;
        [NonSerialized]
        public List<Link> links = new List<Link>();
        [NonSerialized]
        public Action<LinkPoint> OnClick;

        public LinkPoint(string NodeID, LinkType type, LinkDataType dataType, GUIStyle style, Action<LinkPoint> OnClick)
        {
            this.NodeID = NodeID;
            this.type = type;
            this.dataType = dataType;
            this.style = style;
            this.OnClick = OnClick;
            rect = new Rect(0, 0, 10, 20);
        }

        public void Draw(int line, Rect rect)
        {
            this.rect = new Rect(0, 0, 10, 20);
            this.rect.y = rect.y + 5 + ((EditorGUIUtility.singleLineHeight + 5) * line);

            if(type == LinkType.Input)
            {
                this.rect.x = rect.x - this.rect.width + 8;
            }
            else if(type == LinkType.Output)
            {
                this.rect.x = rect.x + rect.width - 8;
            }

            if(GUI.Button(this.rect, "", style))
            {
                if(OnClick != null)
                {
                    OnClick(this);
                }
            }
        }

        
        
    }
