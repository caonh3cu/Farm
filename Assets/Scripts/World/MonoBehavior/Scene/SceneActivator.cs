using System;
using UnityEngine;

namespace CAT
{
    public class SceneActivator : MonoBehaviour
    {
        private void OnEnable()
        {
            if (!WorldManager.initialized) return; 
            var entity_container = GetComponent<EntityContainer>();
            entity_container.SetEntity(G.current_scene);
            G.current_scene.OnMonoStart();
        }
    }
}