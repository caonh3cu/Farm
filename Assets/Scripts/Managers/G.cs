using UnityEngine;

namespace CAT
{
    public class G : ManagerBaseTemplate<G>
    {
        public static int current_scene_id => current_scene == null ? -1 : current_scene.id;
        public static Scene current_scene;
        public static MainPlayer main_player;
        public static bool control_right; // 是否显示主界面UI
    }
}