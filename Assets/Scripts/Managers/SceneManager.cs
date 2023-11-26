
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CAT
{
    [ExecuteInEditMode]
    public class SceneManager : ManagerBaseTemplate<SceneManager>
    {
        [Serializable]
        public class SceneConfig
        {
            public string name;
            public string path;
            public string class_name;
            public SceneBlocksOriginalState original_state;
        }
        [Serializable]
        public class BlockConfig
        {
            public string name;
            public float 导热率;
            public float 蒸发率;
            public float 蒸发温度;
        }
        public SceneConfig[] scene_configs;
        public BlockConfig[] block_configs;
        
        public static Dictionary<int, Scene> all_scene = new Dictionary<int, Scene>();

        public static Dictionary<int, Type> scene_id_to_type = new Dictionary<int, Type>();
        public static Dictionary<Type, int> scene_type_to_id = new Dictionary<Type, int>();
        public static BlockConfig[] block_config => instance.block_configs;

        private void Start()
        {
            for (int i = 0; i < scene_configs.Length; i++)
            {
                var type_str = "CAT." + scene_configs[i].class_name;
                var type = Type.GetType(type_str);
                if (type == null || !type.IsSubclassOf(typeof(Scene)))
                {
                    Debug.LogError($"类型不对:{type_str}");
                    continue;
                }
                scene_id_to_type[i] = type;
                scene_type_to_id[type] = i;
            }
        }

        public static T GetScene<T>() where T : Scene
        {
            if(WorldManager.all_type_entity.TryGetValue(typeof(T), out var scenes))
            {
                foreach (var scene in scenes.Values)
                {
                    return scene as T;
                }
            }
            return null;
        }

        public static SceneConfig GetConfig<T>()
        {
            return GetConfig(typeof(T));
        }

        public static SceneConfig GetConfig(Type type)
        {
            if (scene_type_to_id.TryGetValue(type, out var offset))
                return instance.scene_configs[offset];
            return null;
        }

        public static void ChangeScene(Scene scene, UnityAction after)
        {
            instance.StartChangeScene(scene, after);
        }

        private void StartChangeScene(Scene scene, UnityAction after)
        {
            StartCoroutine(LoadScene(scene, after));
        }
        
        IEnumerator LoadScene(Scene scene, UnityAction after)
        {
            G.current_scene?.OnMonoDestroy();
            G.current_scene?.SetContiner(null);
            G.current_scene = null;
            var sync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetConfig(scene.GetType()).path);
            sync.allowSceneActivation = false;
            GlobalUIManager.loading.Show();
            while (sync.progress<0.9f)
            {
                GlobalUIManager.loading.SetProgress(sync.progress);
                yield return new WaitForEndOfFrame();
            }
            GlobalUIManager.loading.SetProgress(1);
            GlobalUIManager.loading.Hide();
            sync.allowSceneActivation = true;
            G.current_scene = scene;
            after?.Invoke();
        }
    }

}