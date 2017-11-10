using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEditor;

public class WriteCSV : MonoBehaviour
{
    private List<string[]> rowData = new List<string[]>();
    private string[][] output;
    private string title;
    private int length;
    private string lang;

    public LanguageCore bleh;

    void Start()
    {
        Save();
    }

    void Save()
    {
        LanguageCore.Instance.LoadLanguage(SystemLanguage.English);
        SearchLang(SystemLanguage.English.ToString());
        
        LanguageCore.Instance.LoadLanguage(SystemLanguage.Spanish);
        SearchLang(SystemLanguage.Spanish.ToString());
  
        LanguageCore.Instance.LoadLanguage(SystemLanguage.Italian);
        SearchLang(SystemLanguage.Italian.ToString());

        /* 
         * - Solo test -
         * Reemplazar esto por un 'for' que recorra 
         * la lista de idiomas disponibles 
         * ej: '(LoadLanguage(listLangs[i]))'
         */
    }

    public void SearchLang(string tempLang)
    {
        List<string> key = new List<string>(LanguageCore.Instance.resources[1].source.Keys);   
        string[] rowDataTemp = new string[2];
        rowDataTemp[0] = "KEY;";
        rowDataTemp[1] = "TEXT;";
        rowData.Add(rowDataTemp);

        for (int j = 0; j < 3; j++)  
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
        rowData.Clear();
    }

    public void StreamValues()
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

    private string getPath()
    {
//#if UNITY_EDITOR
        return Application.dataPath +"/StreamingAssets/" + lang + ".csv";
//#endif
    }
}

