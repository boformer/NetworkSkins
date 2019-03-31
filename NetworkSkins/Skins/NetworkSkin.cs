using System;
using System.Collections.Generic;
using System.Reflection;
using NetworkSkins.Net;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Skins
{
    public class NetworkSkin
    {
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

        public readonly NetInfo Prefab;
        public readonly List<NetworkSkinModifier> Modifiers = new List<NetworkSkinModifier>();

        public NetworkSkin(NetInfo prefab)
        {
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

            Prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
        }

        // TODO make sure that this is called when a skin is no longer needed
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
        }

        public void ApplyModifier(NetworkSkinModifier modifier)
        {
            modifier.Apply(this);
            Modifiers.Add(modifier);
        }

        // Updates a lane prop without affecting the original lane props of the network
        public void UpdateLaneProp(int laneIndex, int propIndex, Action<NetLaneProps.Prop> updater)
        {
            if (m_lanes.Length <= laneIndex)
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

            var prop = lane.m_laneProps.m_props[propIndex];
            if (prop == null)
            {
                Debug.LogError($"Prop {propIndex} is null for prefab {Prefab}, lane {laneIndex}!");
                return;
            }

            if (!(lane is NetworkSkinLane))
            {
                lane = new NetworkSkinLane(lane);
                m_lanes[laneIndex] = lane;
            }

            if (!(prop is NetworkSkinLaneProp))
            {
                prop = new NetworkSkinLaneProp(prop);
                lane.m_laneProps.m_props[propIndex] = prop;
            }

            updater(prop);
        }

        public void RemoveLaneProp(int laneIndex, int propIndex)
        {
            if (m_lanes.Length <= laneIndex)
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

            if (!(lane is NetworkSkinLane))
            {
                lane = new NetworkSkinLane(lane);
                m_lanes[laneIndex] = lane;
            }

            var props = new List<NetLaneProps.Prop>(lane.m_laneProps.m_props);
            props.RemoveAt(propIndex);
            lane.m_laneProps.m_props = props.ToArray();
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
            if (m_segments == null)
            {
                return;
            }

            foreach (var segment in m_segments)
            {
                if (segment.m_material?.shader?.name == "Custom/Net/Electricity")
                {
                    m_hasWires = true;
                    break;
                }
            }

        }

        public override string ToString()
        {
            return $"Skin for {Prefab.name}";
        }

        internal class NetworkSkinLane : NetInfo.Lane
        {
            public NetworkSkinLane(NetInfo.Lane originalLane)
            {
                CopyProperties(this, originalLane);
                if (originalLane.m_laneProps != null)
                {
                    m_laneProps = UnityEngine.Object.Instantiate(originalLane.m_laneProps);
                }
            }
        }

        internal class NetworkSkinLaneProp : NetLaneProps.Prop
        {
            public NetworkSkinLaneProp(NetLaneProps.Prop originalProp)
            {
                CopyProperties(this, originalProp);
            }
        }

        public static void CopyProperties(object target, object origin)
        {
            var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                fieldInfo.SetValue(target, fieldInfo.GetValue(origin));
            }
        }
    }
}
