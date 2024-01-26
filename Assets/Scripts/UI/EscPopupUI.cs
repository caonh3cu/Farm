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
            Hide();
            GlobalUIManager.main_menu.Show();
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
