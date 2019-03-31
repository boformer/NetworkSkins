namespace NetworkSkins.Skins
{
    public class PillarModifier : NetworkSkinModifier
    {
        public readonly BuildingInfo BridgePillarInfo;

        // Monorail bend pillar
        public readonly BuildingInfo BridgePillarInfo2;

        // Monorail junction pillar
        public readonly BuildingInfo BridgePillarInfo3;

        // Pedestrian path elevation-dependent pillars
        public readonly BuildingInfo[] BridgePillarInfos;

        public readonly BuildingInfo MiddlePillarInfo;

        public PillarModifier(
            BuildingInfo bridgePillarInfo = null, 
            BuildingInfo bridgePillarInfo2 = null, 
            BuildingInfo bridgePillarInfo3 = null, 
            BuildingInfo[] bridgePillarInfos = null, 
            BuildingInfo middlePillarInfo = null)
        {
            BridgePillarInfo = bridgePillarInfo;
            BridgePillarInfo2 = bridgePillarInfo2;
            BridgePillarInfo3 = bridgePillarInfo3;
            BridgePillarInfos = bridgePillarInfos;
            MiddlePillarInfo = middlePillarInfo;
        }

        public override void Apply(NetworkSkin skin)
        {
            skin.m_bridgePillarInfo = BridgePillarInfo;
            skin.m_bridgePillarInfo2 = BridgePillarInfo2;
            skin.m_bridgePillarInfo3 = BridgePillarInfo3;
            skin.m_bridgePillarInfos = BridgePillarInfos;
            skin.m_middlePillarInfo = MiddlePillarInfo;
        }
    }
}
