using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

namespace CAT
{

    public class WorldManager : ManagerBaseTemplate<WorldManager>
    {
        // [Serializable]
        // public class EntityConfig
        // {
        //     public string name;
        //     public string class_name;
        //     public string prefab_name;
        // }
        //
        // private static Dictionary<int, Type> _entity_id_to_type = new Dictionary<int, Type>();
        // private static Dictionary<Type, int> _entity_type_to_id = new Dictionary<Type, int>();
        // private static Dictionary<string, GameObject> _prefab_dict = new Dictionary<string, GameObject>();
        // private static int max_id = 0;
        //
        //
        // public static Dictionary<int, Entity> all_entity = new Dictionary<int, Entity>();
        // public static Dictionary<Type, Dictionary<int, Entity>> all_type_entity =
        //     new Dictionary<Type, Dictionary<int, Entity>>();
        // public static bool created = false;
        // public static EntityConfig[] configs => instance.entity_configs;
        //
        // public EntityConfig[] entity_configs;
        // public int time_pass_speed = 10; // 默认每秒十个时间单位
        // public GameObject[] entity_prefabs;
        // public float update_interval = 1f;
        // public float current_scene_update_interval = 0.1f;
        //
        //
        // private float _last_update_time;
        // private float _current_scene_last_update_time;
        //
        // private void Start()
        // {
        //     for (int i = 0; i < entity_configs.Length; i++)
        //     {
        //         var type_str = "CAT." + entity_configs[i].class_name;
        //         var type = Type.GetType(type_str);
        //         if (type == null || !type.IsSubclassOf(typeof(Entity)))
        //         {
        //             Debug.LogError($"类型不对:{type_str}");
        //             continue;
        //         }
        //         _entity_id_to_type[i] = type;
        //         _entity_type_to_id[type] = i;
        //     }
        //
        //     foreach (var prefab in entity_prefabs)
        //     {
        //         _prefab_dict[prefab.name] = prefab;
        //     }
        // }
        //
        // public static void Create()
        // {
        //     max_id = 0;
        //
        //     CreateEntity<WorldGate>(0, Vector2Int.zero, false);
        //     var home = CreateEntity<Home>(0, Vector2Int.zero, false);
        //     // CreateEntity<MainPlayer>(home.id, new Vector2Int(home.width / 2, home.height / 2), false);
        //     
        //     // CreateEntity<WorldGate>(0, Vector2.zero, false);
        //     
        //     created = true;
        // }
        //
        // public static void Destroy()
        // {
        //     created = false;
        // }
        //
        // // public string Save()
        // // {
        // //     return "";
        // // }
        // // public void ReCreateFromString(int _max_id, string world_data)
        // // {
        // //     // all_scene_blocks = _all_scene_blocks;
        // //     // max_id = _max_id;
        // // }
        //
        // public static Entity CreateEntity(int type_id, int scene_id, Vector2Int position, bool flying=false, Dictionary<string ,object> data = null)
        // {
        //     if (_entity_id_to_type.TryGetValue(type_id, out var type))
        //     {
        //         instance.DoCreateEntity(type, type_id, scene_id, position, flying, data);
        //     }
        //     return null;
        // }
        // public static T CreateEntity<T>(int scene_id, Vector2Int position, bool flying=false, Dictionary<string ,object> data = null) where T : Entity
        // {
        //     var type = typeof(T);
        //     
        //     if (_entity_type_to_id.TryGetValue(type, out var type_id))
        //     {
        //         return instance.DoCreateEntity(type, type_id, scene_id, position, flying, data) as T;
        //     }
        //     return null;
        // }
        //
        // public static void DestroyEntity(Entity entity)
        // {
        //     entity.scene.UnregisterEntity(entity);
        //     entity.OnDestroy();
        //     UnregisterEntity(entity);
        // }
        //
        //
        // public static Entity ChangeType(Entity entity, Type target_type, Dictionary<string ,object> data = null)
        // {
        //     if (entity.GetType().IsSubclassOf(typeof(Scene)) 
        //         || target_type.IsSubclassOf(typeof(Scene))
        //         || entity.GetType().IsSubclassOf(typeof(MainPlayer))
        //         || target_type.IsSubclassOf(typeof(MainPlayer))
        //         )
        //         return entity;
        //     if (_entity_type_to_id.TryGetValue(target_type, out var type_id))
        //     {
        //         var new_entity = Activator.CreateInstance(target_type) as Entity;
        //         new_entity.Init(entity.id, type_id, entity.scene, entity.position);
        //         new_entity.SetBlock(entity.block);
        //         entity.container.SetEntity(new_entity);
        //         UnregisterEntity(entity);
        //         RegisterEntity(new_entity);
        //         entity.scene.ChangeEntity(new_entity);
        //         new_entity.OnChanged(entity, data);
        //         entity.on_change_type.Invoke(new_entity);
        //         return new_entity;
        //     }
        //     return entity;
        // }
        //
        // public static GameObject GetEntityPrefab(int type_id)
        // {
        //     return _prefab_dict[configs[type_id].prefab_name];
        // }
        //
        // private Entity DoCreateEntity(Type type, int type_id, int scene_id, Vector2Int position, bool flying, Dictionary<string, object> data = null)
        // {
        //     if (type.IsSubclassOf(typeof(Scene)))
        //     {
        //         var scene_entity = Activator.CreateInstance(type) as Scene;
        //         var id = NewEntityID();
        //         scene_entity.Init(NewEntityID(), type_id, scene_entity, Vector2Int.zero);
        //
        //         RegisterEntity(scene_entity);
        //         SceneManager.all_scene.Add(scene_entity.id, scene_entity);
        //         scene_entity.OnCreate(data);
        //         return scene_entity;
        //     }
        //     
        //     // 默认创建到世界之门
        //     if (!SceneManager.all_scene.TryGetValue(scene_id, out var scene))
        //     {
        //         all_type_entity.TryGetValue(typeof(WorldGate), out var gates);
        //         if (gates == null)
        //         {
        //             Debug.LogError("世界之门不存在");
        //             return null;
        //         }
        //         foreach (var gate in gates.Values)
        //         {
        //             scene = gate as Scene;
        //         }
        //     }
        //
        //     if (!scene.IsEmpty(position) && !flying)
        //     {
        //         return null;
        //     }
        //     var entity = Activator.CreateInstance(type) as Entity;
        //     entity.Init(NewEntityID(), type_id, scene, position);
        //     RegisterEntity(entity);
        //     scene.RegisterEntity(entity, flying);
        //     entity.OnCreate(data);
        //     if (G.current_scene_id == scene_id)
        //     {
        //         scene.CreateContiner(entity);
        //     }
        //     return entity;
        // }
        //
        // private static void RegisterEntity(Entity entity)
        // {
        //     all_entity.Add(entity.id, entity);
        //     if (!all_type_entity.TryGetValue(entity.GetType(), out var entities))
        //     {
        //         entities = new Dictionary<int, Entity>();
        //         all_type_entity.Add(entity.GetType(), entities);
        //     }
        //     entities.Add(entity.id, entity);
        // }
        //
        // private static void UnregisterEntity(Entity entity)
        // {
        //     all_entity.Remove(entity.id);
        //     if (all_type_entity.TryGetValue(entity.GetType(), out var entities))
        //     {
        //         entities.Remove(entity.id);
        //     }
        // }
        //
        // private int NewEntityID()
        // {
        //     return max_id++;
        // }
        // private void Update()
        // {
        //     if (!created) return;
        //     Profiler.BeginSample("UpdateWorld");
        //     var dt = Time.time - _last_update_time;
        //     Profiler.EndSample();
        //     Profiler.BeginSample("UpdateWorld2");
        //     if (dt > update_interval)
        //     {
        //         _last_update_time = Time.time;
        //         foreach (var entity in all_entity.Values)
        //         {
        //             if(entity.id != G.current_scene_id)
        //                 entity.Update(dt);
        //         }
        //     }
        //     Profiler.EndSample();
        //     Profiler.BeginSample("UpdateWorld3");
        //     dt = Time.time - _current_scene_last_update_time;
        //     if (dt > current_scene_update_interval)
        //     {
        //         Profiler.BeginSample("UpdateWorld4");
        //         _current_scene_last_update_time = Time.time;
        //         Profiler.EndSample();
        //         G.current_scene?.Update(dt);
        //     }
        //     Profiler.EndSample();
        // }
    }
}