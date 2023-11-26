using UnityEngine;
using UnityEngine.UI;

namespace CAT
{
    [ExecuteAlways]
    public class ApplyText : MonoBehaviour
    {
        public string text;
        public int id;
        public MonoBehaviour comp;

        private void OnEnable()
        {
            Apply();
        }
    
        private void OnValidate()
        {
            Apply();
        }

        void Apply()
        {
            var real_text = LanguageManager.GetText(id, text);
            if (comp == null)
            {
                comp = GetComponent<Text>();
            }
            if (comp == null)
            {
                Debug.LogError("没有找到Text");
            }
            if (comp as Text)
            {
                (comp as Text).text = real_text;
            }
        }
    }

}