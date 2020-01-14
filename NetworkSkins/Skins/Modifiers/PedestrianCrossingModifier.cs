using ColossalFramework.IO;

namespace NetworkSkins.Skins.Modifiers
{
    public class PedestrianCrossingModifier : NetworkSkinModifier
    {
        public readonly bool HidePedestrianCrossings;
        public PedestrianCrossingModifier(bool hidePedestrianCrossings) : base(NetworkSkinModifierType.NoPedestrianCrossing) 
        {
            HidePedestrianCrossings = hidePedestrianCrossings;
        }

        public override void Apply(NetworkSkin skin)
        {
            skin.m_hidePedestrianCrossings = HidePedestrianCrossings;
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteBool(HidePedestrianCrossings);
        }

        public static PedestrianCrossingModifier DeserializeImpl(DataSerializer s)
        {
            var hidePedestrianCrossings = s.ReadBool();

            return new PedestrianCrossingModifier(hidePedestrianCrossings);
        }
        #endregion

        #region Equality
        protected bool Equals(PedestrianCrossingModifier other)
        {
            return HidePedestrianCrossings == other.HidePedestrianCrossings;
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

            return Equals((PedestrianCrossingModifier)obj);
        }

        public override int GetHashCode()
        {
            return 1034881573 + HidePedestrianCrossings.GetHashCode();
        }
        #endregion
    }
}
