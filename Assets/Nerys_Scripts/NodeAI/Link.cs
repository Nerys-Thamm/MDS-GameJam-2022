using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

//namespace NodeAI;

    public class Link 
    {
        public LinkPoint input;
        public LinkPoint output;
        public Action<Link> OnClick;

        public Link(LinkPoint input, LinkPoint output, Action<Link> OnClick)
        {
            this.input = input;
            this.output = output;
            this.OnClick = OnClick;
        }

        public void Draw()
        {
            Handles.DrawBezier(
                input.rect.center,
                output.rect.center,
                input.rect.center - Vector2.left * 50f,
                output.rect.center + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            if(Handles.Button((input.rect.center + output.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                if(OnClick != null)
                {
                    OnClick(this);
                }
            }
        }
    }

