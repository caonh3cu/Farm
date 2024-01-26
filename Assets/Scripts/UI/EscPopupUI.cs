using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class EscPopupUI : UIBase
    {
        public void Back()
        {
            Hide();
        }

        public void BackToMain()
        {
            gameObject.SetActive(false);
            // SceneManager.ChangeScene(SceneManager.GetScene<WorldGate>(), null);
        }
    }
}
