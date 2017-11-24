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
    public int indexCurrentLang;
    public int selectIndexLangToExport;
    private List<string[]> rowData = new List<string[]>();
    private string[][] output;
    private string titled;
    private int length;
    static private string lang;
    private int longt;
    private string csvPath;

    private int toolbarInt = 0;
    private Texture tex;
    private Font fuente;

    public float startVal = 0;
    public float progress = 0;
    public float secs = 20;

    [MenuItem("Plugin/Translation")]
    public static void ShowWindows()
    {
        ((LanguageManager)GetWindow(typeof(LanguageManager))).Show();
    }

    void OnEnable()
    {
        for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
        {
            if (LanguageCore.Instance.currentLanguage == LanguageCore.Instance.resources[i].language)
            {
                indexCurrentLang = i;
                break;
            }
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        maxSize = new Vector2(400, 600);
        minSize = new Vector2(400, 600);

        var titleLabelStyle = new GUIStyle(GUI.skin.label);
        titleLabelStyle.fontSize = 22;
        titleLabelStyle.fixedHeight = 25;
        titleLabelStyle.fontStyle = FontStyle.Bold;
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;
        buttonStyle.font = fuente;
        var button2Style = new GUIStyle(GUI.skin.button);
        button2Style.fontSize = 8;
        button2Style.font = fuente;

        var title = new GUIStyle(GUI.skin.label);
        title.fontSize = 12;
        title.font = fuente;

        fuente = (Font)AssetDatabase.LoadAssetAtPath("Assets/Art/Font/Gameplay.ttf", typeof(Font));

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Languages", titleLabelStyle);
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.Space();

        LanguageCore.Instance.ReloadLangResAssets();
        string[] languages = new string[LanguageCore.Instance.resources.Length];
        for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
            languages[i] = LanguageCore.Instance.resources[i].language.ToString();

        int oldIndexLang = indexCurrentLang;

        GUILayout.BeginHorizontal();
        Texture flagTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Art/Flags/" + LanguageCore.Instance.currentLanguage.ToString() + ".png", typeof(Texture));
        GUILayout.Box(flagTexture);
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Current Language", title);
        EditorGUILayout.Space();
        indexCurrentLang = EditorGUILayout.Popup(indexCurrentLang, languages);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        if (indexCurrentLang != oldIndexLang)
            LanguageCore.Instance.LoadLanguage(LanguageCore.Instance.resources[indexCurrentLang].language);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("Export", title);

        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        toolbarInt = GUILayout.Toolbar(toolbarInt, new string[] { "One Language", "All Languages" });
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (toolbarInt == 0)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            selectIndexLangToExport = EditorGUILayout.Popup("Language to export:", selectIndexLangToExport, languages);
            EditorGUILayout.Space();
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("E X P O R T", buttonStyle, GUILayout.Width(150), GUILayout.Height(75)))
        {
            //startVal = (float)EditorApplication.timeSinceStartup;
            if (toolbarInt == 0)
                ExportLang(LanguageCore.Instance.resources[selectIndexLangToExport]);
            else if (toolbarInt == 1)
                ExportAllLangs();
        }
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Import", title);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUIStyle a = GUIStyle.none;
        a.border = new RectOffset(50,50,50,50);
        EditorGUILayout.LabelField("CSV File: ", csvPath, GUI.skin.textField);

        if (GUILayout.Button("O P E N", button2Style, GUILayout.Width(50)))
            csvPath = EditorUtility.OpenFilePanel("Overwrite with csv", "Assets/StreammingAssets", "csv");

        GUILayout.EndHorizontal();
        GUI.enabled = csvPath != null && csvPath != "";


        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("I M P O R T", buttonStyle, GUILayout.Width(150), GUILayout.Height(75)))
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
                    AssetDatabase.Refresh();
                    ImportLang(reader, csvCurrentLang);
                }
            }
            else
            {
                ImportLang(reader, csvCurrentLang);
            }
            csvPath = null;
        }
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();
    }

    private void ImportLang(StreamReader reader, SystemLanguage lang)
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

        string path = "Assets/Langs/" + lang.ToString() + ".asset";
        FileUtil.DeleteFileOrDirectory(path);
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(langResourceAsset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = langResourceAsset;
        AssetDatabase.Refresh();
    }

    private void ExportAllLangs()
    {
        for (int i = 0; i < LanguageCore.Instance.resources.Length; i++)
            ExportLang(LanguageCore.Instance.resources[i]);

        AssetDatabase.Refresh();
    }

    private void ExportLang(LanguageResource langResource)
    {
        string languageName = langResource.language.ToString();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("LANGUAGE" + ";" + languageName.ToUpper() + ";");
        sb.AppendLine("KEY;TEXT;");

        foreach (KeyValuePair<string, string> element in langResource.source)
            sb.AppendLine(element.Key + ";" + element.Value);

        string filePath = GenerateExportPath(languageName);
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
        AssetDatabase.Refresh();
    }

    private string GenerateExportPath(string fileName)
    {
        AssetDatabase.Refresh();
        return Application.dataPath + "/StreamingAssets/" + fileName + ".csv";
    }
}
 
 