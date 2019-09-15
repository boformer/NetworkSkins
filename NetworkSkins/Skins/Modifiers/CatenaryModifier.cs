using ColossalFramework.IO;
using NetworkSkins.Net;
using NetworkSkins.Skins.Serialization;

namespace NetworkSkins.Skins.Modifiers
{
    // segmentOffset 
    // start: -1
    // middle: 0
    // end: 1
    // Inverted flag set on some props

    // Ronyx tracks:
    // Catenaries on second lane
    // endFlagsForbidden: End - for non-end catenaries (1)
    // startFlagsForbidden: End

    // startFlagsRequired: End (-1)
    // rotated by 180


    // single powerline positions

    // offset -1
    // position 2 | -0.15 | 0

    // offset 0
    // position 2 | -0.15 | 0

    // offset 1
    // position 2 | -0.15 | 0
    public class CatenaryModifier : NetworkSkinModifier
    {
        public readonly PropInfo Catenary;
        private readonly PropInfo _endCatenary;
        private readonly PropInfo _tunnelCatenary;

        public CatenaryModifier(PropInfo catenary) : base(NetworkSkinModifierType.Catenary)
        {
            Catenary = catenary;
            _endCatenary = CatenaryUtils.GetEndCatenary(catenary);
            _tunnelCatenary = CatenaryUtils.GetTunnelCatenary(catenary);
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
            if (skin.m_lanes == null) return;

            // TODO implement tunnel cats
            //var tunnelCatenary = CatenaryUtils.GetEndCatenary(Catenary); 

            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                if (skin.m_lanes[l]?.m_laneProps?.m_props == null) continue;

                for (var p = skin.m_lanes[l].m_laneProps.m_props.Length - 1; p >= 0; p--)
                {
                    var laneProp = skin.m_lanes[l].m_laneProps.m_props[p];
                    if (laneProp?.m_finalProp == null) continue;

                    if(CatenaryUtils.IsNormalCatenaryProp(laneProp.m_finalProp) || CatenaryUtils.IsEndCatenaryProp(laneProp.m_finalProp))
                    {
                        if (Catenary == _endCatenary)
                        {
                            ReplaceWithNormalCatenary(skin, l, p);
                        }
                        else
                        {
                            // Start node catenary
                            if (laneProp.m_segmentOffset == -1f)
                            {
                                // Start-of-track node
                                if ((laneProp.m_startFlagsRequired & NetNode.Flags.End) != 0)
                                {
                                    ReplaceWithEndCatenary(skin, l, p);
                                }
                                // Mid-track node
                                else if ((laneProp.m_startFlagsForbidden & NetNode.Flags.End) != 0)
                                {
                                    ReplaceWithNormalCatenary(skin, l, p);
                                }
                                // Vanilla rail: Create separate lane prop for start-of-track
                                else
                                {
                                    skin.CopyAndUpdateLaneProp(l, p, laneProp2 =>
                                    {
                                        laneProp2.m_prop = _endCatenary;
                                        laneProp2.m_finalProp = _endCatenary;
                                        laneProp2.m_startFlagsRequired |= NetNode.Flags.End;
                                        CatenaryUtils.CorrectCatenaryPropAngleAndPosition(laneProp2);
                                    });
                                    skin.UpdateLaneProp(l, p, laneProp2 =>
                                    {
                                        laneProp2.m_prop = Catenary;
                                        laneProp2.m_finalProp = Catenary;
                                        laneProp2.m_startFlagsForbidden |= NetNode.Flags.End;
                                        CatenaryUtils.CorrectCatenaryPropAngleAndPosition(laneProp2);
                                    });
                                }
                            }

                            // End node catenary
                            else if (laneProp.m_segmentOffset == 1f)
                            {
                                // End-of-track node
                                if ((laneProp.m_endFlagsRequired & NetNode.Flags.End) != 0)
                                {
                                    ReplaceWithEndCatenary(skin, l, p);
                                }
                                // Mid-track node
                                else if ((laneProp.m_endFlagsForbidden & NetNode.Flags.End) != 0)
                                {
                                    ReplaceWithNormalCatenary(skin, l, p);
                                }
                                // Vanilla rail: Create separate lane prop for end-of-track
                                else
                                {
                                    skin.CopyAndUpdateLaneProp(l, p, laneProp2 =>
                                    {
                                        laneProp2.m_prop = _endCatenary;
                                        laneProp2.m_finalProp = _endCatenary;
                                        laneProp2.m_endFlagsRequired |= NetNode.Flags.End;
                                        CatenaryUtils.CorrectCatenaryPropAngleAndPosition(laneProp2);
                                    });
                                    skin.UpdateLaneProp(l, p, laneProp2 =>
                                    {
                                        laneProp2.m_prop = Catenary;
                                        laneProp2.m_finalProp = Catenary;
                                        laneProp2.m_endFlagsForbidden |= NetNode.Flags.End;
                                        CatenaryUtils.CorrectCatenaryPropAngleAndPosition(laneProp2);
                                    });
                                }
                            }

                            // Mid-segment catenary
                            else
                            {
                                ReplaceWithNormalCatenary(skin, l, p);
                            }
                        }
                    }
                    else if (CatenaryUtils.IsTunnelCatenaryProp(laneProp.m_finalProp))
                    {
                        if(_tunnelCatenary != null)
                        {
                            ReplaceWithTunnelCatenary(skin, l, p);
                        }
                    }
                }
            }
        }

        private void ReplaceWithNormalCatenary(NetworkSkin skin, int l, int p)
        {
            skin.UpdateLaneProp(l, p, laneProp2 =>
            {
                laneProp2.m_prop = Catenary;
                laneProp2.m_finalProp = Catenary;
                CatenaryUtils.CorrectCatenaryPropAngleAndPosition(laneProp2);
            });
        }

        private void ReplaceWithEndCatenary(NetworkSkin skin, int l, int p)
        {
            skin.UpdateLaneProp(l, p, laneProp2 =>
            {
                laneProp2.m_prop = _endCatenary;
                laneProp2.m_finalProp = _endCatenary;
                CatenaryUtils.CorrectCatenaryPropAngleAndPosition(laneProp2);
            });
        }

        private void ReplaceWithTunnelCatenary(NetworkSkin skin, int l, int p)
        {
            skin.UpdateLaneProp(l, p, laneProp2 =>
            {
                laneProp2.m_prop = _tunnelCatenary;
                laneProp2.m_finalProp = _tunnelCatenary;
                CatenaryUtils.CorrectCatenaryPropAngleAndPosition(laneProp2);
            });
        }

        private static void RemoveWireSegments(NetworkSkin skin)
        {
            if (skin.m_segments == null) return;

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
            if (skin.m_lanes == null) return;

            for (var l = 0; l < skin.m_lanes.Length; l++)
            {
                if (skin.m_lanes[l]?.m_laneProps?.m_props == null) continue;

                for (var p = skin.m_lanes[l].m_laneProps.m_props.Length - 1; p >= 0; p--)
                {
                    var finalProp = skin.m_lanes[l].m_laneProps.m_props[p]?.m_finalProp;
                    if (CatenaryUtils.IsNormalCatenaryProp(finalProp) || CatenaryUtils.IsEndCatenaryProp(finalProp) || CatenaryUtils.IsTunnelCatenaryProp(finalProp))
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

        public static CatenaryModifier DeserializeImpl(DataSerializer s, IPrefabCollection prefabCollection, NetworkSkinLoadErrors errors)
        {
            var catenary = prefabCollection.FindPrefab<PropInfo>(s.ReadUniqueString(), errors);

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

            if (obj.GetType() != GetType())
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
