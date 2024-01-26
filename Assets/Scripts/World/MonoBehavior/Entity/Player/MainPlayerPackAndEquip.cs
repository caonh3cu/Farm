using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CAT
{
    public class MainPlayerPackAndEquip : MonoBehaviour
    {
        [Serializable]
        public class ItemStack
        {
            public Item item;
            public int count;
        }
        public MainPlayer player;
        public Weapon current_weapon;
        public Transform weapon_bone;

        public List<ItemStack> all_items = new List<ItemStack>();

        private HashSet<Item> _nearby_items = new HashSet<Item>();
        private Item _nearest_item;
        

        private void Update()
        {
            if (player.is_die) return;
            var transform = this.transform;
            var position = transform.position;
            var tips = GlobalUIManager.pick_tips;
            if (_nearby_items.Count > 0)
            {
                float nearest_distance = float.MaxValue;
                foreach (var item in _nearby_items)
                {
                    var distance = (item.transform.position - position).sqrMagnitude;
                    if (distance < nearest_distance)
                    {
                        nearest_distance = distance;
                        _nearest_item = item;
                    }
                }

                if (!tips.is_show)
                    tips.Show();
                tips.tips.text = $"F: 拾取\n{_nearest_item.item_name}";
                tips.transform.position = G.main_camera.WorldToScreenPoint(_nearest_item.transform.position);
            }
            else
            {
                _nearest_item = null;
                if(tips.is_show)
                    tips.Hide();
            }
        }

        private void OnTriggerEnter2D(Collider2D _col)
        {
            if (_col.CompareTag("Item"))
            {
                var item = _col.transform.parent.GetComponent<Item>();
                if(!item.owner && !_nearby_items.Contains(item))
                    _nearby_items.Add(item);
            }
        }

        private void OnTriggerExit2D(Collider2D _other)
        {
            if (_other.CompareTag("Item"))
            {
                var item = _other.transform.parent.GetComponent<Item>();
                if (_nearby_items.Contains(item))
                    _nearby_items.Remove(item);
            }
        }

        public void PickNearestItem()
        {
            if (_nearest_item != null)
            {
                _nearby_items.Remove(_nearest_item);
                PickItem(_nearest_item);
                _nearest_item = null;
            }
        }

        public void PickItem(Item _item)
        {
            _item.OnPick(player);
            if (_item as Weapon)
            {
                EquipWeapon(_item as Weapon);
            }
            else if (_item.is_unique)
            {
                all_items.Add(new ItemStack() {count = 1, item = _item});
            }
            else
            {
                if (all_items.Any(a => a.item.item_name == _item.item_name))
                {
                    var first = all_items.First(a => a.item.item_name == _item.item_name);
                    first.count++;
                }
                else
                {
                    all_items.Add(new ItemStack() {count = 1, item = _item});
                }
            }

        }

        public void DropItem()
        {
            
        }

        public void EquipWeapon(Weapon _weapon)
        {
            if(current_weapon)
                DropCurrentWeapon();
            current_weapon = _weapon;
            var weapon_transform = current_weapon.transform;
            weapon_transform.SetParent(weapon_bone);
            weapon_transform.localPosition = Vector3.zero;
            weapon_transform.localRotation = Quaternion.identity;
            var scale = weapon_transform.localScale;
            weapon_transform.localScale = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);
            current_weapon.sprite_renderer.enabled = true;
        }

        public void DropCurrentWeapon()
        {
            current_weapon.OnDrop();
            current_weapon = null;
        }
    }
}