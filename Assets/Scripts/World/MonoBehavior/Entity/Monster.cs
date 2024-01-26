using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class Monster : Entity
    {
        public int health = 20;
        public int max_health = 20;
        public Animator animator;
        public HealthBar health_bar;
        public Rigidbody2D monster_rigidbody;
        public Collider2D monster_collider;
        public bool is_die => health <= 0;

        public void Hurt(Entity source, int _damage)
        {
            if (is_die) return;
            health -= _damage;
            if (health <= 0)
            {
                health = 0;
                monster_rigidbody.simulated = false;
                monster_collider.enabled = false;
                PlayDie();
            }
            else
            {
                PlayHurt();
            }
            health_bar.target_rate = (float)(health) / max_health;
        }
        public void Rebirth()
        {
            ReturnToIdle();
            monster_rigidbody.simulated = true;
            monster_collider.enabled = true;
            health = max_health;
            health_bar.target_rate = (float)(health) / max_health;
        }

        void PlayHurt()
        {
            animator.SetTrigger("hurt");
        }
        void PlayDie()
        {
            animator.SetTrigger("die");
        }
        
        void ReturnToIdle()
        {
            animator.SetTrigger("idle");
        }
    }

}