
using System;
using UnityEngine;
namespace CAT
{
    public class MainMenu : MonoBehaviour
    {
        
        private void OnEnable()
        {
            // 进入世界之门，剥夺操纵权，后面可以释放魔法获得操纵权，并隐藏主界面UI
            G.control_right = false;
        }

        private void OnDisable()
        {
            G.control_right = true;
        }

        public void Play()
        {
            // if (WorldManager.created)
            // {
            //     WorldManager.Destroy();
            // }
            // WorldManager.Create();
            // SceneManager.ChangeScene(SceneManager.GetScene<Home>(), () =>
            // {
            //     GlobalUIManager.main_screen.Show();
            // });
        }

        public void Continue()
        {
        
        }

        public void Options()
        {
        
        }

        public void Quit()
        {
            Application.Quit();
        }


    }

}