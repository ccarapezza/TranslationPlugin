using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LanguageManager : EditorWindow
{
    public SystemLanguage defaultLanguage = SystemLanguage.English;
    public SystemLanguage currentLanguage;
    public string vv;
    public int index;

    [MenuItem("Plugin/Translation")]
    public static void ShowWindows()
    {
        ((LanguageManager)GetWindow(typeof(LanguageManager))).Show();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Languages", EditorStyles.boldLabel);
        if (Application.systemLanguage != SystemLanguage.Unknown)
            currentLanguage = Application.systemLanguage;
        else
            currentLanguage = defaultLanguage;
        //EditorGUILayout.EnumPopup("Current Language", currentLanguage);

        string[] langs = new string[LanguageCore.Instance.resources.Length];
        for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
        {
            langs[i] = LanguageCore.Instance.resources[i].language.ToString();
        }

        index = EditorGUILayout.Popup(index, langs);
        EditorGUILayout.Space();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if(GUILayout.Button("⇓ I M P O R T ⇓", GUILayout.Width(105), GUILayout.Height(35)))
                    {} // Import funct;
        EditorGUILayout.Space();
        if(GUILayout.Button("⇑ E X P O R T ⇑", GUILayout.Width(105), GUILayout.Height(35)))
                    {} // Export funct;
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();  
        GUILayout.EndVertical();      
    }

    //private string FormatLanguage
}