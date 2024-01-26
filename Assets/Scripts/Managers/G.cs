using UnityEngine;

namespace CAT
{
    public class G : ManagerBaseTemplate<G>
    {
        public static MainPlayer main_player => instance._main_player;
        public static Camera main_camera => instance._main_camera;
        public static bool control_right; // 是否显示主界面UI
        public MainPlayer _main_player;
        public Camera _main_camera;
    }
}