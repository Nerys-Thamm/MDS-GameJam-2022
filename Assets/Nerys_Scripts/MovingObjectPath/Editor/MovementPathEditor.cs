using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(MovementPath))]
public class MovementPathEditor : Editor
{

    MovementPath path;
    SerializedProperty nodes;
    public ReorderableList list;
    public bool editPath = false;
    public bool placingNodes = false;


    private void OnEnable()
    {
        path = (MovementPath)target;
        if (path.m_Path == null)
        {
            path.m_Path = new List<MovementPath.PathNode>();
        }
        nodes = serializedObject.FindProperty("m_Path");
        list = new ReorderableList(serializedObject, nodes, true, true, true, true);
        list.drawElementCallback += DrawElement;
        list.drawHeaderCallback = DrawHeader;
        list.index = path.m_Path.Count-1;
        list.onAddCallback = param =>
        {
            if(param.index < 0 || param.index >= path.m_Path.Count)
            {
                param.index = path.m_Path.Count - 1;
            }
            if (path.m_Path.Count == 0 || param.index == 0)
            {
                path.m_Path.Add(new MovementPath.PathNode(path.gameObject.transform.position));
            }
            else
            {
                path.m_Path.Add(new MovementPath.PathNode(path.m_Path[param.index].Position));
            }
        };
        list.onRemoveCallback = param =>
        {
            if(path.m_Path.Count != 0)
            {
                path.m_Path.RemoveAt(param.index);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Path");
        
        
        serializedObject.ApplyModifiedProperties();
    }


    void DrawHeader(Rect rect)
    {
        string name = "Nodes";
        EditorGUI.LabelField(rect, name);
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(
        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("Position"),
        GUIContent.none
        );

    }

    
    private void OnSceneGUI()
    {
        //Draw the path and movement handles
        Handles.color = path.m_PathColor;
        if (path && path.m_Path.Count != 0)
        {
            Vector3 offset = Vector3.zero;
            if(!editPath)
            {
                Vector3 avgPos = Vector3.zero;
                for (int i = 0; i < path.m_Path.Count; i++) avgPos += path.m_Path[i].Position;
                avgPos /= path.m_Path.Count;
                offset = Handles.PositionHandle(avgPos, Quaternion.identity) - avgPos;
            }
            for (int i = 0; i < path.m_Path.Count; i++)
            {
                RaycastHit hit;
                if(Physics.Raycast(path.m_Path[i].Position, Vector3.down, out hit))
                    path.m_Path[i].Position = (hit.point + Vector3.up);
                else if(Physics.Raycast(path.m_Path[i].Position, Vector3.up, out hit))
                    path.m_Path[i].Position = (hit.point + Vector3.up);
                if(editPath && !placingNodes)
                    path.m_Path[i].Position = Handles.PositionHandle(path.m_Path[i].Position, Quaternion.identity);
                else
                {
                    path.m_Path[i].Position += offset;
                }
                Handles.DrawSolidDisc(path.m_Path[i].Position - Vector3.up, hit.normal, 0.5f);
                if (i < path.m_Path.Count - 1)
                {
                    Vector3 pathvector = path.m_Path[i + 1].Position - path.m_Path[i].Position;
                    Handles.DrawLine(path.m_Path[i].Position  - Vector3.up , path.m_Path[i + 1].Position  - Vector3.up);
                    if (pathvector.magnitude > 0)
                    {
                        Handles.ArrowHandleCap(i, path.m_Path[i].Position - Vector3.up, Quaternion.LookRotation(pathvector, Vector3.up), pathvector.magnitude / 2, EventType.Repaint);
                        
                    }
                }
                else if(i == path.m_Path.Count - 1 && path.m_Mode == MovementPath.PathType.CIRCUIT && !placingNodes)
                {
                    Vector3 pathvector = path.m_Path[0].Position - path.m_Path[i].Position;
                    Handles.DrawLine(path.m_Path[i].Position - Vector3.up, path.m_Path[0].Position  - Vector3.up);
                    if (pathvector.magnitude > 0)
                    {
                        Handles.ArrowHandleCap(i, path.m_Path[i].Position - Vector3.up, Quaternion.LookRotation(pathvector, Vector3.up), pathvector.magnitude / 2, EventType.Repaint);
                    }
                }

            }
        }

        //Placing nodes

        if(placingNodes)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        
                            path.m_Path.Add(new MovementPath.PathNode(hit.point + Vector3.up));
                        
                    }
                    else if(Event.current.type == EventType.Layout)
                    {
                        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    }
                    Handles.color = Color.green;
                    if(path.m_Path.Count != 0)
                    {
                        Handles.DrawLine(path.m_Path[path.m_Path.Count - 1].Position - Vector3.up, hit.point);
                        if(path.m_Mode == MovementPath.PathType.CIRCUIT)
                        {
                            Handles.DrawLine(path.m_Path[0].Position - Vector3.up, hit.point);
                        }
                    }
                    Handles.DrawSolidDisc(hit.point, hit.normal, 0.5f);

                }
        }

        //GUI

        Handles.BeginGUI();

        if (editPath)
        {
            if (GUI.Button(new Rect(10, 10, 100, 30), "Stop Editing"))
            {
                editPath = false;
            }
            if (placingNodes)
            {
                if (GUI.Button(new Rect(10, 50, 150, 30), "Stop Placing Nodes"))
                {
                    placingNodes = false;
                }
            }
            else
            {
                if (GUI.Button(new Rect(10, 50, 150, 30), "Start Placing Nodes"))
                {
                    placingNodes = true;
                }
            }
        }
        else
        {
            if (GUI.Button(new Rect(10, 10, 100, 30), "Edit Path"))
            {
                editPath = true;
            }
        }

        Handles.EndGUI();

        
        

    }
}
