using NetworkSkins.Net;

namespace NetworkSkins.Skins
{
    public class StreetLightModifier : NetworkSkinModifier
    {
        public readonly PropInfo StreetLight;

        public readonly float RepeatDistance;

        public StreetLightModifier(PropInfo streetLight, float repeatDistance = 40)
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

                for (var p = 0; p < laneProps.Length; p++)
                {
                    if (StreetLightUtils.IsStreetLightProp(laneProps[p]?.m_finalProp))
                    {
                        skin.UpdateLaneProp(l, p, laneProp =>
                        {
                            laneProp.m_prop = StreetLight;
                            laneProp.m_finalProp = StreetLight;
                            laneProp.m_repeatDistance = RepeatDistance;
                        });
                    }
                }
            }
        }

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

            if (obj.GetType() != this.GetType())
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
