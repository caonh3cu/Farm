using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class ManagerBase : MonoBehaviour
    {
        protected static GameObject game_manager_object;
        private void OnEnable()
        {
            game_manager_object = gameObject;
        }
    }

}