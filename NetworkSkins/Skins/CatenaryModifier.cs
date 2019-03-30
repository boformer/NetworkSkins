using UnityEngine;

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
            // TODO currently only removes catenaries
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
                var laneProps = skin.m_lanes[l]?.m_laneProps?.m_props;
                if (laneProps == null) continue;

                for (var p = laneProps.Length - 1; p >= 0; p--)
                {
                    if (laneProps[p]?.m_finalProp?.name == "RailwayPowerline")
                    {
                        skin.RemoveLaneProp(l, p);
                    }
                }
            }
        }
    }
}
