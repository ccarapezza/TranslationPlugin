using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class LanguageManager : EditorWindow
{
    private const string notFoundTextString = "Key not found in dictionary{0}";

    public static event System.Action onReload;

    public SystemLanguage defaultLanguage = SystemLanguage.English;
    public SystemLanguage currentLanguage;
    public LanguageResource[] resources;

    private Dictionary<string, string> m_dictionaryVar;
    private Dictionary<string, string> m_dictionary {
        get
        {
            if (m_dictionaryVar == null) {
                LoadLanguage(s_instance.currentLanguage);
            }
            return m_dictionaryVar;
        }
        set
        {
            m_dictionaryVar = value;
        }
    }

    private static LanguageManager s_instance;
    public static LanguageManager instance
    {
        get
        {
            if (s_instance == null)
            {
#if UNITY_EDITOR
                s_instance = FindObjectOfType<LanguageManager>();
                if (s_instance == null)
                {
                    s_instance = Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<LanguageManager>(UnityEditor.AssetDatabase.GUIDToAssetPath(UnityEditor.AssetDatabase.FindAssets("LanguageManager t:prefab")[0])));
                }
#endif
            }
            else
            {
                if (s_instance.m_dictionary == null)
                    LoadLanguage(s_instance.currentLanguage);
            }
            return s_instance;
        }
    }

    [MenuItem("PlugIn/Translation")]
    public static void ShowWindows()
    {
        ((LanguageManager)GetWindow(typeof(LanguageManager))).Show();
    }

    void OnGUI()
    {
        if (s_instance != null)
        {
            //Destroy(this.gameObject);
            //Destroy(this);
            //return;
        }

        if (Application.systemLanguage != SystemLanguage.Unknown)
            currentLanguage = Application.systemLanguage;
        else
            currentLanguage = defaultLanguage;

        s_instance = this;
        LoadLanguage(currentLanguage);

        if(Application.isPlaying)
            //DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(this);

    }

    public static void LoadLanguage(SystemLanguage lang)
    {
        int i = 0;
        LanguageResource[] rsc = s_instance.resources;
        for (i = 0; i < rsc.Length; i++)
        {
            if (rsc[i].language == lang)
            {
                s_instance.currentLanguage = lang;
                s_instance.m_dictionary = rsc[i].source;
                break;
            }
        }

        if (onReload != null)
            onReload();
    }

    public static string GetText(string key)
    {
        if (!string.IsNullOrEmpty(key) && instance.m_dictionary.ContainsKey(key))
            return instance.m_dictionary[key];
#if UNITY_EDITOR
        else
            Debug.LogWarningFormat(notFoundTextString, key);
#endif

        return string.Format(notFoundTextString, key);
    }

}