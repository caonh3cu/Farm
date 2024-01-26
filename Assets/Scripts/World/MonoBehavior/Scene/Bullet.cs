using System;
using UnityEngine;

namespace CAT
{
    public class Bullet : MonoBehaviour
    {
        public float duratation = 3f;
        public int damage = 10;
        public float force = 10;
        public float radius = 1.3f;

        public GameObject boom_particle;
        public float scale = 0.3f;
        public LayerMask damage_mask;
        public LayerMask force_mask;
        public Entity owner;

        private float start_time;
        private bool _hurted;
        private Rigidbody2D _rigidbody;
        
        private void OnEnable()
        {
            _hurted = false;
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            start_time = Time.time;
        }

        private void Update()
        {
            if (Time.time - start_time > duratation)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!_hurted)
            {
                Vector2 position = transform.position;
                _hurted = true;
                var hits = Physics2D.CircleCastAll(position, radius, _rigidbody.velocity, 0, damage_mask);
                foreach (var hit in hits)
                {
                    if (hit.collider.CompareTag("MainPlayer"))
                    {
                        var player = hit.collider.GetComponent<MainPlayer>();
                        player.Hurt(owner, damage);
                    }
                    else if (hit.collider.CompareTag("Monster"))
                    {
                        var monster = hit.collider.GetComponent<Monster>();
                        monster.Hurt(owner, damage);
                    }
                }
                hits = Physics2D.CircleCastAll(position, radius, _rigidbody.velocity, 0, force_mask);
                foreach (var hit in hits)
                {
                    if(hit.rigidbody!=null)
                        hit.rigidbody.AddForce((hit.point-position).normalized * force);
                }

                GameObject.Instantiate(boom_particle, position, Quaternion.identity).transform.localScale =
                    Vector3.one * scale;
                Destroy(gameObject);
            }
        }
    }
}