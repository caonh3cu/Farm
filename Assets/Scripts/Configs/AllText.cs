using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    [CreateAssetMenu(fileName="texts", menuName="配置文件/文本配置")]
    public class AllText : ScriptableObject
    {
        public List<string> texts = new List<string>();
    }
}
