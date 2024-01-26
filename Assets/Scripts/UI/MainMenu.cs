
using System;
using UnityEngine;
namespace CAT
{
    public class MainMenu : UIBase
    {

        // Start is called before the first frame update
        public void StartGame()
        {
            GlobalUIManager.main_screen.Show();
            Hide();
        }
        public void Quit()
        {
            Application.Quit();
        }


    }

}