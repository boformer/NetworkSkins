using NetworkSkins.Net;

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
            if (Catenary != null)
            {
                UpdateCatenaries(skin);
            }
            else
            {
                RemoveWireSegments(skin);
                RemoveCatenaries(skin);
            }
        }

        private void UpdateCatenaries(NetworkSkin skin)
        {
            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                if (skin.m_lanes[l]?.m_laneProps?.m_props == null) continue;

                for (var p = skin.m_lanes[l].m_laneProps.m_props.Length - 1; p >= 0; p--)
                {
                    var finalProp = skin.m_lanes[l].m_laneProps.m_props[p]?.m_finalProp;
                    if (CatenaryUtils.IsCatenaryProp(finalProp))
                    {
                        skin.UpdateLaneProp(l, p, laneProp =>
                        {
                            laneProp.m_prop = Catenary;
                            laneProp.m_finalProp = Catenary;
                        });
                    }
                }
            }
        }

        private static void RemoveWireSegments(NetworkSkin skin)
        {
            for (var s = skin.m_segments.Length - 1; s >= 0; s--)
            {
                var segment = skin.m_segments[s];
                if (CatenaryUtils.IsWireSegment(segment))
                {
                    skin.RemoveSegment(s);
                }
            }
        }

        private static void RemoveCatenaries(NetworkSkin skin)
        {
            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                if (skin.m_lanes[l]?.m_laneProps?.m_props == null) continue;

                for (var p = skin.m_lanes[l].m_laneProps.m_props.Length - 1; p >= 0; p--)
                {
                    var finalProp = skin.m_lanes[l].m_laneProps.m_props[p]?.m_finalProp;
                    if (CatenaryUtils.IsCatenaryProp(finalProp))
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
