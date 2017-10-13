using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;


[CustomEditor(typeof(TranslationPlugin.UI.Text))]
public class TextEditor : GraphicEditor
{
    private SerializedProperty m_FontData;
    private int m_selectedKey;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_FontData = serializedObject.FindProperty("m_FontData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.font = EditorStyles.boldFont;
        EditorGUILayout.LabelField(new GUIContent("Translation"), titleStyle);

        if (LanguageManager.instance == null) return;

        List<string> key = new List<string>(LanguageManager.instance.resources[0].source.Keys);

        m_selectedKey = key.IndexOf(serializedObject.FindProperty("key").stringValue);

        EditorGUI.indentLevel++;
        m_selectedKey = EditorGUILayout.Popup("Translation key:", m_selectedKey, key.ToArray());
        if(m_selectedKey!=-1)
            serializedObject.FindProperty("key").stringValue = key[m_selectedKey];
        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(m_FontData);

        base.AppearanceControlsGUI();
        base.RaycastControlsGUI();

        serializedObject.ApplyModifiedProperties();
    }
}
