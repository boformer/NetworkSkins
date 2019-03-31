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

        #region Equality
        protected bool Equals(PillarModifier other)
        {
            return Equals(BridgePillarInfo, other.BridgePillarInfo) && Equals(BridgePillarInfo2, other.BridgePillarInfo2) && Equals(BridgePillarInfo3, other.BridgePillarInfo3) && Equals(BridgePillarInfos, other.BridgePillarInfos) && Equals(MiddlePillarInfo, other.MiddlePillarInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((PillarModifier)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (BridgePillarInfo != null ? BridgePillarInfo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BridgePillarInfo2 != null ? BridgePillarInfo2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BridgePillarInfo3 != null ? BridgePillarInfo3.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BridgePillarInfos != null ? BridgePillarInfos.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MiddlePillarInfo != null ? MiddlePillarInfo.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
