// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches
{
    // TODO remove
    public struct PillarPatcherState
    {
        public readonly BuildingInfo m_bridgePillarInfo;
        public readonly BuildingInfo m_bridgePillarInfo2;
        public readonly BuildingInfo m_bridgePillarInfo3;
        public readonly BuildingInfo[] m_bridgePillarInfos;
        public readonly BuildingInfo m_middlePillarInfo;

        public PillarPatcherState(
            BuildingInfo bridgePillarInfo = null, 
            BuildingInfo bridgePillarInfo2 = null, 
            BuildingInfo bridgePillarInfo3 = null, 
            BuildingInfo[] bridgePillarInfos = null, 
            BuildingInfo middlePillarInfo = null)
        {
            m_bridgePillarInfo = bridgePillarInfo;
            m_bridgePillarInfo2 = bridgePillarInfo2;
            m_bridgePillarInfo3 = bridgePillarInfo3;
            m_bridgePillarInfos = bridgePillarInfos;
            m_middlePillarInfo = middlePillarInfo;
        }
    }
}
