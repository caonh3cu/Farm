using System;
using Newtonsoft.Json;

namespace CAT
{
    [Serializable]
    public struct SceneBlock
    {
        public int type;
        public double temperature;
        public double water;
        public float z;
        public int entity_id;
        public float state_time;
    }
}