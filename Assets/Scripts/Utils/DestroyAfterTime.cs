using System;
using UnityEngine;

namespace CAT
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float time;
        private void Start()
        {
            Invoke(nameof(DestroyThis), time);
        }

        public void DestroyThis()
        {
            Destroy(gameObject);
        }
    }
}