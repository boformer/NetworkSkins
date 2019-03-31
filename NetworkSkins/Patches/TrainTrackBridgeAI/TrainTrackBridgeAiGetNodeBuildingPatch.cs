using Harmony;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.TrainTrackBridgeAI
{
    [HarmonyPatch(typeof(global::TrainTrackBridgeAI), "GetNodeBuilding")]
    public static class TrainTrackBridgeAiGetNodeBuildingPatch
    {
        public static void Prefix(ref global::TrainTrackBridgeAI __instance, ushort nodeID, out TrainTrackBridgePillarPatcherState? __state)
        {
            var skin = NetworkSkinManager.NodeSkins[nodeID];
            if (skin != null)
            {
                __state = new TrainTrackBridgePillarPatcherState(__instance.m_bridgePillarInfo, __instance.m_middlePillarInfo);
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
            if (__state == null)
            {
                return;
            }

            __instance.m_bridgePillarInfo = __state.Value.BridgePillarInfo;
            __instance.m_middlePillarInfo = __state.Value.MiddlePillarInfo;
        }
    }

    public struct TrainTrackBridgePillarPatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo MiddlePillarInfo;

        public TrainTrackBridgePillarPatcherState(BuildingInfo bridgePillarInfo, BuildingInfo middlePillarInfo)
        {
            BridgePillarInfo = bridgePillarInfo;
            MiddlePillarInfo = middlePillarInfo;
        }
    }
}
