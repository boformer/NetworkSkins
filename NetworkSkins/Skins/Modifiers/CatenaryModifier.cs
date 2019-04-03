using ColossalFramework.IO;
using NetworkSkins.Net;
using NetworkSkins.Skins.Serialization;

// TODO currently only removes catenaries
// TODO add support for railway: https://gist.github.com/ronyx69/b4cd41742803eaeac6d19322384a0f4a

// TODO change namespace of all modifiers after merge with GUI branch!
namespace NetworkSkins.Skins.Modifiers
{
    public class CatenaryModifier : NetworkSkinModifier
    {
        public readonly PropInfo Catenary;

        public CatenaryModifier(PropInfo catenary) : base(NetworkSkinModifierType.Catenary)
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
                            CatenaryUtils.CorrectCatenaryPropAngle(laneProp);
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

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteUniqueString(Catenary?.name);
        }

        public static CatenaryModifier DeserializeImpl(DataSerializer s, NetworkSkinLoadErrors errors)
        {
            var catenary = NetworkSkinSerializationUtils.FindPrefab<PropInfo>(s.ReadUniqueString(), errors);

            return new CatenaryModifier(catenary);
        }
        #endregion

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
