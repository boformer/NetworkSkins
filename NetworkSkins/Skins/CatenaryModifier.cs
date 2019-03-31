using UnityEngine;

// TODO currently only removes catenaries
// TODO add support for railway and catenary replacer catenaries

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
    }
}
