using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NetworkSkins.Net
{
    public static class TreeUtils
    {
        public static List<TreeInfo> GetAvailableTrees()
        {
            var trees = new List<TreeInfo>();

            var prefabCount = PrefabCollection<TreeInfo>.LoadedCount();
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                trees.Add(PrefabCollection<TreeInfo>.GetLoaded(prefabIndex));
            }

            trees.Sort((a, b) => string.Compare(a.GetUncheckedLocalizedTitle(), b.GetUncheckedLocalizedTitle(), StringComparison.Ordinal));

            return trees;
        }

        [CanBeNull]
        public static TreeInfo GetDefaultTree(NetInfo prefab, LanePosition position)
        {
            return NetUtils.GetMatchingLaneProp(prefab, laneProp => laneProp.m_finalTree != null, position)?.m_finalTree;
        }

        public static float GetDefaultRepeatDistance(NetInfo prefab, LanePosition position)
        {
            return NetUtils.GetMatchingLaneProp(prefab, laneProp => laneProp.m_finalTree != null, position)?.m_repeatDistance ?? 20f;
        }
    }
}
