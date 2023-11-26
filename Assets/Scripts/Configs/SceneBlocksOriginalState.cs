using System;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    [CreateAssetMenu(fileName = "xxx_origin_state", menuName = "配置文件/场景初始状态配置", order = 0)]
    public class SceneBlocksOriginalState : ScriptableObject
    {
        [Serializable]
        public class Row
        {
            public SceneBlock[] row;
        }
        public Row[] origin_blocks;
        public float light_intencity;
        public float water;
        public List<Vector2Int> type_capacity;
    }
}