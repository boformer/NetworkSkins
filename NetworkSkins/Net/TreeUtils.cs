using JetBrains.Annotations;

namespace NetworkSkins.Net
{
    public static class TreeUtils
    {
        [CanBeNull]
        public static TreeInfo GetDefaultTree(NetInfo prefab, LanePosition position)
        {
            return NetUtil.GetMatchingLaneProp(prefab, laneProp => laneProp.m_finalTree != null, position)?.m_finalTree;
        }

        public static float GetDefaultRepeatDistance(NetInfo prefab, LanePosition position)
        {
            return NetUtil.GetMatchingLaneProp(prefab, laneProp => laneProp.m_finalTree != null, position)?.m_repeatDistance ?? 20f;
        }
    }
}
