using Harmony;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.PedestrianBridgeAI
{
    [HarmonyPatch(typeof(global::PedestrianBridgeAI), "GetNodeBuilding")]
    public class PedestrianBridgeAiGetNodeBuildingPatch
    {
        public static void Prefix(ref global::PedestrianBridgeAI __instance, ushort nodeID, out PedestrianBridgePatcherState? __state)
        {
            var skin = NetworkSkinManager.NodeSkins[nodeID];
            if (skin != null)
            {
                __state = new PedestrianBridgePatcherState(__instance.m_bridgePillarInfo, __instance.m_bridgePillarInfos);
                __instance.m_bridgePillarInfo = skin.m_bridgePillarInfo;
                __instance.m_bridgePillarInfos = skin.m_bridgePillarInfos;
            }
            else
            {
                __state = null;
            }
        }

        public static void Postfix(ref global::PedestrianBridgeAI __instance, ref PedestrianBridgePatcherState? __state)
        {
            if (__state == null)
            {
                return;
            }

            __instance.m_bridgePillarInfo = __state.Value.BridgePillarInfo;
            __instance.m_bridgePillarInfos = __state.Value.BridgePillarInfos;
        }
    }

    public struct PedestrianBridgePatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo[] BridgePillarInfos;

        public PedestrianBridgePatcherState(BuildingInfo bridgePillarInfo, BuildingInfo[] bridgePillarInfos)
        {
            BridgePillarInfo = bridgePillarInfo;
            BridgePillarInfos = bridgePillarInfos;
        }
    }
}
