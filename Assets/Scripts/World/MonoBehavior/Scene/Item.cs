using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;

namespace CAT
{
    public class Item : MonoBehaviour
    {
        public Player owner;
        public MainPlayer main_player=> owner as MainPlayer;
        public bool is_owner_main_player => owner && owner.GetType() == typeof(MainPlayer);
        public string item_name;
        public string item_description;
        public bool is_unique;
        public SpriteRenderer sprite_renderer;
        public Rigidbody2D item_rigidbody;
        public Collider2D item_collider;
        public Collider2D item_trigger;

        public virtual void OnPick(MainPlayer _player)
        {
            owner = _player;
            var transform = this.transform;
            transform.SetParent(_player.transform);
            transform.localPosition = Vector3.zero;
            sprite_renderer.enabled = false;
            item_rigidbody.simulated = false;
            item_collider.enabled = false;
            item_trigger.enabled = false;
        }

        public virtual void OnDrop()
        {
            var transform = this.transform;
            transform.SetParent(null);
            transform.localPosition = owner.transform.position + new Vector3(0, 1, 0);
            owner = null;
            sprite_renderer.enabled = true;
            item_rigidbody.simulated = true;
            item_collider.enabled = true;
            item_trigger.enabled = true;
        }
    }
}