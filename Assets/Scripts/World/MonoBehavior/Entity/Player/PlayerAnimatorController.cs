using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CAT
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        public Animator _animator;

        public void SetTrigger(string _trigger_name)
        {
            _animator.SetTrigger(_trigger_name);
        }

        public float speed
        {
            get => _animator.speed;
            set => _animator.speed = value;
        }

        public void SetBool(string _bool_name, bool _bool_value)
        {
            _animator.SetBool(_bool_name, _bool_value);
        }
    }
}