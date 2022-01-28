using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(SFX_Effect))]
public class SFX_Effect_Editor : Editor
{

    SFX_Effect effect;
    SerializedProperty effectsList;
    public ReorderableList list;


    private void OnEnable()
    {
        effect = (SFX_Effect)target;
        effectsList = serializedObject.FindProperty("m_effectsList");
        list = new ReorderableList(serializedObject, effectsList, true, true, true, true);
        list.elementHeight = EditorGUIUtility.singleLineHeight * 4;
        list.drawElementCallback += DrawElement;
        list.drawHeaderCallback = DrawHeader;
        list.index = effect.m_effectsList.Count - 1;
        list.onAddCallback = param =>
        {
            if (param.index < 0 || param.index >= effect.m_effectsList.Count)
            {
                param.index = effect.m_effectsList.Count - 1;
            }
            if (effect.m_effectsList.Count == 0 || param.index == 0)
            {
                effect.m_effectsList.Add(new SFX_Effect.Effect());
            }
            else
            {
                effect.m_effectsList.Add(new SFX_Effect.Effect());
            }
        };
        list.onRemoveCallback = param =>
        {
            if (effect.m_effectsList.Count != 0)
            {
                effect.m_effectsList.RemoveAt(param.index);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("▶    Play Effect")) effect.Play();
        if (GUILayout.Button("❚❚ Pause Effect")) effect.Pause();
        if (GUILayout.Button("◼    Stop Effect")) effect.Stop();

        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }


    void DrawHeader(Rect rect)
    {
        string name = "Custom Effect Editor";
        EditorGUI.LabelField(rect, name);
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Effect Call Type:");
        EditorGUI.PropertyField(
        new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 5, 150, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("m_type"),
        GUIContent.none
        );
        switch (effect.m_effectsList[index].m_type)
        {
            case SFX_Effect.SFXtype.DESTROY_OBJ:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5) * 2, 150, EditorGUIUtility.singleLineHeight), "[ Destroy the Object ]");
                break;
            case SFX_Effect.SFXtype.PLAY_PARTICLE:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5) * 2, 150, EditorGUIUtility.singleLineHeight), "[ Play a Particle Effect ]");
                EditorGUI.PropertyField(
            new Rect(rect.x + 150, rect.y, rect.width - 150, EditorGUIUtility.singleLineHeight * 4),
            element.FindPropertyRelative("m_particle"),
            GUIContent.none
            );
                break;
            case SFX_Effect.SFXtype.PLAY_SOUND:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5) * 2, 150, EditorGUIUtility.singleLineHeight), "[ Play a Sound ]");
                EditorGUI.PropertyField(
            new Rect(rect.x + 150, rect.y, (rect.width - 150) / 2, EditorGUIUtility.singleLineHeight * 4),
            element.FindPropertyRelative("m_clip"),
            GUIContent.none
            );
                EditorGUI.PropertyField(
            new Rect(rect.x + 150 + ((rect.width - 150) / 2), rect.y, (rect.width - 150) / 2, EditorGUIUtility.singleLineHeight * 4),
            element.FindPropertyRelative("m_source"),
            GUIContent.none
            );
                break;
            case SFX_Effect.SFXtype.SET_SCREEN_SHAKE:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5) * 2, 170, EditorGUIUtility.singleLineHeight), "[ Set Screen Shake Amount ]");
                EditorGUI.PropertyField(
            new Rect(rect.x + 170, rect.y + EditorGUIUtility.singleLineHeight + 5, (rect.width - 170), EditorGUIUtility.singleLineHeight * 4),
            element.FindPropertyRelative("m_screenShakeAmplitude"),
            GUIContent.none
            );
                break;
            case SFX_Effect.SFXtype.SET_GAMEPAD_RUMBLE:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5) * 2, 170, EditorGUIUtility.singleLineHeight), "[ Set Gamepad Rumble Speed ]");
                EditorGUI.PropertyField(
            new Rect(rect.x + 170, rect.y + EditorGUIUtility.singleLineHeight + 5, (rect.width - 170), EditorGUIUtility.singleLineHeight * 4),
            element.FindPropertyRelative("m_controllerRumbleSpeed"),
            GUIContent.none
            );
                break;
            case SFX_Effect.SFXtype.STOP_PARTICLE:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5) * 2, 150, EditorGUIUtility.singleLineHeight), "[ Stop a Particle Effect ]");
                EditorGUI.PropertyField(
            new Rect(rect.x + 150, rect.y, rect.width - 150, EditorGUIUtility.singleLineHeight * 4),
            element.FindPropertyRelative("m_particle"),
            GUIContent.none
            );
                break;
            case SFX_Effect.SFXtype.WAIT:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5) * 2, 160, EditorGUIUtility.singleLineHeight), "[ Delay the next Effect Call ]");
                EditorGUI.LabelField(new Rect(rect.x + 150, rect.y + (EditorGUIUtility.singleLineHeight + 5), 50, EditorGUIUtility.singleLineHeight), "   FOR ");
                EditorGUI.LabelField(new Rect(rect.x + 200 + rect.width - 320, rect.y + (EditorGUIUtility.singleLineHeight + 5), 70, EditorGUIUtility.singleLineHeight), " SECONDS");
                EditorGUI.PropertyField(
            new Rect(rect.x + 200, rect.y + EditorGUIUtility.singleLineHeight + 5, rect.width - 320, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("m_waitDuration"),
            GUIContent.none
            );
                break;
            case SFX_Effect.SFXtype.SPAWN:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + 5) * 2, 150, EditorGUIUtility.singleLineHeight), "[ Spawn a Prefab ]");
                EditorGUI.PropertyField(
            new Rect(rect.x + 150, rect.y, rect.width - 150, EditorGUIUtility.singleLineHeight * 4),
            element.FindPropertyRelative("m_prefab"),
            GUIContent.none
            );
                break;
            default:
                break;
        }

        // EditorGUI.LabelField(new Rect(rect.x + 160, rect.y, 100, EditorGUIUtility.singleLineHeight), "Speed Curve");

        // EditorGUI.PropertyField(
        // new Rect(rect.x + 250, rect.y, 150, EditorGUIUtility.singleLineHeight),
        // element.FindPropertyRelative("SpeedCurve"),
        // GUIContent.none
        // );
        // EditorGUI.PropertyField(
        // new Rect(rect.x, rect.y, 150, EditorGUIUtility.singleLineHeight),
        // element.FindPropertyRelative("Position"),
        // GUIContent.none
        // );

    }


}