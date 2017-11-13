using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEditor;

[ExecuteInEditMode]
public class LanguageManager : EditorWindow
{
    public IFormattable defaultLanguage = SystemLanguage.English;
    public IFormattable currentLanguage;
    public int index;

    private List<string[]> rowData = new List<string[]>();
    private string[][] output;
    private string title;
    private int length;
    static private string lang;
    private int longt;

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
        string[] langs = new string[LanguageCore.Instance.resources.Length];
        for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
            langs[i] = LanguageCore.Instance.resources[i].language.ToString();
        int ant = index;
        index = EditorGUILayout.Popup(index, langs);
        LanguageCore.Instance.LoadLanguage(LanguageCore.Instance.resources[index].language);
 
        if(ant != index)
            AssetDatabase.Refresh();

        EditorGUILayout.Space();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();

        if(GUILayout.Button("▼ I M P O R T ▼", GUILayout.Width(105), GUILayout.Height(35)))
                    {} // Import funct;
        EditorGUILayout.Space();
        if(GUILayout.Button("▲  E X P O R T ▲ ", GUILayout.Width(105), GUILayout.Height(35)))
            Save(LanguageCore.Instance.resources[index].language);

        if(GUILayout.Button("S A V E   A L L ", GUILayout.Width(105), GUILayout.Height(35)))
            SaveAll();

        EditorGUILayout.Space();
        GUILayout.EndHorizontal();  
        GUILayout.EndVertical();      
    }

    private void SaveAll()
    {
        longt = LanguageCore.Instance.resources.Length;
        for(int i = 0; i < longt; i++)
        {
            SystemLanguage tempLang = LanguageCore.Instance.resources[i].language;
            LanguageCore.Instance.LoadLanguage(tempLang);
            SearchLang(tempLang.ToString());
            rowData.Clear();
        }
    }

    private void Save(SystemLanguage lang)
    {
        SearchLang(lang.ToString());
        rowData.Clear();
    }

    private void SearchLang(string tempLang)
    {
        List<string> key = new List<string>(LanguageCore.Instance.resources[1].source.Keys);   
        string[] rowDataTemp = new string[2];
        rowDataTemp[0] = "KEY;";
        rowDataTemp[1] = "TEXT;";
        rowData.Add(rowDataTemp);

        for (int j = 0; j < longt; j++)  
        {
            lang = LanguageCore.Instance.resources[j].language.ToString();

            if(lang.Contains(tempLang))
                for (int i = 0; i < key.Count; i++)
                {
                    string var = LanguageCore.Instance.GetText(key[i]);
                    rowDataTemp = new string[2];
                    rowDataTemp[0] = key[i] + ";";
                    rowDataTemp[1] = var + ";";
                    rowData.Add(rowDataTemp);
                    StreamValues();  
                }
        }
    }

    private void StreamValues()
    {
        title = "LANGUAGE" + ";" + lang.ToUpper() + ";";
        output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
            output[i] = rowData[i];

        length = output.GetLength(0);
        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join("", output[index]));

        string filePath = getPath(); 
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(title);
        outStream.WriteLine(sb);
        outStream.Close();
        AssetDatabase.Refresh();
    }

    public string getPath()
    {
        return Application.dataPath +"/StreamingAssets/" + lang + ".csv";
    }
}