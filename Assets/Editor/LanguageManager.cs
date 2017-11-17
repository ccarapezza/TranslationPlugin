using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
public class LanguageManager : EditorWindow
{
    /*public IFormattable defaultLanguage = SystemLanguage.English;
    public IFormattable currentLanguage;*/
    public int index;
    /*public LanguageResource[] resources; */

    private List<string[]> rowData = new List<string[]>();
    private string[][] output;
    private string titled;
    private int length;
    static private string lang;
    private int longt;
    private string csvPath;

    [MenuItem("Plugin/Translation")]
    public static void ShowWindows()
    {
        ((LanguageManager)GetWindow(typeof(LanguageManager))).Show();
    }

    void OnEnable()
    {
        SaveAll();
    }

    void OnGUI()
    {

        EditorGUILayout.LabelField("Languages", EditorStyles.boldLabel);

        LanguageCore.Instance.ReloadLangResAssets();
        string[] langs = new string[LanguageCore.Instance.resources.Length];
        for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
            langs[i] = LanguageCore.Instance.resources[i].language.ToString();

        int indexAnt = index;
        index = EditorGUILayout.Popup(index, langs);

        LanguageCore.Instance.LoadLanguage(LanguageCore.Instance.resources[index].language);

        var tempLang = LanguageCore.Instance.resources[index].language.ToString();

        if (index != indexAnt)
            SearchLang(tempLang);

        EditorGUILayout.Space();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("OPEN", GUILayout.Width(105), GUILayout.Height(35)))
            csvPath = EditorUtility.OpenFilePanel("Overwrite with csv", "Assets/StreammingAssets", "csv");
        if (GUILayout.Button("▼IMPORT▼", GUILayout.Width(105), GUILayout.Height(35)))
        {
            var reader = new StreamReader(csvPath);
            string langLine = reader.ReadLine();
            string csvLang = langLine.Split(';')[1];

            SystemLanguage csvCurrentLang = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), csvLang, true);
            bool isExist = false;
            for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
            {
                if (LanguageCore.Instance.resources[i].language == csvCurrentLang)
                    isExist = true;
            }
            if (isExist)
            {
                if (EditorUtility.DisplayDialog("The language already exists", "Do you want to overwrite the data?", "Yes", "No"))
                {
                    FileUtil.DeleteFileOrDirectory("Assets/Langs/" + GetImportPath(csvPath) + ".asset");
                    AssetDatabase.Refresh();
                    ImportLang(csvPath, reader, csvCurrentLang);
                }
            }
            else
            {
                ImportLang(csvPath, reader, csvCurrentLang);
            }

            //LanguageCore.Instance.resources.Contains()
            //LanguageCore.Instance.resources.Contains()

            /*for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
            {  
                langs[i] = LanguageCore.Instance.resources[i].language.ToString();
                if (langs[i].Contains(tempVar))
                {
                    count++;
                    if (EditorUtility.DisplayDialog("The language already exists", "Do you want to overwrite the data?", "Yes", "No"))
                    {
                        FileUtil.DeleteFileOrDirectory("Assets/Langs/" + tempVar + ".asset");
                        AssetDatabase.Refresh();
                        ImportLang(tempPath);
                    }
                }
            }*/

            //Falta que inserte los datos del csv en las keys correspondientes
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("▲EXPORT▲ ", GUILayout.Width(105), GUILayout.Height(35)))
            SearchLang(tempLang);
        if (GUILayout.Button("SAVE ALL", GUILayout.Width(105), GUILayout.Height(35)))
            SaveAll();

        EditorGUILayout.Space();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    /*public string FindPath()
    {
        return 
    }*/

    private string ImportLang(string langName, StreamReader reader, SystemLanguage lang)
    {
        LanguageResource langResourceAsset = CreateInstance<LanguageResource>();
        reader.ReadLine();
        string translatesStr = reader.ReadToEnd();
        string[] translates = translatesStr.Split('\n');

        langResourceAsset.language = lang;
        for (int i = 0; i < translates.Length; i++)
        {
            string[] line = translates[i].Split(';');
            if (line.Length < 2) continue;
            langResourceAsset.source.Add(line[0], line[1]);
        }

        string path = "Assets/Langs";
        langName = GetImportPath(langName);
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + langName + ".asset");
        AssetDatabase.CreateAsset(langResourceAsset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = langResourceAsset;
        AssetDatabase.Refresh();
        return langName;
    }

    private string GetImportPath(string langName)
    {
        langName = Path.GetFileName(langName);
        var langResPos = langName.IndexOf(".csv");
        langName = langName.Remove(langResPos);
        langName = langName.Trim();
        return langName;
    }

    private void SaveAll()
    {
        longt = LanguageCore.Instance.resources.Length;
        for (int i = 0; i < longt; i++)
        {
            SystemLanguage tempLang = LanguageCore.Instance.resources[i].language;
            LanguageCore.Instance.LoadLanguage(tempLang);
            SearchLang(tempLang.ToString());
        }
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

            if (lang.Contains(tempLang))
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
        for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
        {
            if(LanguageCore.Instance.resources[i].language.ToString().Contains(lang))
            {
                titled = "LANGUAGE" + ";" + lang.ToUpper() + ";";
                output = new string[rowData.Count][];

                for (int j = 0; j < output.Length; j++)
                    output[j] = rowData[j];

                length = output.GetLength(0);
                StringBuilder sb = new StringBuilder();

                for (int index = 0; index < length; index++)
                    sb.AppendLine(string.Join("", output[index]));

                string filePath = getPath();
                StreamWriter outStream = System.IO.File.CreateText(filePath);
                outStream.WriteLine(titled);
                outStream.WriteLine(sb);
                outStream.Close();
            }
        }
    }

    public string getPath()
    {
        AssetDatabase.Refresh();
        return Application.dataPath + "/StreamingAssets/" + lang + ".csv";
    }
}