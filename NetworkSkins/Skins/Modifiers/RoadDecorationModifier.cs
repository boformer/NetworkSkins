using ColossalFramework.IO;

namespace NetworkSkins.Skins.Modifiers
{
    public class RoadDecorationModifier : NetworkSkinModifier
    {
        public readonly bool nodeMarkingsHidden;
        public RoadDecorationModifier(bool nodeMarkingsHidden) : base(NetworkSkinModifierType.RoadDecoration) 
        {
            this.nodeMarkingsHidden = nodeMarkingsHidden;
        }

        public override void Apply(NetworkSkin skin)
        {
            skin.m_nodeMarkingsHidden = nodeMarkingsHidden;
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteBool(nodeMarkingsHidden);
        }

        public static RoadDecorationModifier DeserializeImpl(DataSerializer s)
        {
            var nodeMarkingsHidden = s.ReadBool();

            return new RoadDecorationModifier(nodeMarkingsHidden);
        }
        #endregion

        #region Equality
        protected bool Equals(RoadDecorationModifier other)
        {
            return nodeMarkingsHidden == other.nodeMarkingsHidden;
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

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((RoadDecorationModifier)obj);
        }

        public override int GetHashCode()
        {
            return 1034881573 + nodeMarkingsHidden.GetHashCode();
        }
        #endregion
    }
}
