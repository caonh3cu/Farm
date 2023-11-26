using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CAT
{
    [ExecuteInEditMode]
    public class BlockEditor : MonoBehaviour
    {
        public SceneBlocksOriginalState state;
        private Scene _scene;
        public List<List<BlockMono>> block_monos = new List<List<BlockMono>>();
        public GameObject prefab;
        public float speed = 1;
        public double water = 0;
        public float time = 0;
        public float day = 0;
        public float hot = 100;
        private void OnEnable()
        {
            Fresh();
        }

        private void OnValidate()
        {
            Fresh();
        }

        private void Fresh()
        {
            if (state == null || prefab == null) return;
            if (_scene == null || _scene.width != state.origin_blocks.Length ||
                _scene.height != state.origin_blocks[0].row.Length)
            {
                _scene = new Scene();
                _scene.DoCreate(state);
                var trans = transform;
                trans.localPosition=Vector3.zero;
                trans.localRotation = Quaternion.identity;
                trans.localScale = Vector3.one;
                while (block_monos.Count <= _scene.width)
                    block_monos.Add(new List<BlockMono>());
                for (int i = 0; i < _scene.width; i++)
                {
                    while (block_monos[i].Count <= _scene.height)
                    {
                        block_monos[i].Add(null);
                    }
                    for (int j = 0; j < _scene.height; j++)
                    {
                        if (block_monos[i][j] == null)
                        {
                            var obj = GameObject.Find($"[{i}, {j}]");
                            if (obj == null)
                            {
                                obj = GameObject.Instantiate(prefab, transform);
                                obj.name = $"[{i}, {j}]";
                                obj.transform.SetParent(trans);
                                obj.transform.localPosition = new Vector3(i + 0.5f, 0, j + 0.5f);
                            }
                            block_monos[i][j] = obj.GetComponent<BlockMono>();
                        }
                        block_monos[i][j].Set(_scene.blocks[i][j]);
                    }
                }
            }
        }

        private void Update()
        {
            if (_scene == null) return;
            if (Application.isPlaying)
            {
                _scene.blocks[5][5].temperature = hot;
                if (speed > 3000)
                {
                    _scene.UpdateBlocks(Time.deltaTime * speed, _scene.light_intencity * 0.25f);
                    day += (Time.deltaTime * speed) / (3600 * 24);
                }
                else
                {
                    time += Time.deltaTime*speed / (3600 * 24);
                    if (time > 1)
                    {
                        time -= 1;
                        day += 1;
                    }
                    if(time<0.5f)
                        _scene.UpdateBlocks(Time.deltaTime*speed, Mathf.Abs(time-0.25f) * 4f * _scene.light_intencity);
                    else
                        _scene.UpdateBlocks(Time.deltaTime*speed, 0);
                }
                water = _scene.water;
                for (int i = 0; i < _scene.width; i++)
                {
                    for (int j = 0; j < _scene.height; j++)
                    { 
                        block_monos[i][j].Set(_scene.blocks[i][j]);
                    }
                }
            }
        }
    }

}