using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class PlayerController : MonoBehaviour
    {
        private static float _NORMAL_SPEED = 4f;
        private static float _NORMAL_ATTACK_DURATION= 0.5f;
        private static float _NORMAL_HURT_DURATION= 0.3f;
        public float speed = 4f;
        
        [Range(0, 10)]
        public float attack_duration = 3f;
        [Range(0, 10)]
        public float hit_time = 2f;
        [Range(0, 10)]
        public float attack_end_time = 0.4f;

        [Range(0, 1)] public float pre_input_rate = 0.1f;
        public bool last_input_first;
        [Range(0, 1)]
        public float attack_post_rate = 0.0875f;

        public float hurt_stun_time = 0.5f;

            
        [SerializeField] private Transform _animate_root;
        private Animator _animator;

        private bool _attack_trigger;
        private float _attack_button_press_time;
        private bool _attack_succeed;
        private bool _attack_ended;

        private float _hurt_start_time;
        private bool _is_hurting;

        private float _param1, _param2, _param3;
        
        private Rigidbody2D _rigidbody;

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetTrigger("idle");
            _rigidbody = GetComponent<Rigidbody2D>();
            _attack_button_press_time = - attack_duration;
        }

        private void Update()
        {
            _rigidbody.velocity = Vector2.zero;
            var hurt_passed = Time.time - _hurt_start_time;
            if (hurt_passed < hurt_stun_time)
            {
                return;
            }

            if (_is_hurting)
            {
                _is_hurting = false;
                _animator.speed = 1;
            }
            var attack_passed = Time.time - _attack_button_press_time;
            if (attack_passed < attack_duration)
            {
                if (!_attack_succeed && (attack_passed > hit_time))
                {
                    _attack_succeed = true;
                    DoAttack();
                }
                if(!_attack_ended && attack_passed > attack_end_time + hit_time)
                    EndAttack();
                if (attack_passed > attack_end_time + attack_post_rate*attack_duration + hit_time)
                    _attack_button_press_time = 0;
                return;
            }

            if (_attack_succeed)
            {
                _attack_succeed = false;
            }
            _attack_ended = false;
            
            if (_attack_trigger)
            {
                _attack_trigger = false;
                attack_duration = _param1;
                hit_time = _param2;
                attack_end_time = _param3;
                is_run = false;
                PlayAttack();
                _attack_button_press_time = Time.time;
                _animator.speed = _NORMAL_ATTACK_DURATION / attack_duration;
                return;
            }
            var move_vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (move_vector.x != 0 || move_vector.y != 0)
            {
                if(move_vector.x>0)
                    _animate_root.localRotation = Quaternion.Euler(0,  0, 0);
                else
                    _animate_root.localRotation = Quaternion.Euler(0,  180, 0);
                is_run = true;
            }
            else
            {
                is_run = false;
            }
            _rigidbody.velocity = move_vector * speed;
            if (Input.GetKeyDown("k"))
            {
                Attack();
            }
            if (Input.GetKeyDown("h"))
            {
                Hurt();
            }
            if (Input.GetKeyDown("o"))
            {
                BigShoot();
            }
            if (Input.GetKeyDown("p"))
            {
                FieldMagic();
            }
        }

        public void Attack() => StartSkill(1, 0.4f, 0.1f);
        public void FieldMagic() => StartSkill(1f / 8, 1.8f, 0.75f);
        public void BigShoot() => StartSkill(1f / 8, 3.5f, 0.5f);
        public void StartSkill(float _speed,float 前摇, float 后摇)
        {
            if (Time.time - _attack_button_press_time < attack_duration*pre_input_rate) return;
            if (!last_input_first && _attack_trigger) return;
            _param1 = Mathf.Max(前摇 + 后摇, _NORMAL_ATTACK_DURATION / _speed);
            if (后摇 < _param1 * attack_post_rate)
            {
                后摇 = _param1 * attack_post_rate;
                Debug.LogError("配错了！");
            }
            _param2 = 前摇;
            _param3 = 后摇 - _param1 * attack_post_rate;
            _attack_trigger = true;
        }
        

        public void Hurt()
        {
            is_run = false;
            _attack_button_press_time = 0;
            _attack_succeed = false;
            _animator.speed = _NORMAL_HURT_DURATION / hurt_stun_time;
            _hurt_start_time = Time.time;
            _is_hurting = true;
            PlayHurt();
        }

        void EndAttack()
        {
            // _animator.speed = Mathf.Lerp(_animator.speed, 1, 0.3f);
            _attack_ended = true;
            if (!_is_hurting)
            {
                ReturnToIdle();
            }
        }

        void DoAttack()
        {
            Debug.Log("攻击");
        }

        private bool _is_run = false;
        bool is_run
        {
            get { return is_run; }
            set
            {
                if (value != _is_run)
                {
                    if (value)
                        _animator.speed = speed / _NORMAL_SPEED;
                    else
                        _animator.speed = 1;
                    _animator.SetBool("isRun", value);
                    _is_run = value;
                }
            }
        }
        
        void PlayAttack()
        {
            _animator.SetTrigger("attack");
        }
        void PlayHurt()
        {
            _animator.SetTrigger("hurt");
        }
        void PlayDie()
        {
            _animator.SetTrigger("die");
        }
        void ReturnToIdle()
        {
            _animator.SetTrigger("idle");
        }
    }
}