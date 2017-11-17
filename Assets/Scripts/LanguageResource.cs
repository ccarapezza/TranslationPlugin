using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

[System.Serializable]
public class Element
{
    public string key;
    public string text;
}

[CreateAssetMenu(fileName = "LanguageResource", menuName = "Language Resource", order = 150)]
public class LanguageResource : ScriptableObject, ISerializationCallbackReceiver
{
    public LanguageResource master;
    public SystemLanguage language;
    public List<Element> elements = new List<Element>();
    public Dictionary<string, string> source = new Dictionary<string, string>(100);

    public void OnBeforeSerialize()
    {
        master  = (LanguageResource)AssetDatabase.LoadAssetAtPath("Assets/Langs/English.asset", typeof(LanguageResource)); 

        elements.Clear();
        var keys = new List<string>(source.Keys);

        for (var i = 0; i < keys.Count; i++)
            elements.Add(new Element { key = keys[i], text = source[keys[i]] });
    }

    public void OnAfterDeserialize()
    {
        int i;
        Element element;
        source.Clear();        

        for (i = 0; i < elements.Count; i++)
        {
            element = elements[i];
            source.Add(element.key, element.text);
        }
    }

    public void AddNewKeys(List<string> newKeys)
    {
        int i;
        for (i = 0; i < newKeys.Count; i++)
        {
            source.Add(newKeys[i], newKeys[i]);
        }
    }
}