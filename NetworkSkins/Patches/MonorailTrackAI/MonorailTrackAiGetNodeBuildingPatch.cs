using Harmony;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.MonorailTrackAI
{
    [HarmonyPatch(typeof(global::MonorailTrackAI), "GetNodeBuilding")]
    public class MonorailTrackAiGetNodeBuildingPatch
    {
        public static void Prefix(ref global::MonorailTrackAI __instance, ushort nodeID, out MonorailTrackPillarPatcherState? __state)
        {
            var skin = NetworkSkinManager.NodeSkins[nodeID];
            if (skin != null)
            {
                __state = new MonorailTrackPillarPatcherState(__instance.m_bridgePillarInfo, __instance.m_bridgePillarInfo2,
                    __instance.m_bridgePillarInfo3, __instance.m_middlePillarInfo);
                __instance.m_bridgePillarInfo = skin.m_bridgePillarInfo;
                __instance.m_bridgePillarInfo2 = skin.m_bridgePillarInfo2;
                __instance.m_bridgePillarInfo3 = skin.m_bridgePillarInfo3;
                __instance.m_middlePillarInfo = skin.m_middlePillarInfo;
            }
            else
            {
                __state = null;
            }
        }

        public static void Postfix(ref global::MonorailTrackAI __instance, ref MonorailTrackPillarPatcherState? __state)
        {
            if (__state == null)
            {
                return;
            }

            __instance.m_bridgePillarInfo = __state.Value.BridgePillarInfo;
            __instance.m_bridgePillarInfo2 = __state.Value.BridgePillarInfo2;
            __instance.m_bridgePillarInfo3 = __state.Value.BridgePillarInfo3;
            __instance.m_middlePillarInfo = __state.Value.MiddlePillarInfo;
        }
    }

    public struct MonorailTrackPillarPatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo BridgePillarInfo2;
        public readonly BuildingInfo BridgePillarInfo3;
        public readonly BuildingInfo MiddlePillarInfo;

        public MonorailTrackPillarPatcherState(BuildingInfo bridgePillarInfo, BuildingInfo bridgePillarInfo2, BuildingInfo bridgePillarInfo3, BuildingInfo middlePillarInfo)
        {
            BridgePillarInfo = bridgePillarInfo;
            BridgePillarInfo2 = bridgePillarInfo2;
            BridgePillarInfo3 = bridgePillarInfo3;
            MiddlePillarInfo = middlePillarInfo;
        }
    }
}