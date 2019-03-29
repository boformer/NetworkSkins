using Harmony;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    /// <summary>
    /// CalculateNode is called after a node was created or when a segment connected to the node was added, removed or updated.
    /// The method decides from which segment it is inheriting its NetInfo (for roads, ít's the widest road).
    /// </summary>
    [HarmonyPatch(typeof(NetNode), "CalculateNode")]
    public class NetNodeCalculateNodePatch
    {
        public static void Postfix(ref NetNode __instance, ushort nodeID)
        {
            if (__instance.m_flags != 0)
            {
                var previousSkin = NetworkSkinManager.NodeSkins[nodeID];

                NetworkSkin skinWithHighestPrio = null;
                var netManager = NetManager.instance;
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

                Debug.Log($"CalculateNode on {nodeID}. Settin skin to {skinWithHighestPrio}");
                NetworkSkinManager.NodeSkins[nodeID] = skinWithHighestPrio;

                // Make sure that the color map is updated when a skin with a different color is applied!
                if (previousSkin?.m_color != skinWithHighestPrio?.m_color)
                {
                    netManager.UpdateNodeColors(nodeID);
                }
            }
        }
    }
}
