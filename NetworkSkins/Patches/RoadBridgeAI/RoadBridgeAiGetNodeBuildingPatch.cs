using Harmony;
using NetworkSkins.Skins;

// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.RoadBridgeAI
{
    [HarmonyPatch(typeof(global::RoadBridgeAI), "GetNodeBuilding")]
    public static class RoadBridgeAiGetNodeBuildingPatch
    {
        public static void Prefix(ref global::RoadBridgeAI __instance, ushort nodeID, out RoadBridgePillarPatcherState? __state)
        {
            var skin = NetworkSkinManager.NodeSkins[nodeID];
            if (skin != null)
            {
                __state = new RoadBridgePillarPatcherState(__instance);

                __instance.m_bridgePillarInfo = skin.m_bridgePillarInfo;
                __instance.m_middlePillarInfo = skin.m_middlePillarInfo;
            }
            else
            {
                __state = null;
            }
        }

        public static void Postfix(ref global::RoadBridgeAI __instance, ref RoadBridgePillarPatcherState? __state)
        {
            __state?.Restore(__instance);
        }
    }

    public struct RoadBridgePillarPatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo MiddlePillarInfo;

        public RoadBridgePillarPatcherState(global::RoadBridgeAI netAi)
        {
            BridgePillarInfo = netAi.m_bridgePillarInfo;
            MiddlePillarInfo = netAi.m_middlePillarInfo;
        }

        public void Restore(global::RoadBridgeAI netAi)
        {
            netAi.m_bridgePillarInfo = BridgePillarInfo;
            netAi.m_middlePillarInfo = MiddlePillarInfo;
        }
    }
}
