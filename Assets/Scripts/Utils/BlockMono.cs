using TMPro;
using UnityEngine;

namespace CAT
{
    public class BlockMono : MonoBehaviour
    {
        public TextMeshPro text;

        public void Set(SceneBlock block)
        {
            text.text =
                $"type:   {block.type}\ntemp:  {block.temperature.ToString("0.00")}°C\nwater: {block.water.ToString("0.00")}\nz:         {block.z.ToString("0.00")}";
        }
    }
}