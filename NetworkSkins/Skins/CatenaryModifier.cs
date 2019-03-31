using UnityEngine;

// TODO currently only removes catenaries
// TODO add support for railway and catenary replacer catenaries
// TODO problem with missing rail end prop!

namespace NetworkSkins.Skins
{
    public class CatenaryModifier : NetworkSkinModifier
    {
        public readonly PropInfo Catenary;

        public CatenaryModifier(PropInfo catenary)
        {
            Catenary = catenary;
        }

        public override void Apply(NetworkSkin skin)
        {
            
            for (var s = skin.m_segments.Length - 1; s >= 0; s--)
            {
                var segment = skin.m_segments[s];
                if (segment.m_material?.shader?.name == "Custom/Net/Electricity")
                {
                    skin.RemoveSegment(s);
                }
            }

            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                if (skin.m_lanes[l]?.m_laneProps?.m_props == null) continue;

                for (var p = skin.m_lanes[l].m_laneProps.m_props.Length - 1; p >= 0; p--)
                {
                    var propName = skin.m_lanes[l].m_laneProps.m_props[p]?.m_finalProp?.name;
                    if (propName == "RailwayPowerline" || propName == "RailwayPowerline Singular")
                    {
                        skin.RemoveLaneProp(l, p);
                    }
                }
            }
        }

        #region Equality
        protected bool Equals(CatenaryModifier other)
        {
            return Equals(Catenary, other.Catenary);
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

            return Equals((CatenaryModifier) obj);
        }

        public override int GetHashCode()
        {
            return (Catenary != null ? Catenary.GetHashCode() : 0);
        } 
        #endregion
    }
}
