using UnityEditor;
using UnityEditor.SceneManagement;
namespace CAT
{
    public class FastMenuItem
    {
        [MenuItem("种田/主场景 %g")]
        static void GotoMainScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        }
    }
} 