using NetworkSkins.Net;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches
{
    /// <summary>
    /// Temporarily patches the pillars of the NetInfo with the ones selected in the GUI.
    /// Used to patch NetTool.
    /// </summary>
    public static class ActivePillarPatcher
    {
        public static void GetActiveNodeBuilding(NetAI netAI, ushort nodeID, ref NetNode data, out BuildingInfo building, out float heightOffset)
        {
            var patcherState = Apply(netAI.m_info, NetworkSkinManager.instance.GetActiveSkin(netAI.m_info));
            netAI.GetNodeBuilding(nodeID, ref data, out building, out heightOffset);
            Revert(netAI.m_info, patcherState);
        }

        public static PillarPatcherState? Apply(NetInfo info, NetworkSkin skin)
        {
            if (info == null || skin == null)
            {
                return null;
            }

            var state = new PillarPatcherState(info);

            PillarUtils.SetBridgePillar(info, skin.m_bridgePillarInfo);
            PillarUtils.SetBridgePillar2(info, skin.m_bridgePillarInfo2);
            PillarUtils.SetBridgePillar3(info, skin.m_bridgePillarInfo3);
            PillarUtils.SetBridgePillars(info, skin.m_bridgePillarInfos);
            PillarUtils.SetMiddlePillar(info, skin.m_middlePillarInfo);

            return state;
        }

        public static void Revert(NetInfo info, PillarPatcherState? state)
        {
            if (info == null || state == null)
            {
                return;
            }

            state.Value.Restore(info);
        }
    }
    public struct PillarPatcherState
    {
        public readonly BuildingInfo BridgePillarInfo;
        public readonly BuildingInfo BridgePillarInfo2;
        public readonly BuildingInfo BridgePillarInfo3;
        public readonly BuildingInfo[] BridgePillarInfos;
        public readonly BuildingInfo MiddlePillarInfo;

        public PillarPatcherState(NetInfo prefab)
        {
            BridgePillarInfo = PillarUtils.GetDefaultBridgePillar(prefab);
            BridgePillarInfo2 = PillarUtils.GetDefaultBridgePillar2(prefab);
            BridgePillarInfo3 = PillarUtils.GetDefaultBridgePillar3(prefab);
            BridgePillarInfos = PillarUtils.GetDefaultBridgePillars(prefab);
            MiddlePillarInfo = PillarUtils.GetDefaultMiddlePillar(prefab);
        }

        public void Restore(NetInfo prefab)
        {
            PillarUtils.SetBridgePillar(prefab, BridgePillarInfo);
            PillarUtils.SetBridgePillar2(prefab, BridgePillarInfo2);
            PillarUtils.SetBridgePillar3(prefab, BridgePillarInfo3);
            PillarUtils.SetBridgePillars(prefab, BridgePillarInfos);
            PillarUtils.SetMiddlePillar(prefab, MiddlePillarInfo);
        }
    }
}
