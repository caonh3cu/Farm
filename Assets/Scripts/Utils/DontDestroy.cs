using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class DontDestroy : MonoBehaviour
    {
        private static GameObject instance;
        // Start is called before the first frame update
        void Start()
        {
            if (instance)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                foreach (var t in GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.SetActive(true);
                }

                instance = gameObject;
            }
        }

    }

}