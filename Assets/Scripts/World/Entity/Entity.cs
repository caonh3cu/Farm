using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace CAT
{
    [Serializable]
    public class Entity
    {
        [JsonIgnore] public EntityContainer container{ get; private set; }
        [JsonIgnore] public Scene scene { get; private set; }
        public int type { get; private set; }
        public int id { get; private set; }
        public int scene_id { get; private set; }
        public Vector2Int position { get; private set; }

        public Vector2Int block { get; private set; }
        public bool is_flying => block.x == Int32.MaxValue;

        public UnityAction<Entity> on_change_type;

        internal void Init(int _id, int _type, Scene _scene, Vector2Int _position)
        {
            type = _type;
            id = _id;
            scene = _scene;
            scene_id = _scene.id;
            position = _position;
            block = new Vector2Int(Int32.MaxValue, Int32.MaxValue);
        }

        internal void SetBlock(Vector2Int _block)
        {
            block = _block;
        }

        internal void SetContiner(EntityContainer _container)
        {
            container = _container;
        }
        
        public virtual void OnCreate(Dictionary<string, object> dict)
        {
            
        }

        public virtual void OnInitFromDict()
        {
            
        }

        public virtual void OnChanged(Entity from, Dictionary<string, object> dict)
        {
            OnCreate(dict);
        }
        
        public virtual void Update(float dt)
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }

        public virtual void OnMonoStart()
        {
            
        }

        public virtual void OnMonoUpdate()
        {
            
        }

        public virtual void OnMonoDestroy()
        {
            
        }
    }
}