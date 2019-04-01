using Harmony;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.TrainTrackBridgeAI
{
    // TODO removed because it's throwing exce
    [HarmonyPatch(typeof(global::TrainTrackBridgeAI), "GetNodeBuilding")]
    public static class TrainTrackBridgeAiGetNodeBuildingPatch
    {
        public static void Prefix(ref global::TrainTrackBridgeAI __instance, ushort nodeID, out TrainTrackBridgePillarPatcherState? __state)
        {
            var skin = NetworkSkinManager.NodeSkins[nodeID];
            if (skin != null)
            {
                __state = new TrainTrackBridgePillarPatcherState(__instance);

                __instance.m_bridgePillarInfo = skin.m_bridgePillarInfo;
                __instance.m_middlePillarInfo = skin.m_middlePillarInfo;
            }
            else
            {
                __state = null;
            }
        }

        public static void Postfix(ref global::TrainTrackBridgeAI __instance, ref TrainTrackBridgePillarPatcherState? __state)
        {
            __state?.Restore(__instance);
        }
    }

    public struct TrainTrackBridgePillarPatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo MiddlePillarInfo;

        public TrainTrackBridgePillarPatcherState(global::TrainTrackBridgeAI netAi)
        {
            BridgePillarInfo = netAi.m_bridgePillarInfo;
            MiddlePillarInfo = netAi.m_middlePillarInfo;
        }

        public void Restore(global::TrainTrackBridgeAI netAi)
        {
            netAi.m_bridgePillarInfo = BridgePillarInfo;
            netAi.m_middlePillarInfo = MiddlePillarInfo;
        }
    }
}
