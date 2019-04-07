using System.Collections.Generic;

namespace NetworkSkins
{
    public static class DictionaryExtensions
    {
        public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> self, Dictionary<TKey, TValue> merge)
        {
            foreach (var item in merge)
            {
                self[item.Key] = item.Value;
            }
        }
    }
}
