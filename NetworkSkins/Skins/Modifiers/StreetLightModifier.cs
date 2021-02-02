using ColossalFramework.IO;
using NetworkSkins.Net;
using NetworkSkins.Skins.Serialization;

namespace NetworkSkins.Skins.Modifiers
{
    public class StreetLightModifier : NetworkSkinModifier
    {
        public readonly PropInfo StreetLight;

        public readonly float RepeatDistance;

        public StreetLightModifier(PropInfo streetLight, float repeatDistance = 40) : base(NetworkSkinModifierType.StreetLight)
        {
            StreetLight = streetLight;
            RepeatDistance = repeatDistance;
        }

        public override void Apply(NetworkSkin skin)
        {
            if (skin.m_lanes == null) return;

            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                var laneProps = skin.m_lanes[l]?.m_laneProps?.m_props;
                if (laneProps == null) continue;

                for (var p = laneProps.Length - 1; p >= 0; p--)
                {
                    if (StreetLightUtils.IsStreetLightProp(laneProps[p]?.m_finalProp))
                    {
                        if (StreetLight != null) {
                            skin.UpdateLaneProp(l, p, laneProp =>
                            {
                                laneProp.m_prop = StreetLight;
                                laneProp.m_finalProp = StreetLight;
                                laneProp.m_repeatDistance = RepeatDistance;
                                StreetLightUtils.CorrectStreetLightPropAngleAndPosition(skin.Prefab, laneProp, skin.Prefab.m_halfWidth, skin.m_lanes[l].m_position);
                            });
                        }
                        else
                        {
                            skin.RemoveLaneProp(l, p);
                        }
                    }
                }
            }
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteUniqueString(StreetLight?.name);
            s.WriteFloat(RepeatDistance);
        }

        public static StreetLightModifier DeserializeImpl(DataSerializer s, IPrefabCollection prefabCollection, NetworkSkinLoadErrors errors)
        {
            var streetLight = prefabCollection.FindPrefab<PropInfo>(s.ReadUniqueString(), errors);
            var repeatDistance = s.ReadFloat();

            return new StreetLightModifier(streetLight, repeatDistance);
        }
        #endregion

        #region Equality
        protected bool Equals(StreetLightModifier other)
        {
            return Equals(StreetLight, other.StreetLight) && RepeatDistance.Equals(other.RepeatDistance);
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

            return Equals((StreetLightModifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((StreetLight != null ? StreetLight.GetHashCode() : 0) * 397) ^ RepeatDistance.GetHashCode();
            }
        }
        #endregion
    }
}
