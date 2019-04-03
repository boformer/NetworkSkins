using System;
using System.Linq;
using ColossalFramework.IO;
using NetworkSkins.Skins.Serialization;

namespace NetworkSkins.Skins.Modifiers
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
            BuildingInfo middlePillarInfo = null
        ) : base(NetworkSkinModifierType.Pillar)
        {
            // bridgePillarInfos is not allowed to contain null (will cause exceptions in NetTool)
            if (bridgePillarInfos != null && bridgePillarInfos.Contains(null))
            {
                throw new Exception("bridgePillarInfos cannot contain null!");
            }
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

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteUniqueString(BridgePillarInfo?.name);
            s.WriteUniqueString(BridgePillarInfo2?.name);
            s.WriteUniqueString(BridgePillarInfo3?.name);
            s.WriteUniqueStringArray(BridgePillarInfos?.Select(prefab => prefab?.name).ToArray());
            s.WriteUniqueString(MiddlePillarInfo?.name);
        }

        public static PillarModifier DeserializeImpl(DataSerializer s, NetworkSkinLoadErrors errors)
        {
            var bridgePillarInfo = NetworkSkinSerializationUtils.FindPrefab<BuildingInfo>(s.ReadUniqueString(), errors);

            var bridgePillarInfo2 = NetworkSkinSerializationUtils.FindPrefab<BuildingInfo>(s.ReadUniqueString(), errors);

            var bridgePillarInfo3 = NetworkSkinSerializationUtils.FindPrefab<BuildingInfo>(s.ReadUniqueString(), errors);

            var bridgePillarNames = s.ReadUniqueStringArray();
            BuildingInfo[] bridgePillarInfos = null;
            if (bridgePillarNames != null)
            {
                bridgePillarInfos = bridgePillarNames.Select(prefabName => NetworkSkinSerializationUtils.FindPrefab<BuildingInfo>(prefabName, errors)).ToArray();

                // bridgePillarInfos is not allowed to contain null (will cause exceptions in NetTool)
                if (bridgePillarInfos.Contains(null)) 
                {
                    bridgePillarInfos = null;
                }
            }

            var middlePillarInfo = NetworkSkinSerializationUtils.FindPrefab<BuildingInfo>(s.ReadUniqueString(), errors);

            return new PillarModifier(bridgePillarInfo, bridgePillarInfo2, bridgePillarInfo3, bridgePillarInfos, middlePillarInfo);
        }
        #endregion

        #region Equality
        protected bool Equals(PillarModifier other)
        {
            return Equals(BridgePillarInfo, other.BridgePillarInfo) && Equals(BridgePillarInfo2, other.BridgePillarInfo2)
                && Equals(BridgePillarInfo3, other.BridgePillarInfo3) && Equals(BridgePillarInfos, other.BridgePillarInfos)
                && Equals(MiddlePillarInfo, other.MiddlePillarInfo);
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
