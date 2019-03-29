using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Skins
{
    public class NetworkSkin
    {
        public NetInfo.Lane[] m_lanes;

        public bool m_createPavement;
        public bool m_createGravel;
        public bool m_createRuining;
        public bool m_clipTerrain;

        public Color m_color;

        public readonly NetInfo Prefab;
        public readonly List<NetworkSkinModifier> Modifiers = new List<NetworkSkinModifier>();

        public NetworkSkin(NetInfo prefab)
        {
            if (prefab.m_lanes != null)
            {
                m_lanes = new NetInfo.Lane[prefab.m_lanes.Length];
                Array.Copy(prefab.m_lanes, m_lanes, m_lanes.Length);
            }
            m_createPavement = prefab.m_createPavement;
            m_createGravel = prefab.m_createGravel;
            m_createRuining = prefab.m_createRuining;
            m_clipTerrain = prefab.m_clipTerrain;
            m_color = prefab.m_color;

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
