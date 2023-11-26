using UnityEngine;
using UnityEngine.UI;

namespace CAT
{
    public class LoadingUI : UIBase
    {
        public Text text;

        public void SetProgress(float progress)
        {
            text.text = (progress*100).ToString("0.00%");
        }
    }

}