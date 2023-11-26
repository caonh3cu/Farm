using UnityEngine;

namespace CAT
{
    public class UIBase : MonoBehaviour
    {
        public bool is_show => gameObject.activeSelf;
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}