using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace TranslationPlugin.UI
{
    public class Text : UnityEngine.UI.Text
    {

        public string key;
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/TranslatableText", false)]
        public static void CreateTranslatableText(MenuCommand menuCommand)
        {
            Canvas canvas = FindObjectOfType<Canvas>();

            GameObject parentGo = new GameObject("TranslationPlugin.Text");
            parentGo.layer = LayerMask.NameToLayer("UI");
            parentGo.transform.parent = canvas.transform;
            Text textComponent = parentGo.AddComponent<Text>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(textComponent, "Create " + textComponent.name);
            Selection.activeObject = textComponent;
        }
#endif

        /// <summary>
        /// Text that's being displayed by the Text.
        /// </summary>
        public override string text
        {
            get
            {
                m_Text = LanguageManager.GetText(key);
                return m_Text;
            }
        }
    }
}