using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    /// <summary>
    /// Temporarily patches m_color of a NetInfo to the color defined in the skin of the node/segment
    /// </summary>
    public static class ColorPatcher
    {
        public static Color GetSegmentColor(NetAI netAI, ushort segmentID, ref global::NetSegment data, InfoManager.InfoMode infoMode)
        {
            bool applied = Apply(netAI.m_info, NetworkSkinManager.SegmentSkins[segmentID], out var patcherState);
            var segmentColor = netAI.GetColor(segmentID, ref data, infoMode);
            if(applied) Revert(netAI.m_info, patcherState);
            return segmentColor;
        }

        public static Color GetNodeColor(NetAI netAI, ushort nodeID, ref global::NetNode data, InfoManager.InfoMode infoMode)
        {
            var applied = Apply(netAI.m_info, NetworkSkinManager.NodeSkins[nodeID], out var patcherState);
            var segmentColor = netAI.GetColor(nodeID, ref data, infoMode);
            if(applied) Revert(netAI.m_info, patcherState);
            return segmentColor;
        }

        public static bool Apply(NetInfo info, NetworkSkin skin, out Color state)
        {
            if (info == null || skin == null)
            {
                state = default;
                return false;
            }

            state = info.m_color;
            info.m_color = skin.m_color;
            return true;
        }

        public static void Revert(NetInfo info, Color state)
        {
            info.m_color = state;
        }
    }
}
