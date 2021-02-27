using HarmonyLib;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.MetroTrackBridgeAI
{
    [HarmonyPatch(typeof(global::MetroTrackBridgeAI), "GetNodeBuilding")]
    public static class MetroTrackBridgeAiGetNodeBuildingPatch {
        public static void Prefix(ref global::MetroTrackBridgeAI __instance, ushort nodeID, out MetroTrackBridgePillarPatcherState? __state)
        {
            var skin = NetworkSkinManager.NodeSkins[nodeID];
            if (skin != null)
            {
                __state = new MetroTrackBridgePillarPatcherState(__instance);

                __instance.m_bridgePillarInfo = skin.m_bridgePillarInfo;
                __instance.m_middlePillarInfo = skin.m_middlePillarInfo;
            }
            else
            {
                __state = null;
            }
        }

        public static void Postfix(ref global::MetroTrackBridgeAI __instance, ref MetroTrackBridgePillarPatcherState? __state)
        {
            __state?.Restore(__instance);
        }
    }

    public struct MetroTrackBridgePillarPatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo MiddlePillarInfo;

        public MetroTrackBridgePillarPatcherState(global::MetroTrackBridgeAI netAi)
        {
            BridgePillarInfo = netAi.m_bridgePillarInfo;
            MiddlePillarInfo = netAi.m_middlePillarInfo;
        }

        public void Restore(global::MetroTrackBridgeAI netAi)
        {
            netAi.m_bridgePillarInfo = BridgePillarInfo;
            netAi.m_middlePillarInfo = MiddlePillarInfo;
        }
    }
}
