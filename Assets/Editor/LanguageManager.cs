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
        //EditorGUILayout.EnumPopup("Current Language", defaultLanguage);

    }
}