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
            if (menuCommand.context != null)
                parentGo.transform.parent = ((GameObject)menuCommand.context).transform;
            else
                parentGo.transform.parent = canvas.transform;

            Text textComponent = parentGo.AddComponent<Text>();
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(textComponent, "Create " + textComponent.name);
            Selection.activeObject = textComponent;

            textComponent.key = textComponent.GenerateKey();
            LanguageCore.Instance.AddKeyToAllLanguages(textComponent.key);
        }
#endif

        private string GenerateKey()
        {
            string keyName = "";
            Transform root = transform;
            while (root != null)
            {
                if (keyName == "")
                    keyName = root.name;
                else
                    keyName = root.name + "." + keyName;
                root = root.transform.parent;
            }

            return keyName;
        }

        public override string text
        {
            get
            {
                m_Text = LanguageCore.Instance.GetText(key);
                return m_Text;
            }
        }

        public void Reload()
        {
            UpdateGeometry();
        }

        protected override void OnDestroy() {
            Debug.Log("Delete keys: " + key);
            LanguageCore.Instance.RemoveKeyToAllLanguages(key);
            base.OnDestroy();
        }

        private void Update()
        {
            if (key != GenerateKey())
            {
                LanguageCore.Instance.ReplaceKeyInAllLanguages(GenerateKey(), key);
                key = GenerateKey();
                Debug.Log("change");
            }
        }
    }
}