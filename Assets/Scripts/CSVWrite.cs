using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class CSVWrite : MonoBehaviour
{

    private List<string[]> rowData = new List<string[]>();
    private string[][] output;
    private int length;
    private string delimiter = "";

    public string lang;

    void Start()
    {
        Save();
    }

    void Save()
    {
        List<string> key = new List<string>(LanguageCore.Instance.resources[0].source.Keys);   

        string[] rowDataTemp = new string[3];
        rowDataTemp[0] = "LANGUAGE;";
        rowDataTemp[1] = "KEY;";
        rowDataTemp[2] = "TEXT;";
        rowData.Add(rowDataTemp);

        for (int j = 0; j < 3; j++)
        {
            lang = LanguageCore.Instance.resources[j].ToString();
            if(lang.Contains("Spanish"))
            for (int i = 0; i < key.Count; i++)
            {
                string var = LanguageCore.Instance.GetText(key[i]);
                rowDataTemp = new string[3];
                rowDataTemp[0] =  lang + ";";
                rowDataTemp[1] = key[i] + ";";
                rowDataTemp[2] = var + ";";
                rowData.Add(rowDataTemp);
                randomFunct(i);  
            }
        }
    }

    public void randomFunct(int j)
    {
        output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        length = output.GetLength(0);
        delimiter = ""; //,

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        string filePath = getPath(j);

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    private string getPath(int j)
    {
//#if UNITY_EDITOR
        return Application.dataPath +"/StreamingAssets/" + lang + ".csv";
//#endif
    }
}

