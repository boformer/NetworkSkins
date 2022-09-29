﻿using HarmonyLib;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches._NetNode
{
    /// <summary>
    /// CalculateNode is called after a node was created or when a segment connected to the node was added, removed or updated.
    /// The method decides from which segment it is inheriting its NetInfo (for roads, it's the widest road).
    /// </summary>
    [HarmonyPatch(typeof(global::NetNode), "CalculateNode")]
    public static class NetNodeCalculateNodePatch
    {
        public static void Prefix(ref global::NetNode __instance, ushort nodeID)
        {
            if (__instance.m_flags != 0)
            {
                var previousSkin = NetworkSkinManager.NodeSkins[nodeID];

                NetworkSkin skinWithHighestPrio = null;
                var netManager = global::NetManager.instance;
                float currentPrio = -1E+07f;
                float currentBuildIndex = 0;
                for (int i = 0; i < 8; i++)
                {
                    ushort segment = __instance.GetSegment(i);
                    if (segment != 0)
                    {
                        NetInfo info = netManager.m_segments.m_buffer[segment].Info;
                        float nodeInfoPriority = info.m_netAI.GetNodeInfoPriority(segment, ref netManager.m_segments.m_buffer[segment]);
                        var buildIndex = netManager.m_segments.m_buffer[segment].m_buildIndex;
                        if (nodeInfoPriority > currentPrio || nodeInfoPriority == currentPrio && buildIndex > currentBuildIndex)
                        {
                            skinWithHighestPrio = NetworkSkinManager.SegmentSkins[segment];
                            currentPrio = nodeInfoPriority;
                            currentBuildIndex = netManager.m_segments.m_buffer[segment].m_buildIndex;
                        }
                    }
                }

                if (previousSkin != skinWithHighestPrio)
                {
                    NetworkSkinManager.instance.UpdateNodeSkin(nodeID, skinWithHighestPrio);
                }
            }
        }
    }
}
