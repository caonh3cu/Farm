using UnityEngine;

namespace CAT
{
    public class G : ManagerBaseTemplate<G>
    {
        public static bool player_control => initialized && GlobalUIManager.main_screen.is_show;
        public static MainPlayer main_player => instance._main_player;
        public static Camera main_camera => instance._main_camera;
        
        public MainPlayer _main_player;
        public Camera _main_camera;
    }
}