using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ColossalFramework.IO;
using NetworkSkins.Net;
using NetworkSkins.Skins.Serialization;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Skins
{
    public class NetworkSkin
    {
        public readonly NetInfo Prefab;

        public ReadOnlyCollection<NetworkSkinModifier> Modifiers => _modifiers.AsReadOnly();
        private readonly List<NetworkSkinModifier> _modifiers;

        public BuildingInfo m_bridgePillarInfo;

        // Monorail bend pillar
        public BuildingInfo m_bridgePillarInfo2;

        // Monorail junction pillar
        public BuildingInfo m_bridgePillarInfo3;

        // Pedestrian path elevation-dependent pillars
        public BuildingInfo[] m_bridgePillarInfos;

        public BuildingInfo m_middlePillarInfo;

        public NetInfo.Lane[] m_lanes;
        public NetInfo.Segment[] m_segments;

        public bool m_createPavement;
        public bool m_createGravel;
        public bool m_createRuining;
        public bool m_clipTerrain;

        public Color m_color;

        public bool m_hasWires;
        
        public int UseCount = 0;

        public NetworkSkin(NetInfo prefab, List<NetworkSkinModifier> modifiers)
        {
            Prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            _modifiers = modifiers;

            m_bridgePillarInfo = PillarUtils.GetDefaultBridgePillar(prefab);
            m_bridgePillarInfo2 = PillarUtils.GetDefaultBridgePillar2(prefab);
            m_bridgePillarInfo3 = PillarUtils.GetDefaultBridgePillar3(prefab);
            m_bridgePillarInfos = PillarUtils.GetDefaultBridgePillars(prefab);
            m_middlePillarInfo = PillarUtils.GetDefaultMiddlePillar(prefab);
            
            if (prefab.m_lanes != null)
            {
                m_lanes = new NetInfo.Lane[prefab.m_lanes.Length];
                Array.Copy(prefab.m_lanes, m_lanes, m_lanes.Length);
            }

            m_hasWires = true;
            if (prefab.m_segments != null)
            {
                m_segments = new NetInfo.Segment[prefab.m_segments.Length];
                Array.Copy(prefab.m_segments, m_segments, m_segments.Length);
            }
            m_createPavement = prefab.m_createPavement;
            m_createGravel = prefab.m_createGravel;
            m_createRuining = prefab.m_createRuining;
            m_clipTerrain = prefab.m_clipTerrain;
            m_color = prefab.m_color;

            UpdateHasWires();

            foreach (var modifier in _modifiers)
            {
                modifier.Apply(this);
            }
        }

        public void Destroy()
        {
            if (m_lanes != null)
            {
                foreach (var lane in m_lanes)
                {
                    if (lane is NetworkSkinLane && lane.m_laneProps != null)
                    {
                        // NetLaneProps is a ScriptableObject, must be destroyed when no longer in use!
                        UnityEngine.Object.Destroy(lane.m_laneProps);
                        lane.m_laneProps = null;
                    }
                }
            }

            if (m_segments != null)
            {
                foreach (var segment in m_segments)
                {
                    if (segment is NetworkSkinSegment && segment.m_material != null)
                    {
                        // NetLaneProps is a ScriptableObject, must be destroyed when no longer in use!
                        UnityEngine.Object.Destroy(segment.m_material);
                        segment.m_material = null;
                    }
                }
            }
        }

        #region Modifications
        // Updates a lane prop without affecting the original lane props of the network
        public void UpdateLaneProp(int laneIndex, int propIndex, Action<NetLaneProps.Prop> updater)
        {
            if (m_lanes == null || m_lanes.Length <= laneIndex)
            {
                Debug.LogError($"Invalid lane index {laneIndex} for prefab {Prefab}!");
                return;
            }

            var lane = m_lanes[laneIndex];
            if (lane == null || lane.m_laneProps == null || lane.m_laneProps.m_props == null)
            {
                Debug.LogError($"Lane {laneIndex} is null or doesn't have any props!");
                return;
            }

            if (lane.m_laneProps.m_props.Length <= propIndex)
            {
                Debug.LogError($"Invalid prop index {propIndex} for prefab {Prefab}, lane {laneIndex}!");
                return;
            }

            // duplicate the lane so we do not affect the original prefab
            if (!(lane is NetworkSkinLane))
            {
                lane = new NetworkSkinLane(lane);
                m_lanes[laneIndex] = lane;
            }

            var prop = lane.m_laneProps.m_props[propIndex];
            if (prop == null)
            {
                Debug.LogError($"Prop {propIndex} is null for prefab {Prefab}, lane {laneIndex}!");
                return;
            }

            updater(prop);
        }

        public void RemoveLaneProp(int laneIndex, int propIndex)
        {
            if (m_lanes == null || m_lanes.Length <= laneIndex)
            {
                Debug.LogError($"Invalid lane index {laneIndex} for prefab {Prefab}!");
                return;
            }

            var lane = m_lanes[laneIndex];
            if (lane == null || lane.m_laneProps == null || lane.m_laneProps.m_props == null)
            {
                Debug.LogError($"Lane {laneIndex} is null or doesn't have any props!");
                return;
            }

            if (lane.m_laneProps.m_props.Length <= propIndex)
            {
                Debug.LogError($"Invalid prop index {propIndex} for prefab {Prefab}, lane {laneIndex}!");
                return;
            }

            // duplicate the lane so we do not affect the original prefab
            if (!(lane is NetworkSkinLane))
            {
                lane = new NetworkSkinLane(lane);
                m_lanes[laneIndex] = lane;
            }

            var props = new List<NetLaneProps.Prop>(lane.m_laneProps.m_props);
            props.RemoveAt(propIndex);
            lane.m_laneProps.m_props = props.ToArray();
        }

        public void UpdateSegmentMaterial(int segmentIndex, Action<Material> updater)
        {
            if (m_segments == null || m_segments.Length <= segmentIndex)
            {
                Debug.LogError($"Invalid segment index {segmentIndex} for prefab {Prefab}!");
                return;
            }

            var segment = m_segments[segmentIndex];
            if (segment == null || segment.m_segmentMaterial == null)
            {
                Debug.LogError($"Segment {segmentIndex} is null or doesn't have a material!");
                return;
            }

            // duplicate the segment so we do not affect the original prefab
            if (!(segment is NetworkSkinSegment))
            {
                Debug.Log("Creating custom NetworkSkinSegment");
                segment = new NetworkSkinSegment(segment);
                m_segments[segmentIndex] = segment;
            }

            updater(segment.m_segmentMaterial);

            UpdateHasWires();
        }

        public void RemoveSegment(int segmentIndex)
        {
            var segments = new List<NetInfo.Segment>(m_segments);
            segments.RemoveAt(segmentIndex);
            m_segments = segments.ToArray();

            UpdateHasWires();
        }

        private void UpdateHasWires()
        {
            m_hasWires = false;

            if (m_segments == null) return;

            foreach (var segment in m_segments)
            {
                if (segment.m_segmentMaterial?.shader?.name == "Custom/Net/Electricity")
                {
                    m_hasWires = true;
                    break;
                }
            }
        }
        #endregion

        #region Serialization
        public void Serialize(DataSerializer s)
        {
            s.WriteUniqueString(Prefab.name);
            s.WriteInt32(_modifiers.Count);
            foreach (var modifier in _modifiers)
            {
                modifier.Serialize(s);
            }
        }

        // nullable
        public static NetworkSkin Deserialize(DataSerializer s, IPrefabCollection prefabCollection, NetworkSkinLoadErrors errors)
        {
            var prefab = prefabCollection.FindPrefab<NetInfo>(s.ReadUniqueString(), errors);

            var modifiersCount = s.ReadInt32();
            var modifiers = new List<NetworkSkinModifier>();
            for (var m = 0; m < modifiersCount; m++)
            {
                var modifier = NetworkSkinModifier.Deserialize(s, prefabCollection, errors);
                if (modifier != null)
                {
                    modifiers.Add(modifier);
                }
            }

            // We are checking if the prefab is null after all the modifiers are deserialized!
            // This is important for the deserialization of the next items!
            if (prefab == null)
            {
                return null;
            }

            return new NetworkSkin(prefab, modifiers);
        }
        #endregion

        public override string ToString()
        {
            return $"Skin for {Prefab.name} with {_modifiers.Count} modifiers (used {UseCount} times)";
        }

        // nullable
        public static NetworkSkin GetMatchingSkinFromList(List<NetworkSkin> skins, NetInfo prefab, List<NetworkSkinModifier> modifiers)
        {
            foreach (var skin in skins)
            {
                if (skin.Prefab == prefab && skin._modifiers.SequenceEqual(modifiers))
                {
                    return skin;
                }
            }

            return null;
        }

        private static void CopyProperties(object target, object origin)
        {
            var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                fieldInfo.SetValue(target, fieldInfo.GetValue(origin));
            }
        }

        private class NetworkSkinLane : NetInfo.Lane
        {
            public NetworkSkinLane(NetInfo.Lane originalLane)
            {
                CopyProperties(this, originalLane);
                if (originalLane.m_laneProps != null)
                {
                    m_laneProps = UnityEngine.Object.Instantiate(originalLane.m_laneProps);
                    for (var i = 0; i < originalLane.m_laneProps.m_props.Length; i++)
                    {
                        m_laneProps.m_props[i] = new NetLaneProps.Prop();
                        CopyProperties(m_laneProps.m_props[i], originalLane.m_laneProps.m_props[i]);
                    }
                }
            }
        }

        // TODO check if m_lodValue is shared with the original segment. it should!
        private class NetworkSkinSegment : NetInfo.Segment
        {
            public NetworkSkinSegment(NetInfo.Segment originalSegment)
            {
                CopyProperties(this, originalSegment);

                if (originalSegment.m_segmentMaterial != null)
                {
                    m_segmentMaterial = UnityEngine.Object.Instantiate(originalSegment.m_segmentMaterial);
                }
            }
        }
    }
}
