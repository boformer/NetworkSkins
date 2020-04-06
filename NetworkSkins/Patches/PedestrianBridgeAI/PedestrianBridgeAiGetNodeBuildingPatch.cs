using HarmonyLib;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.PedestrianBridgeAI
{
    [HarmonyPatch(typeof(global::PedestrianBridgeAI), "GetNodeBuilding")]
    public class PedestrianBridgeAiGetNodeBuildingPatch
    {
        public static void Prefix(ref global::PedestrianBridgeAI __instance, ushort nodeID, out PedestrianBridgePillarPatcherState? __state)
        {
            var skin = NetworkSkinManager.NodeSkins[nodeID];
            if (skin != null)
            {
                __state = new PedestrianBridgePillarPatcherState(__instance);

                __instance.m_bridgePillarInfo = skin.m_bridgePillarInfo;
                __instance.m_bridgePillarInfos = skin.m_bridgePillarInfos;
            }
            else
            {
                __state = null;
            }
        }

        public static void Postfix(ref global::PedestrianBridgeAI __instance, ref PedestrianBridgePillarPatcherState? __state)
        {
            __state?.Restore(__instance);
        }
    }

    public struct PedestrianBridgePillarPatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo[] BridgePillarInfos;

        public PedestrianBridgePillarPatcherState(global::PedestrianBridgeAI netAi)
        {
            BridgePillarInfo = netAi.m_bridgePillarInfo;
            BridgePillarInfos = netAi.m_bridgePillarInfos;
        }

        public void Restore(global::PedestrianBridgeAI netAi)
        {
            netAi.m_bridgePillarInfo = BridgePillarInfo;
            netAi.m_bridgePillarInfos = BridgePillarInfos;
        }
    }
}
