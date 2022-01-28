using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(DialogueScript))]
public class DialogueEditor : Editor
{

    DialogueScript m_dialogue;
    SerializedProperty phraseList;
    public ReorderableList list;


    private void OnEnable()
    {
        m_dialogue = (DialogueScript)target;
        phraseList = serializedObject.FindProperty("m_Phrases");
        list = new ReorderableList(serializedObject, phraseList);
        list.elementHeight = EditorGUIUtility.singleLineHeight * 8;
        list.drawElementCallback += DrawElement;
        list.drawHeaderCallback = DrawHeader;
        if (m_dialogue.m_Phrases.Count != 0)
        {
        list.index = (m_dialogue.m_Phrases.Count == 0 ? 0 : m_dialogue.m_Phrases.Count - 1);
        }
        list.onAddCallback = param =>
        {
            if (param.index < 0 || param.index >= m_dialogue.m_Phrases.Count)
            {
                param.index = m_dialogue.m_Phrases.Count - 1;
            }
            if (m_dialogue.m_Phrases.Count == 0 || param.index == 0)
            {
                m_dialogue.m_Phrases.Add(new DialogueScript.Phrase());
            }
            else
            {
                m_dialogue.m_Phrases.Add(new DialogueScript.Phrase());
            }
        };
        list.onRemoveCallback = param =>
        {
            if (m_dialogue.m_Phrases.Count != 0)
            {
                m_dialogue.m_Phrases.RemoveAt(param.index);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        //serializedObject.Update();
        // if (GUILayout.Button("▶    Play Effect")) effect.Play();
        // if (GUILayout.Button("❚❚ Pause Effect")) effect.Pause();
        // if (GUILayout.Button("◼    Stop Effect")) effect.Stop();

        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }


    void DrawHeader(Rect rect)
    {
        string name = "Dialogue";
        EditorGUI.LabelField(rect, name);
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Phrase type:");
        EditorGUI.PropertyField(
        new Rect(rect.x + 80, rect.y, 80, EditorGUIUtility.singleLineHeight),
        element.FindPropertyRelative("m_type"),
        GUIContent.none
        );
        switch (m_dialogue.m_Phrases[index].m_type)
        {
            
            case DialogueScript.PhraseType.DEFAULT:
                EditorGUI.LabelField(
                    new Rect(rect.x + ((rect.width)-120), rect.y, 120, EditorGUIUtility.singleLineHeight),
                    "Text Speed: ");
                EditorGUI.PropertyField(
                new Rect(rect.x + ((rect.width)-40), rect.y, 40, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("m_speed"),
                GUIContent.none
                );
                EditorGUI.LabelField(
                    new Rect(rect.x + ((rect.width)-120), rect.y + EditorGUIUtility.singleLineHeight, 120, EditorGUIUtility.singleLineHeight),
                    "Blip Sound: ");
                EditorGUI.PropertyField(
                new Rect(rect.x + ((rect.width)-40), rect.y + EditorGUIUtility.singleLineHeight, 40, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("m_blip"),
                GUIContent.none
                );
                EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight * 6, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("m_characterSprite"),
                GUIContent.none
                );
                EditorGUI.PropertyField(
                new Rect(rect.x + EditorGUIUtility.singleLineHeight * 6, rect.y + EditorGUIUtility.singleLineHeight * 2, rect.width-(EditorGUIUtility.singleLineHeight * 6), EditorGUIUtility.singleLineHeight * 6),
                element.FindPropertyRelative("m_message"),
                GUIContent.none
                );
                if(m_dialogue.m_Phrases[index].m_characterSprite != null)
                {
                EditorGUI.DrawTextureTransparent(
                    new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight * 6, EditorGUIUtility.singleLineHeight * 6),
                    m_dialogue.m_Phrases[index].m_characterSprite.texture
                );
                }
                break;
            case DialogueScript.PhraseType.SFX:
                EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight * 6, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("m_characterSprite"),
                GUIContent.none
                );
                EditorGUI.PropertyField(
                new Rect(rect.x + EditorGUIUtility.singleLineHeight * 6, rect.y + EditorGUIUtility.singleLineHeight * 2, rect.width-(EditorGUIUtility.singleLineHeight * 6), EditorGUIUtility.singleLineHeight * 6),
                element.FindPropertyRelative("m_message"),
                GUIContent.none
                );
                if(m_dialogue.m_Phrases[index].m_characterSprite != null)
                {
                EditorGUI.DrawTextureTransparent(
                    new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight * 6, EditorGUIUtility.singleLineHeight * 6),
                    m_dialogue.m_Phrases[index].m_characterSprite.texture
                );
                }
                EditorGUI.PropertyField(
                new Rect(rect.x + ((rect.width)-120), rect.y, 120, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("m_speed"),
                GUIContent.none
                );
                EditorGUI.PropertyField(
                new Rect(rect.x + ((rect.width)-120), rect.y + EditorGUIUtility.singleLineHeight, 120, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("m_sfx"),
                GUIContent.none
                );
                break;
            
            default:
                break;
        }
    }
}
