using ColossalFramework.IO;
using NetworkSkins.Net;
using NetworkSkins.Skins.Serialization;

namespace NetworkSkins.Skins.Modifiers
{
    public class PillarModifier : NetworkSkinModifier
    {
        public readonly PillarType Type;
        public readonly BuildingInfo Pillar;

        public PillarModifier(PillarType type, BuildingInfo pillar) : base(NetworkSkinModifierType.Pillar)
        {
            Type = type;
            Pillar = pillar;
        }

        public override void Apply(NetworkSkin skin)
        {
            if (Type == PillarType.Bridge)
            {
                skin.m_bridgePillarInfo = Pillar;
            }
            else if(Type == PillarType.Middle)
            {
                skin.m_middlePillarInfo = Pillar;
            }

            skin.m_bridgePillarInfo2 = null;
            skin.m_bridgePillarInfo3 = null;
            skin.m_bridgePillarInfos = null;
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteInt32((int)Type);
            s.WriteUniqueString(Pillar?.name);
        }

        public static PillarModifier DeserializeImpl(DataSerializer s, IPrefabCollection prefabCollection, NetworkSkinLoadErrors errors)
        {
            var type = (PillarType)s.ReadInt32();

            var pillar = prefabCollection.FindPrefab<BuildingInfo>(s.ReadUniqueString(), errors);

            return new PillarModifier(type, pillar);
        }
        #endregion

        #region Equality
        protected bool Equals(PillarModifier other)
        {
            return Type == other.Type && Equals(Pillar, other.Pillar);
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

            return Equals((PillarModifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Type * 397) ^ (Pillar != null ? Pillar.GetHashCode() : 0);
            }
        }
        #endregion
    }
}
