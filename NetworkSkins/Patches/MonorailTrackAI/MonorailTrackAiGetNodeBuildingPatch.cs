using HarmonyLib;
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
                __state = new MonorailTrackPillarPatcherState(__instance);

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
            __state?.Restore(__instance);
        }
    }

    public struct MonorailTrackPillarPatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo BridgePillarInfo2;
        public readonly BuildingInfo BridgePillarInfo3;
        public readonly BuildingInfo MiddlePillarInfo;

        public MonorailTrackPillarPatcherState(global::MonorailTrackAI netAi)
        {
            BridgePillarInfo = netAi.m_bridgePillarInfo;
            BridgePillarInfo2 = netAi.m_bridgePillarInfo2;
            BridgePillarInfo3 = netAi.m_bridgePillarInfo3;
            MiddlePillarInfo = netAi.m_middlePillarInfo;
        }

        public void Restore(global::MonorailTrackAI netAi)
        {
            netAi.m_bridgePillarInfo = BridgePillarInfo;
            netAi.m_bridgePillarInfo2 = BridgePillarInfo2;
            netAi.m_bridgePillarInfo3 = BridgePillarInfo3;
            netAi.m_middlePillarInfo = MiddlePillarInfo;
        }
    }
}