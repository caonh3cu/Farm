using System;
using UnityEngine;

namespace CAT
{
    public class EntityContainer : MonoBehaviour
    {
        public Entity entity { get; private set; }

        internal void SetEntity(Entity _entity)
        {
            entity = _entity;
            entity.SetContiner(this);
            entity.OnMonoStart();
        }

        private void Update()
        {
            entity?.OnMonoUpdate();
        }
        
        internal void RemoveEntity(Entity _entity)
        {
            entity?.OnMonoDestroy();
            entity = null;
        }

        
    }
}