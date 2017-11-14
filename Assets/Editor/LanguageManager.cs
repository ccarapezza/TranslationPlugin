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
    public LanguageResource[] resources;


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
        resources = LanguageCore.Instance.resources;
        int indexAnt;
        EditorGUILayout.LabelField("Languages", EditorStyles.boldLabel);
        
        if (Application.systemLanguage != SystemLanguage.Unknown)
            currentLanguage = Application.systemLanguage;
        else
            currentLanguage = defaultLanguage;
        indexAnt = index;
        string[] langs = new string[resources.Length];
        for (int i = 0; i < resources.Length; i++)
            langs[i] = resources[i].language.ToString();

        index = EditorGUILayout.Popup(index, langs);
        LanguageCore.Instance.LoadLanguage(resources[index].language);
        var tempLang = resources[index].language.ToString();
        if (index != indexAnt)
            SearchLang(tempLang);
        EditorGUILayout.Space();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("▼IMPORT▼", GUILayout.Width(105), GUILayout.Height(35)))
            //ImportLang();
        EditorGUILayout.Space();
        if(GUILayout.Button("▲EXPORT▲ ", GUILayout.Width(105), GUILayout.Height(35)))
            SearchLang(tempLang);
        EditorGUILayout.Space();
        if (GUILayout.Button("SAVE ALL", GUILayout.Width(105), GUILayout.Height(35)))
            SaveAll();

        EditorGUILayout.Space();
        GUILayout.EndHorizontal();  
        GUILayout.EndVertical();      
    }

    private void ImportLang() 
    {
        string path = "Assets/StreamingAssets/English.csv";
        StreamReader reader = new StreamReader(path);
        StreamWriter writer = new StreamWriter(path);
        reader.Close();
        List<string> key = new List<string>(resources[1].source.Keys);
        for (int i = 0; i < resources[1].source.Keys.Count; i++)
        {
            //Debug.Log(LanguageCore.Instance.GetText(key[i]));
            writer.Write(LanguageCore.Instance.GetText(key[i]));
        }
        writer.Close();
    }
    private void SaveAll()
    {
        longt = resources.Length;
        for(int i = 0; i < longt; i++)
        {
            SystemLanguage tempLang = resources[i].language;
            LanguageCore.Instance.LoadLanguage(tempLang);
            SearchLang(tempLang.ToString());
        }
    }

    private void SearchLang(string tempLang)
    {
        List<string> key = new List<string>(resources[1].source.Keys);   
        string[] rowDataTemp = new string[2];
        rowDataTemp[0] = "KEY;";
        rowDataTemp[1] = "TEXT;";
        rowData.Add(rowDataTemp);

        for (int j = 0; j < longt; j++)  
        {
            lang = resources[j].language.ToString();

            if(lang.Contains(tempLang))
            {
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
        rowData.Clear();
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
    }

    public string getPath()
    {
        AssetDatabase.Refresh();
        return Application.dataPath +"/StreamingAssets/" + lang + ".csv";
    }
}