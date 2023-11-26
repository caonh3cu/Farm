using System.Collections.Generic;

namespace CAT
{
    public class LanguageManager : ManagerBaseTemplate<LanguageManager>
    {
        public List<AllText> texts = new List<AllText>();
        private static int _now_index;

        public static string GetText(int id, string raw)
        {
            if (instance && _now_index < instance.texts.Count)
            {
                if (id < instance.texts[_now_index].texts.Count)
                {
                    var target = instance.texts[_now_index].texts[id];
                    if(!string.IsNullOrEmpty(target))
                        return target;
                }
            }
            return raw;
        }
    }

}