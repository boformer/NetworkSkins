using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    public class ColorPatcher
    {
        private static int _debugCounter = 0;

        public static Color GetSegmentColor(NetAI netAI, ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode)
        {
            var patcherState = Apply(netAI.m_info, NetworkSkinManager.SegmentSkins[segmentID]);
            var segmentColor = netAI.GetColor(segmentID, ref data, infoMode);
            Revert(netAI.m_info, patcherState);
            return segmentColor;
        }

        public static Color GetNodeColor(NetAI netAI, ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode)
        {
            var patcherState = Apply(netAI.m_info, NetworkSkinManager.NodeSkins[nodeID]);
            var segmentColor = netAI.GetColor(nodeID, ref data, infoMode);
            Revert(netAI.m_info, patcherState);
            return segmentColor;
        }

        public static Color? Apply(NetInfo info, NetworkSkin skin)
        {
            if (info == null || skin == null || info.m_color == skin.m_color)
            {
                return null;
            }

            Debug.Log($"Patching colors to skin {skin} {_debugCounter}");
            _debugCounter++;

            var state = info.m_color;

            info.m_color = skin.m_color;

            return state;
        }

        public static void Revert(NetInfo info, Color? state)
        {
            if (info == null || state == null)
            {
                return;
            }

            _debugCounter--;
            Debug.Log($"Reversing colors {_debugCounter}");

            info.m_color = state.Value;
        }
    }
}
