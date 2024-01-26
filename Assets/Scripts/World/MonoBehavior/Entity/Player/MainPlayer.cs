using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CAT
{
    public class MainPlayer : Player
    {
        
        public float _NORMAL_SPEED = 4f;
        private static float _NORMAL_ATTACK_DURATION= 0.5f;
        private static float _NORMAL_HURT_DURATION= 0.3f;
        public float walk_speed = 4;
        public float run_speed = 8;
        public float max_speed = 10f;

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

        public float jump_speed = 150;
        public float jump_start_duration = 0.3f;
        public float jump_hold_duration = 0.3f;
        public float jump_hold_min_duration = 0.1f;

        public Transform attack_start_transform;
        public Vector2 attack_start_offset;

        public bool attacking => _attack_button_press_time > 0;
        public MainPlayerPackAndEquip pack_and_equip;

        public int max_health = 100;
        public int health = 100;
        public bool is_die => health <= 0;
        public HealthBar health_bar;
        public CapsuleCollider2D player_collider;
        public LayerMask physics2d_layer_mask;
            
        [SerializeField] private Transform _animate_root;
        private PlayerAnimatorController _animator;
        
        private float _move_speed = 4f;

        private bool _attack_trigger;
        private float _attack_button_press_time;
        private bool _attack_succeed;
        private bool _attack_ended;

        private float _hurt_start_time;
        private bool _is_hurting;

        private float _jump_start_time;
        private bool _jump_hold;
        private bool _space_up;

        private float _param1, _param2, _param3;
        private UnityAction<Transform> _attack_callback;
        
        private Rigidbody2D _rigidbody;
        private int _scene_layer;

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<PlayerAnimatorController>();
            _animator.SetTrigger("idle");
            _rigidbody = GetComponent<Rigidbody2D>();
            _attack_button_press_time = - attack_duration;
            _scene_layer = LayerMask.NameToLayer("Default");
            attack_start_transform.localPosition = attack_start_offset;
        }

        private void Update()
        {
            if (!G.player_control) return;
            if (is_die)
            {
                if(Input.GetKeyDown(KeyCode.R))
                    Rebirth();
                return;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                pack_and_equip.PickNearestItem();
            }
            
            Vector2 position = transform.position;
            Vector2 velocity = _rigidbody.velocity;
            if (velocity.y > max_speed)
                velocity.y = max_speed;
            else if (velocity.y < -max_speed)
                velocity.y = -max_speed;
            _rigidbody.velocity = velocity;
            
            // 受击最优先
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
            
            // shift加速
            _move_speed = Input.GetKey(KeyCode.LeftShift) ? run_speed : walk_speed;

            // 判断跳跃过程
            if (Input.GetKeyUp(KeyCode.Space))
                _space_up = true;
            if ((_jump_hold || Time.time - _jump_start_time + 0.14f < jump_start_duration) && IsBelowFloor(position))
            {
                _jump_hold = false;
                _jump_start_time = Time.time - jump_start_duration + 0.14f;
            }
            else if (_jump_hold && (Time.time - _jump_start_time > jump_hold_duration || (Time.time - _jump_start_time > jump_hold_min_duration && _space_up)))
            {
                _jump_hold = false;
                _jump_start_time = Time.time;
            }
            if (_jump_hold)
            {
                float t = (jump_hold_min_duration - (Time.time - _jump_start_time)) / jump_hold_min_duration * 0.3f + 0.7f;
                _rigidbody.AddForce(new Vector2(0, jump_speed * math.sqrt(t)));
            }
            else if (Time.time - _jump_start_time < jump_start_duration)
            {
                float t = (jump_start_duration - (Time.time - _jump_start_time)) / jump_start_duration * 0.7f;
                _rigidbody.AddForce(new Vector2(0,
                    jump_speed * math.sqrt(t)));
            }
            else
            {
                // 判断浮空
                if (IsOnGround(position) )
                    is_jumping = false;
                else if(IsOnGround(position + new Vector2(player_collider.size.x * 0.5f, 0)))
                    is_jumping = false;
                else if (IsOnGround(position - new Vector2(player_collider.size.x * 0.5f, 0)))
                    is_jumping = false;
                else
                    is_jumping = true;
            }
            
            
            // 然后是跳跃
            if (is_jumping)
            {            
                var jump_move_speed = Input.GetAxisRaw("Horizontal");
                if (jump_move_speed != 0)
                {
                    SetForward(jump_move_speed > 0);
                }
                if (IsOnWall(position, jump_move_speed < 0))
                    jump_move_speed = 0;
                velocity.x = jump_move_speed * this._move_speed;
                _rigidbody.velocity = velocity;
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
                return;
            }
            
            // 速度设置为0
            _rigidbody.velocity = velocity = Vector2.zero;

            if (!attacking && Input.GetMouseButtonDown(0))
            {
                if(pack_and_equip.current_weapon!=null)
                    pack_and_equip.current_weapon.Attack(Input.mousePosition);
            }
            // 然后是攻击动作
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
                    _attack_button_press_time = -attack_duration;
                return;
            }
            _attack_button_press_time = -attack_duration;

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
            
            // 最后是移动
            // var move_vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            var move_vector = Input.GetAxisRaw("Horizontal");
            if (move_vector != 0)
            {
                SetForward(move_vector > 0);
                
                is_run = true;
            }
            else
            {
                is_run = false;
            }
            velocity.x = move_vector * this._move_speed;
            _rigidbody.velocity = velocity;
        }

        public void Attack(bool _is_left, UnityAction<Transform> _callback) =>
            StartSkill(_is_left, 0.5f, 0.4f, 0.1f, _callback);
        public void FieldMagic(bool _is_left, UnityAction<Transform> _callback) =>
            StartSkill(_is_left, 4, 1.8f, 0.75f, _callback);
        public void BigShoot(bool _is_left, UnityAction<Transform> _callback) =>
            StartSkill(_is_left, 4, 3.5f, 0.5f, _callback);
        public void StartSkill(bool _is_left, float 总时间,float 前摇, float 后摇, UnityAction<Transform> _callback)
        {
            if (Time.time - _attack_button_press_time < attack_duration*pre_input_rate) return;
            if (!last_input_first && _attack_trigger) return;
            SetForward(_is_left);
            _param1 = Mathf.Max(前摇 + 后摇, 总时间);
            if (后摇 < _param1 * attack_post_rate)
            {
                后摇 = _param1 * attack_post_rate;
                Debug.LogError("配错了！");
            }
            _param2 = 前摇;
            _param3 = 后摇 - _param1 * attack_post_rate;
            _attack_callback = _callback;
            _attack_trigger = true;
        }
        

        public void Hurt(Entity source, int _damage)
        {
            if (is_die) return;
            is_run = false;
            _attack_button_press_time = -attack_duration;
            _attack_succeed = false;
            health -= _damage;
            if (health <= 0)
            {
                health = 0;
                PlayDie();
            }
            else
            {
                _animator.speed = _NORMAL_HURT_DURATION / hurt_stun_time;
                _hurt_start_time = Time.time;
                _rigidbody.velocity = Vector2.zero;
                _is_hurting = true;
                PlayHurt();
            }
            health_bar.target_rate = (float)(health) / max_health;
        }

        public void Rebirth()
        {
            ReturnToIdle();
            health = max_health;
            health_bar.target_rate = (float)(health) / max_health;
        }

        public void Jump()
        {
            is_run = false;
            _attack_button_press_time = -attack_duration;
            _attack_succeed = false;
            _jump_hold = true;
            _space_up = false;
            is_jumping = true;
            _jump_start_time = Time.time;
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
            _attack_callback.Invoke(attack_start_transform);
        }

        private bool _is_run = false;
        bool is_run
        {
            get { return _is_run; }
            set
            {
                if (value != _is_run)
                {
                    if (value)
                        _animator.speed = _move_speed / _NORMAL_SPEED;
                    else
                        _animator.speed = 1;
                    _animator.SetBool("isRun", value);
                    _is_run = value;
                }
            }
        }

        private bool _is_jumping;

        private bool is_jumping
        {
            get => _is_jumping;
            set
            {
                if (value != _is_jumping)
                {
                    _is_jumping = value;
                    _animator.SetBool("isJump", value);
                    if (!value)
                    {
                        _jump_start_time = 0;
                        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
                    }
                }

            }
        }

        void SetForward(bool _is_left)
        {
            if (_is_left)
            {
                _animate_root.localRotation = Quaternion.Euler(0,  0, 0);
                if(pack_and_equip.current_weapon)
                    pack_and_equip.current_weapon.sprite_renderer.sortingOrder = 0;
            }
            else
            {
                _animate_root.localRotation = Quaternion.Euler(0,  180, 0);
                if(pack_and_equip.current_weapon)
                    pack_and_equip.current_weapon.sprite_renderer.sortingOrder = 24;
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

        bool IsBelowFloor(Vector2 _position)
        {
            var up_pos = _position + player_collider.offset + new Vector2(0, player_collider.size.y * 0.5f);
            var hits = Physics2D.RaycastAll(up_pos, new Vector2(0, 1), 2f, physics2d_layer_mask);
            foreach (var h in hits)
            {
                if (h.collider != player_collider &&
                    Mathf.Abs(h.point.y - up_pos.y) < 0.05f)
                {
                    return true;
                }
            }

            hits = Physics2D.RaycastAll(up_pos + new Vector2(player_collider.size.x * 0.45f, 0), new Vector2(0, 1), 2f,
                physics2d_layer_mask);
            foreach (var h in hits)
            {
                if (h.collider != player_collider &&
                    Mathf.Abs(h.point.y - up_pos.y) < 0.05f)
                {
                    return true;
                }
            }

            hits = Physics2D.RaycastAll(up_pos + new Vector2(-player_collider.size.x * 0.45f, 0), new Vector2(0, 1), 2f,
                physics2d_layer_mask);
            foreach (var h in hits)
            {
                if (h.collider != player_collider &&
                    Mathf.Abs(h.point.y - up_pos.y) < 0.05f)
                {
                    return true;
                }
            }
            return false;
        }
        bool IsOnGround(Vector2 _position)
        {
            var hits = Physics2D.RaycastAll(_position, new Vector2(0, -1), 2f, physics2d_layer_mask);
            foreach (var h in hits)
            {
                if (h.collider != player_collider && Mathf.Abs(h.point.x - _position.x) < player_collider.size.x * 0.5f &&
                    Mathf.Abs(h.point.y - _position.y) < 0.05f)
                {
                    return true;
                }
            }
            return false;
        }

        bool IsOnWall(Vector2 _position, bool _is_left)
        {
            var p = _position + new Vector2(_is_left ? -player_collider.size.x * 0.5f : player_collider.size.x * 0.5f,
                player_collider.size.y * 0.5f);
            var hits = Physics2D.RaycastAll(p, _is_left ? Vector2.left : Vector2.right,1, physics2d_layer_mask);
            foreach (var h in hits)
            {
                if (h.collider != player_collider && Mathf.Abs(h.point.y - p.y) < player_collider.size.y * 0.5f
                                            && Mathf.Abs(h.point.x - p.x) < 0.05f)
                {
                    return true;
                }
            }

            hits = Physics2D.RaycastAll(p + new Vector2(0, player_collider.size.y * 0.49f),
                _is_left ? Vector2.left : Vector2.right, 1, physics2d_layer_mask);
            foreach (var h in hits)
            {
                if (h.collider != player_collider && Mathf.Abs(h.point.y - p.y) < player_collider.size.y * 0.5f
                                            && Mathf.Abs(h.point.x - p.x) < 0.05f)
                {
                    return true;
                }
            }
            hits = Physics2D.RaycastAll(p - new Vector2(0, player_collider.size.y * 0.49f),
                _is_left ? Vector2.left : Vector2.right, 1, physics2d_layer_mask);
            foreach (var h in hits)
            {
                if (h.collider != player_collider && Mathf.Abs(h.point.y - p.y) < player_collider.size.y * 0.5f
                                            && Mathf.Abs(h.point.x - p.x) < 0.05f)
                {
                    return true;
                }
            }
            return false;
        }
    }
}