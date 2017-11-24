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
                instance.ReloadLangResAssets();
                instance.LoadLanguage(SystemLanguage.English);
            }
            return instance;
        }
    }

    public void LoadLanguage(SystemLanguage lang)
    {
        for (int i = 0; i < instance.resources.Length; i++)
        {
            if (instance.resources[i].language == lang)
            {
                EditorPrefs.SetInt("currentLanguage", i);
                instance.currentLanguage = lang;
                instance.m_dictionary = instance.resources[i].source;
                break;
            }
        }
        TranslationPlugin.UI.Text[] translatableTexts = GameObject.FindObjectsOfType<TranslationPlugin.UI.Text>();
        foreach (var translatableText in translatableTexts)
        {
            if (translatableText.enabled)
            {
                translatableText.enabled = false;
                translatableText.enabled = true;
            }
        }
    }

    public void ReloadLangResAssets()
    {
        List<LanguageResource> lrs = new List<LanguageResource>();
        //Se carga el diccionario
        string[] langFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);
        foreach (string langFile in langFiles)
        {
            string assetPath = "Assets" + langFile.Replace(Application.dataPath, "").Replace('\\', '/');
            LanguageResource sourceLang = (LanguageResource)AssetDatabase.LoadAssetAtPath(assetPath, typeof(LanguageResource));
            lrs.Add(sourceLang);
        }
        instance.resources = lrs.ToArray();
    }

    public void AddKeyToAllLanguages(string key) {
        ReloadLangResAssets();
        foreach (LanguageResource langRes in instance.resources)
        {
            langRes.AddNewKeys(new List<string>(new string[1] { key }));
        }
    }

    public void ReplaceKeyInAllLanguages(string newKey, string oldKey)
    {
        ReloadLangResAssets();
        foreach (LanguageResource langRes in instance.resources)
        {
            string value = langRes.source[oldKey];
            if (value != null)
            {
                langRes.source.Remove(oldKey);
                langRes.source.Add(newKey, value);
            }
        }
    }

    public void RemoveKeyToAllLanguages(string key)
    {
        ReloadLangResAssets();
        foreach (LanguageResource langRes in instance.resources)
        {
            langRes.source.Remove(key);
        }
    }

    public string GetText(string key)
    {
        if (!string.IsNullOrEmpty(key) && instance.m_dictionary.ContainsKey(key))
            return instance.m_dictionary[key];
        return string.Format(notFoundTextString, key);
    }
}