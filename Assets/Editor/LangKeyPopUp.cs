using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LangKeyPopup : PopupWindowContent
{
    private string m_key;
    private string m_translation;

    /// <summary>
    /// 
    /// </summary>
    public static event System.Action<string, string> onAddTranslation;

    public override Vector2 GetWindowSize()
    {
        return new Vector2(500, 180);
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Label("New translation", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Key:");
        m_key = EditorGUILayout.TextField(m_key);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Translation:");
        m_translation = EditorGUILayout.TextArea(m_translation, GUILayout.Height(40));

        if (GUI.Button(new Rect(rect.width - 105, rect.height - EditorGUIUtility.singleLineHeight - 5, 100, EditorGUIUtility.singleLineHeight),
            "Add translation")) {
            if (string.IsNullOrEmpty(m_key) || string.IsNullOrEmpty(m_translation))
            {
                EditorUtility.DisplayDialog("Warning!", "'Key' and 'Translation' fields can not be empty", "Ok");
            }
            else
            {
                onAddTranslation(m_key, m_translation);
                editorWindow.Close();
            }
        }
    }
}