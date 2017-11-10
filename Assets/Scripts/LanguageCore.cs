using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LanguageCore
{
    private const string notFoundTextString = "Key not found in dictionary{0}";

    public static event System.Action onReload;
    public SystemLanguage defaultLanguage = SystemLanguage.English;
    public SystemLanguage currentLanguage;
    public LanguageResource[] resources;

    private Dictionary<string, string> m_dictionaryVar;
    private Dictionary<string, string> m_dictionary
    {
        get
        {
            /*if (m_dictionaryVar == null)
            {
                LoadLanguage(instance.currentLanguage);
            }*/
            return m_dictionaryVar;
        }
        set
        {
            m_dictionaryVar = value;
        }
    }

    private static LanguageCore instance;

    private LanguageCore() { }

    public static LanguageCore Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LanguageCore();
                List<LanguageResource> lrs = new List<LanguageResource>();
                //Se carga el diccionario
                //TODO: Modificar la manera de traer los LanguageResources por AssetDatabase
                string[] langFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);
                foreach (string langFile in langFiles)
                {
                    string assetPath = "Assets" + langFile.Replace(Application.dataPath, "").Replace('\\', '/');
                    LanguageResource sourceLang = (LanguageResource)AssetDatabase.LoadAssetAtPath(assetPath, typeof(LanguageResource));
                    lrs.Add(sourceLang);
                    //Debug.Log(sourceLang.master);
                }
                instance.resources = lrs.ToArray();
                //Debug.Log("Lenguajes: " + instance.resources.);
                instance.LoadLanguage(SystemLanguage.Spanish);
            }
            return instance;
        }
    }

    public void LoadLanguage(SystemLanguage lang)
    {
        //Debug.Log("Load Language: " + lang);
        for (int i = 0; i < instance.resources.Length; i++)
        {
            if (instance.resources[i].language == lang)
            {
                instance.currentLanguage = lang;
                instance.m_dictionary = instance.resources[i].source;
                break;
            }
        }

        if (onReload != null)
            onReload();
    }

    public string GetText(string key)
    {
        if (!string.IsNullOrEmpty(key) && instance.m_dictionary.ContainsKey(key))
            return instance.m_dictionary[key];
        return string.Format(notFoundTextString, key);
    }
}