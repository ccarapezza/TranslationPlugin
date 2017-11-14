using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System;

[CustomEditor(typeof(LanguageResource))]
public class LanguageResourceEditor : Editor
{
    private SerializedProperty m_languageResources {
        get {
            return serializedObject.FindProperty("elements");
        }
    }
    private SerializedProperty m_language {
        get
        {
            return serializedObject.FindProperty("language");
        }
    }

    private LanguageResource m_masterLanguageResources;
    private int m_editKey;
    private string m_newKey = "";
    private string m_searchString = "";
    private List<string> m_newKeys = new List<string>();
    private List<string> m_missingKeys = new List<string>();

    private void OnEnable()
    {
        m_editKey = -1;

        LangKeyPopup.onAddTranslation += (key, translation) =>
        {
            AddNewKeyHandler(key, translation);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CalculateResourcesInfo();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.PropertyField(m_language, new GUIContent("Language:"));
        SearchToolbar();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        TranslationList();

        serializedObject.ApplyModifiedProperties();
    }


    private void CalculateResourcesInfo()
    {
        var titleLabelStyle = new GUIStyle(GUI.skin.label);
        titleLabelStyle.fontSize = 18;
        titleLabelStyle.fixedHeight = 25;
        EditorGUILayout.LabelField("Resource Syncs", titleLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("master"), new GUIContent("Master Resource:"));

        m_masterLanguageResources = serializedObject.FindProperty("master").objectReferenceValue as LanguageResource;
        /////////////// 

        if (m_masterLanguageResources != null)
        {
            m_newKeys.Clear();
            m_missingKeys.Clear();

            var keys = new List<string>(m_masterLanguageResources.source.Keys);

            for (int i = 0; i < m_languageResources.arraySize; i++)
            {
                var item = m_languageResources.GetArrayElementAtIndex(i);
                var key = item.FindPropertyRelative("key").stringValue;
                if (!m_masterLanguageResources.source.ContainsKey(key))
                {
                    m_newKeys.Add(key);
                }
            }

            foreach (var key in keys)
            {
                bool finded = false;
                for (int i = 0; i < m_languageResources.arraySize; i++)
                {
                    var item = m_languageResources.GetArrayElementAtIndex(i);
                    var keyEl = item.FindPropertyRelative("key").stringValue;
                    if (keyEl.Equals(key))
                    {
                        finded = true;
                        break;
                    }
                }
                if (!finded)
                    m_missingKeys.Add(key);
            }

            var positiveLabelStyle = new GUIStyle(GUI.skin.label);
            positiveLabelStyle.normal.textColor = new Vector4(0.1f, 0.6f, 0.1f, 1);
            positiveLabelStyle.font = EditorStyles.boldFont;

            var negativeLabelStyle = new GUIStyle(GUI.skin.label);
            negativeLabelStyle.normal.textColor = new Vector4(0.6f, 0.1f, 0.1f, 1);
            negativeLabelStyle.font = EditorStyles.boldFont;


            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(string.Format("-Master resource total keys: {0}", m_masterLanguageResources.source.Count));
            EditorGUILayout.LabelField(string.Format("-Current resource total keys: {0}", m_languageResources.arraySize));
            if (m_missingKeys.Count == 0 && m_newKeys.Count == 0)
            {
                EditorGUILayout.LabelField(string.Format("Language resource is sincronized", m_missingKeys.Count.ToString()), positiveLabelStyle);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(string.Format("Language resource is out of sync", m_missingKeys.Count.ToString()), negativeLabelStyle);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(string.Format("-Missing keys: {0}", m_missingKeys.Count.ToString()));
                EditorGUILayout.LabelField(string.Format("-New keys: {0}", m_newKeys.Count.ToString()));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;


            GUILayout.BeginHorizontal();
            {
                var actionLabelStyle = new GUIStyle(GUI.skin.button);
                actionLabelStyle.fontSize = 16;
                EditorGUI.BeginDisabledGroup(m_newKeys.Count == 0 || m_missingKeys.Count == 0);
                if(GUILayout.Button("Merge ⇄", actionLabelStyle))
                    Merge();
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(m_newKeys.Count == 0);
                if (GUILayout.Button("Merge To Master →", actionLabelStyle))
                    MergeToMaster();
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(m_missingKeys.Count == 0);
                if (GUILayout.Button("Merge From Master ←", actionLabelStyle))
                    MergeFromMaster();
                EditorGUI.EndDisabledGroup();

            }
            GUILayout.EndHorizontal();
        }
    }

    private void SearchToolbar()
    {
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        {
            m_searchString = GUILayout.TextField(m_searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));

            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty")))
            {
                GUI.FocusControl(null);
            }
        }
        GUILayout.EndHorizontal();
    }

    private void TranslationList()
    {
        List<int> deleteIndexes = new List<int>();
        int index;

        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        GUILayout.BeginVertical();
        {
            for (index = 0; index < m_languageResources.arraySize; index++)
            {
                var serializedProperty = m_languageResources.GetArrayElementAtIndex(index);
                
                if (!serializedProperty.FindPropertyRelative("key").stringValue.Contains(m_searchString))
                    continue;

                GUILayout.BeginHorizontal();
                {
                    if (m_editKey == index)
                    {
                        m_newKey = EditorGUILayout.TextField(m_newKey);
                        var saveButtonStyle = new GUIStyle(GUI.skin.button);
                        saveButtonStyle.normal.textColor = Color.green;

                        if (GUILayout.Button("✔", saveButtonStyle, GUILayout.Width(20)))
                        {
                            if (!ExistKey(m_newKey))
                                serializedProperty.FindPropertyRelative("key").stringValue = m_newKey;
                            m_editKey = -1;
                            m_newKey = "";
                            
                        }
                        var buttonStyle = new GUIStyle(GUI.skin.button);
                        buttonStyle.font = EditorStyles.boldFont;
                        if (GUILayout.Button("←", buttonStyle, GUILayout.Width(25)))
                        {
                            m_editKey = -1;
                        }
                    }
                    else
                    {
                        var keyLabelStyle = new GUIStyle(GUI.skin.label);
                        if (m_newKeys.Contains(serializedProperty.FindPropertyRelative("key").stringValue))
                        {
                            keyLabelStyle.normal.textColor = new Vector4(0.1f, 0.6f, 0.1f, 1);
                            keyLabelStyle.font = EditorStyles.boldFont;
                        }

                        EditorGUILayout.LabelField(serializedProperty.FindPropertyRelative("key").stringValue, keyLabelStyle);

                        if (GUILayout.Button("Edit key", GUILayout.Width(80)))
                        {
                            m_newKey = serializedProperty.FindPropertyRelative("key").stringValue;
                            m_editKey = index;
                        }

                        var buttonStyle = new GUIStyle(GUI.skin.button);
                        buttonStyle.normal.textColor = Color.red;
                        if (GUILayout.Button("✖", buttonStyle, GUILayout.Width(20)))
                        {
                            if (EditorUtility.DisplayDialog("Confirm delete", "Are you sure you want to permanently delete this key and translation?", "Yes", "No"))
                            {
                                deleteIndexes.Add(index);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("text"), new GUIContent(""));
                EditorGUI.indentLevel--;
                //Horizontal line
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

            }
        }

        EditorGUILayout.Space();
        Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent("Button"), GUI.skin.button);
        if (GUI.Button(buttonRect, "Add translation key..."))
        {
            PopupWindow.Show(buttonRect, new LangKeyPopup());
        }
        GUILayout.EndVertical();

        for (index = 0; index < deleteIndexes.Count; index++)
        {
            m_languageResources.DeleteArrayElementAtIndex(deleteIndexes[index]);
            deleteIndexes.Clear();
        }
    }

    private void Merge()
    {
        if (EditorUtility.DisplayDialog("Confirm merge", string.Format("Are you sure you want to add {0} new keys to master resource and add {1} missings keys to current resource?", m_newKeys.Count, m_missingKeys.Count), "Yes", "No"))
        {
            m_masterLanguageResources.AddNewKeys(m_newKeys);
            foreach (var key in m_missingKeys)
            {
                AddNewKeyHandler(key, key);
            }
        }
    }

    private void MergeToMaster()
    {
        if (EditorUtility.DisplayDialog("Confirm merge to master", string.Format("Are you sure you want to add {0} new keys to master resource?", m_newKeys.Count), "Yes", "No"))
        {
            m_masterLanguageResources.AddNewKeys(m_newKeys);
        }
    }

    private void MergeFromMaster()
    {
        if (EditorUtility.DisplayDialog("Confirm merge from master", string.Format("Are you sure you want to add {0} missings keys to current resource?", m_missingKeys.Count), "Yes", "No"))
        {
            foreach (var key in m_missingKeys)
            {
                AddNewKeyHandler(key,key);
            }
        }
    }

    private void AddNewKeyHandler(string key, string translation)
    {

        if (!ExistKey(key))
        {
            var index = m_languageResources.arraySize;
            m_languageResources.arraySize++;
            var element = m_languageResources.GetArrayElementAtIndex(index);

            element.FindPropertyRelative("key").stringValue = key;
            element.FindPropertyRelative("text").stringValue = translation;

            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            EditorUtility.DisplayDialog("Error!", "An element with the same 'Key' already exist", "Ok");
        }

    }

    private bool ExistKeyWithoutIndex(string key, int index)
    {
        int i;
        for (i = 0; i < m_languageResources.arraySize; i++)
        {
            var element = m_languageResources.GetArrayElementAtIndex(i);
            if (element.FindPropertyRelative("key").stringValue == key && index != i)
            {
                return true;
            }
        }
        return false;
    }

    private bool ExistKey(string key)
    {
        return ExistKeyWithoutIndex(key, -1);
    }
}