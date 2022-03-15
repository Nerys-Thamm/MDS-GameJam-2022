using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

//namespace NodeAI;
    [System.Serializable]
    public class LinkPointEvent : UnityEvent<LinkPoint> { };
    [System.Serializable]
    public enum LinkType
    {
        Input,
        Output
    }
    [System.Serializable]
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
        public int fieldIndex = 0;
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
        [SerializeField]
        public List<string> linkIDs;
        [SerializeField]
        public LinkPointEvent OnClickEvent;

        public LinkPoint(string NodeID, LinkType type, LinkDataType dataType, GUIStyle style, LinkPointEvent OnClickEvent)
        {
            this.NodeID = NodeID;
            this.type = type;
            this.dataType = dataType;
            this.style = style;
            this.OnClickEvent = OnClickEvent;
            rect = new Rect(0, 0, 10, 20);
        }

        public void ReconnectEvents(LinkPointEvent OnClickEvent)
        {
            this.OnClickEvent = OnClickEvent;
        }

        public void ReconnectLinks(AIController controller)
        {
            foreach (string linkID in linkIDs)
            {
                Link link = controller.GetLinkFromID(linkID);
                if (link != null)
                {
                    if(type == LinkType.Output)
                    {
                        link.input = this;
                    }
                    else
                    {
                        link.output = this;
                    }
                }
            }
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

            if(GUI.Button(this.rect, "", EditorStyles.miniButton))
            {
                if(OnClickEvent != null)
                {
                    //OnClick(this);
                    OnClickEvent.Invoke(this);
                }
            }
        }

        
        
    }
