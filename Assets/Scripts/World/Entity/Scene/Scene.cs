using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Profiling;

namespace CAT
{
    public class Scene : Entity
    {
        public int width, height;
        public float light_intencity;
        public double water;
        public float rain_speed;
        public Dictionary<int, int> type_capacity = new Dictionary<int, int>();

        [JsonIgnore] private SceneBlock[][] _blocks1;
        [JsonIgnore] private SceneBlock[][] _blocks2;
        public SceneBlock[][] blocks => _blocks1;
        
        [JsonIgnore]
        public Dictionary<int, Entity> scene_entities = new Dictionary<int, Entity>();
        public Dictionary<int, int> type_num = new Dictionary<int, int>();
        public override void OnCreate(Dictionary<string, object> dict)
        {
            var config = SceneManager.GetConfig(GetType());
            DoCreate(config.original_state);
        }

        public void DoCreate(SceneBlocksOriginalState original_state)
        {
            width = original_state.origin_blocks.Length;
            height = original_state.origin_blocks[0].row.Length;
            if (original_state.origin_blocks.Length != width)
            {
                Debug.LogError($"尺寸不对{type}");
                return;
            }
            _blocks1 = new SceneBlock[width][];
            _blocks2 = new SceneBlock[width][];
            for (int i = 0; i < width; i++)
            {
                var row = original_state.origin_blocks[i].row;
                if (row.Length != height)
                {
                    Debug.LogError($"尺寸不对{type}");
                    return;
                }
                _blocks1[i] = new SceneBlock[height];
                _blocks2[i] = new SceneBlock[height];
                for (int j = 0; j < height; j++)
                {
                    _blocks1[i][j] = row[j];
                    _blocks1[i][j].state_time = 0;
                    _blocks2[i][j] = row[j];
                    _blocks2[i][j].state_time = 0;
                }
            }
            light_intencity = original_state.light_intencity;
            water = original_state.water;
            foreach (var pair in original_state.type_capacity)
            {
                type_capacity[pair.x] = pair.y;
            }
            rain_speed = 0;
        }

        public override void OnInitFromDict()
        {
            
        }

        public bool IsEmpty(Vector2Int coord)
        {
            return IsEmpty(coord.x, coord.y);
        }
        public bool IsEmpty(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= width)
                return false;
            return blocks[x][y].entity_id == 0;
        }
        internal void RegisterEntity(Entity entity, bool flying)
        {
            if (!flying)
            {
                SetEntity(entity);
            }
            scene_entities.Add(entity.id, entity);
            if (!type_num.TryGetValue(entity.type, out var num))
            {
                num = 0;
            }
            type_num[entity.type] = num + 1;
        }

        internal void UnregisterEntity(Entity entity)
        {
            if (scene_entities.TryGetValue(entity.id, out entity))
            {
                if (G.current_scene_id == id)
                {
                    RemoveContiner(entity);
                }
                if (!entity.is_flying)
                {
                    RemoveEntity(entity.block);
                }
                scene_entities.Remove(entity.id);
                type_num[entity.type] -= 1;
            }
        }

        internal void ChangeEntity(Entity entity)
        {
            scene_entities[entity.id] = entity;
        }

        public override void OnMonoStart()
        {
            foreach (var entity in scene_entities.Values)
            {
                CreateContiner(entity);
            }
        }

        public override void OnMonoDestroy()
        {
            foreach (var entity in scene_entities.Values)
            {
                RemoveContiner(entity);
            }
        }

        internal void CreateContiner(Entity entity)
        {
            var prefab = WorldManager.GetEntityPrefab(entity.type);
            var entity_container = GameObject.Instantiate(prefab, container.transform).AddComponent<EntityContainer>();
            entity_container.transform.localPosition = new Vector3(entity.position.x, 0, entity.position.y);
            entity_container.SetEntity(entity);
            entity.OnMonoStart();
        }

        internal void RemoveContiner(Entity entity)
        {
            var entity_container = entity.container;
            entity.OnMonoDestroy();
            entity.SetContiner(null);
            GameObject.Destroy(entity_container.gameObject);
        }

        private void SetEntity(Entity entity)
        {
            blocks[entity.position.x][entity.position.y].entity_id = entity.id;
            entity.SetBlock(entity.position);
        }
        private void RemoveEntity(Vector2Int block_id)
        {
            blocks[block_id.x][block_id.y].entity_id = 0;
        }

        public override void Update(float dt)
        {
            var current_light = light_intencity;
            Profiler.BeginSample($"UpdateScene{GetType().Name}, {width*height} blocks");
            UpdateBlocks(dt, current_light);
            Profiler.EndSample();
        }

        public void UpdateBlocks(float dt, float current_light)
        {
            
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    var block = blocks[x][y];
                    {
                        var type = block.type;
                        var config = SceneManager.block_config[type];
                        block.state_time += dt;
                        block.temperature += current_light * 光照强度比例 * dt;
                        var evaporation = Math.Max(0, block.temperature - config.蒸发温度) * config.蒸发率 * 蒸发率比例 * dt;
                        if (block.water > evaporation)
                        {
                            block.water -= evaporation;
                            block.temperature -= evaporation * 每点水带走热量;
                            water += evaporation;
                        }
                        else
                        {
                            block.temperature -= evaporation * 每点水带走热量;
                            water += block.water;
                            block.water = 0;
                        }
                        var rain = rain_speed * dt;
                        block.water += rain;
                        water -= rain;
                        
                        BlockType type_enum = (BlockType)type;
                        switch (type_enum)
                        {
                            case BlockType.荒漠:
                            {
                                if (block.temperature < 0)
                                    type_enum = BlockType.雪地;
                                else if (block.water > 荒漠临界)
                                {
                                    type_enum = BlockType.土壤;
                                }
                                break;
                            }
                            case BlockType.土壤:
                            {
                                if (block.water < 荒漠临界)
                                    type_enum = BlockType.荒漠;
                                else if (block.water > 水域临界)
                                    type_enum = BlockType.水面;
                                else if (block.temperature < 0)
                                    type_enum = BlockType.雪地;
                                else if (block.temperature > 草的生长温度.y || block.temperature < 草的生长温度.x)
                                    block.state_time = 0;
                                else if (block.state_time > 长草时间)
                                    type_enum = BlockType.草地;
                                break;
                            }
                            case BlockType.草地:
                            {
                                if (block.temperature < 草的生长温度.x || block.temperature > 草的生长温度.y || block.water<荒漠临界)
                                    type_enum = BlockType.枯草;
                                else if (block.water > 水域临界)
                                    type_enum = BlockType.水面;
                                break;
                            }
                            case BlockType.枯草:
                            {
                                if (block.temperature > 枯草燃点)
                                {
                                    // todo.
                                }
                                else if (block.state_time > 枯草腐烂时间)
                                    type_enum = BlockType.土壤;
                                break;
                            }
                            case BlockType.雪地:
                            {
                                if (block.temperature > 0)
                                    type_enum = BlockType.土壤;
                                break;
                            }
                            case BlockType.岩石:
                            {

                                break;
                            }
                            case BlockType.水面:
                            {
                                if (block.temperature < 0)
                                    type_enum = BlockType.冰面;
                                else if (block.water < 水域临界)
                                    type_enum = BlockType.土壤;
                                break;
                            }
                            case BlockType.冰面:
                            {
                                if (block.temperature > 0)
                                    type_enum = BlockType.水面;
                                break;
                            }
                        }
                        var new_type = (int) type_enum;
                        if (block.type != new_type)
                        {
                            block.type = new_type;
                            block.state_time = 0;
                        }
                        
                    }
                    _blocks2[x][y] = block;
                }

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var block = blocks[x][y];
                var config = SceneManager.block_config[block.type];
                
                SceneBlock block1, block2, block3, block4;
                int low_num = 0;
                double sum_temperature_change = 0;
                if (x + 1 < width)
                {
                    block1 = blocks[x + 1][y];
                    low_num += block.z > block1.z ? 1 : 0;
                    var sub = block.temperature - block1.temperature;
                    if (Math.Abs(block.temperature - block1.temperature) > 1)
                    {
                        sum_temperature_change -=
                            sub * Math.Min(SceneManager.block_config[block1.type].导热率, config.导热率) * dt;
                    }
                }
                else
                {
                    block1.z = Single.MaxValue;
                }

                if (y + 1 < height)
                {
                    block2 = blocks[x][y + 1];
                    low_num += block.z > block2.z ? 1 : 0;
                    var sub = block.temperature - block2.temperature;
                    if (Math.Abs(block.temperature - block2.temperature) > 1)
                    {
                        sum_temperature_change -=
                            sub * Math.Min(SceneManager.block_config[block2.type].导热率, config.导热率) * dt;
                    }
                }
                else
                {
                    block2.z = Single.MaxValue;
                }

                if (x - 1 >= 0)
                {
                    block3 = blocks[x - 1][y];
                    low_num += block.z > block3.z ? 1 : 0;
                    var sub = block.temperature - block3.temperature;
                    if (Math.Abs(block.temperature - block3.temperature) > 1)
                    {
                        sum_temperature_change -=
                            sub * Math.Min(SceneManager.block_config[block3.type].导热率, config.导热率) * dt;
                    }
                }
                else
                {
                    block3.z = Single.MaxValue;
                }

                if (y - 1 >= 0)
                {
                    block4 = blocks[x][y - 1];
                    low_num += block.z > block4.z ? 1 : 0;
                    var sub = block.temperature - block4.temperature;
                    if (Math.Abs(block.temperature - block4.temperature) > 1)
                    {
                        sum_temperature_change -=
                            sub * Math.Min(SceneManager.block_config[block4.type].导热率, config.导热率) * dt;
                    }
                }
                else
                {
                    block4.z = Single.MaxValue;
                }

                ref var target_block = ref _blocks2[x][y];
                double flow_water = block.water - 水流动临界;
                if (low_num > 0 && flow_water > 0)
                {
                    if (flow_water > 水的最大流速 * low_num)
                    {
                        flow_water = 水的最大流速;
                        target_block.water -= 水的最大流速 * low_num;
                    }
                    else
                    {
                        target_block.water -= flow_water;
                        flow_water /= low_num;
                    }
                }
                
                if (x + 1 < width)
                {
                    ref var b = ref _blocks2[x + 1][y];
                    if (b.z < block.z)
                        b.water += flow_water;
                }
                if (y + 1 < height)
                {
                    ref var b = ref _blocks2[x][y + 1];
                    if (b.z < block.z)
                        b.water += flow_water;
                }
                if (x - 1 >= 0)
                {
                    ref var b = ref _blocks2[x - 1][y];
                    if (b.z < block.z)
                        b.water += flow_water;
                }
                if (y - 1 >= 0)
                {
                    ref var b = ref _blocks2[x][y - 1];
                    if (b.z < block.z)
                        b.water += flow_water;
                }
                
                target_block.temperature += sum_temperature_change;
            }

            (_blocks1, _blocks2) = (_blocks2, _blocks1);
        }

        enum BlockType
        {
            荒漠 = 0,
            土壤 = 1,
            草地 = 2,
            枯草 = 3,
            雪地 = 4,
            岩石 = 5,
            水面 = 6,
            冰面 = 7,
        }

        private static readonly double 每点水带走热量 = 0.01f;
        private static readonly double 光照强度比例 = 0.0001f;
        private static readonly double 蒸发率比例 = 0.0001f;
        private static readonly int 荒漠临界 = 10000;
        private static readonly int 水流动临界 = 180000;
        private static readonly int 水域临界 = 200000;
        private static readonly double 长草时间 = 200;
        private static readonly double 草的最高温度 = 50;
        private static readonly Vector2Int 草的生长温度 = new Vector2Int(2, 40);
        private static readonly int 枯草燃点 = 183;
        private static readonly double 枯草腐烂时间 = 20;
        private static readonly double 水的最大流速 = 1000;
    }

}