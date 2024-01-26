using System;
using UnityEngine;

namespace CAT
{
    public class Weapon : Item
    {
        private Vector2 attack_click_position;
        public GameObject bullet_prefab;
        public float bullet_force = 1;
        public float 前摇 = 0.4f;
        public float 后摇 = 0.1f;

        public virtual void Attack(Vector2 _mouse_position)
        {
            attack_click_position = (Vector2) G.main_camera.ScreenToWorldPoint(_mouse_position) -
                                    ((Vector2) main_player.transform.position + main_player.player_collider.offset);
            main_player.StartSkill(attack_click_position.x > 0, 前摇 + 后摇, 前摇, 后摇, DoAttack);
            
        }

        public virtual void DoAttack(Transform _attack_start_transform)
        {
            var start_position = (Vector2)(_attack_start_transform.position);
            var direction = (attack_click_position +
                    ((Vector2) main_player.transform.position + main_player.player_collider.offset) - start_position)
                .normalized;
            var bullet = GameObject.Instantiate(bullet_prefab);
            bullet.transform.position = start_position;
            bullet.transform.forward = _attack_start_transform.forward;
            bullet.GetComponent<Rigidbody2D>().AddForce(direction * bullet_force);
            bullet.GetComponent<Bullet>().owner = main_player;
        }
    }
}