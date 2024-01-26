using System;
using Cinemachine;
using UnityEngine;

namespace CAT
{
    public class ChangeCharacter : MonoBehaviour
    {
        public enum CharacterType
        {
            DOG,
            CAT,
        }

        public CharacterType character_type = CharacterType.DOG;
        public MainPlayer dog;
        public MainPlayer cat;
        public GameObject effect;
        public Vector3 effect_offset = new Vector3(0, 1, 0);
        public CinemachineVirtualCamera virtual_camera;

        private void Start()
        {
            dog.gameObject.SetActive(true);
            cat.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!G.player_control) return;
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (character_type == CharacterType.DOG)
                {
                    character_type = CharacterType.CAT;
                    dog.gameObject.SetActive(false);
                    cat.gameObject.SetActive(true);
                    cat.transform.position = dog.transform.position;
                    GameObject.Instantiate(effect, dog.transform.position + effect_offset, Quaternion.identity);
                    G.instance._main_player = cat;
                    virtual_camera.Follow = cat.transform;
                }
                else if (character_type == CharacterType.CAT)
                {
                    character_type = CharacterType.DOG;
                    dog.gameObject.SetActive(true);
                    cat.gameObject.SetActive(false);
                    dog.transform.position = cat.transform.position;
                    var e = GameObject.Instantiate(effect, cat.transform.position + effect_offset, Quaternion.identity);
                    G.instance._main_player = dog;
                    virtual_camera.Follow = dog.transform;
                }
            }
        }
    }
}