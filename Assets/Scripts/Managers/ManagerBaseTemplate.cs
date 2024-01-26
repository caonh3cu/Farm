
using System;
using UnityEngine;

namespace CAT
{
    public class ManagerBaseTemplate<T> : MonoBehaviour where T:ManagerBaseTemplate<T>
    {
        private static T _instance;

        public static bool initialized => _instance != null;
        public static T instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this as T;
        }
    }
}