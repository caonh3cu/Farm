using System;
using UnityEngine;

namespace CAT
{
    public class HealthBar : MonoBehaviour
    {
        public Transform bar;
        public float current_rate = 1;
        public float change_speed = 0.3f;
        public float target_rate = 1;

        private void Update()
        {
            float step = change_speed * Time.deltaTime;
            if (target_rate > current_rate)
            {
                if (current_rate + step < target_rate)
                    current_rate += step;
                else
                    current_rate = target_rate;
            }
            else
            {
                if (current_rate - step > target_rate)
                    current_rate -= step;
                else
                    current_rate = target_rate;
            }
            var scale = bar.localScale;
            var pos = bar.localPosition;
            pos.x = -0.962f * (1 - current_rate);
            scale.x = 0.97f * current_rate;
            bar.localScale = scale;
            bar.localPosition = pos;
        }
    }
}