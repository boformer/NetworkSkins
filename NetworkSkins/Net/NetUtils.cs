using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NetworkSkins.Net
{
    public static class NetUtils
    {
        // Get the specific variations of a network for tunnel, ground, elevated and bridge.
        // Each network type has a distint AI, which stores this info.
        // To support other network types (e.g. monorail), you have to add them here.
        // Otherwise the NS settings window will not show up!
        public static HashSet<NetInfo> GetPrefabVariations(NetInfo prefab)
        {
            var prefabs = new HashSet<NetInfo> {prefab};

            var slopePrefab = GetSlopePrefab(prefab);
            if (slopePrefab != null) prefabs.Add(slopePrefab);

            var elevatedPrefab = GetElevatedPrefab(prefab);
            if (elevatedPrefab != null) prefabs.Add(elevatedPrefab);

            var bridgePrefab = GetBridgePrefab(prefab);
            if (bridgePrefab != null) prefabs.Add(bridgePrefab);

            return prefabs;
        }

        [CanBeNull]
        public static NetInfo GetSlopePrefab(NetInfo prefab)
        {
            if (prefab.m_netAI is RoadAI roadAi)
            {
                return roadAi.m_slopeInfo;
            }
            else if (prefab.m_netAI is TrainTrackAI trainTrackAi)
            {
                return trainTrackAi.m_slopeInfo;
            }
            else if (prefab.m_netAI is PedestrianPathAI pathAi)
            {
                return pathAi.m_slopeInfo;
            }
            else if (prefab.m_netAI is PedestrianWayAI wayAi)
            {
                return wayAi.m_slopeInfo;
            }
            else
            {
                return null;
            }
        }

        [CanBeNull]
        public static NetInfo GetElevatedPrefab(NetInfo prefab)
        {
            if (prefab.m_netAI is RoadAI roadAi)
            {
                return roadAi.m_elevatedInfo;
            }
            else if (prefab.m_netAI is RoadBridgeAI)
            {
                return prefab;
            }
            else if (prefab.m_netAI is TrainTrackAI trainTrackAi)
            {
                return trainTrackAi.m_elevatedInfo;
            }
            else if (prefab.m_netAI is TrainTrackBridgeAI)
            {
                return prefab;
            }
            else if (prefab.m_netAI is PedestrianPathAI pathAi)
            {
                return pathAi.m_elevatedInfo;
            }
            else if (prefab.m_netAI is PedestrianWayAI wayAi)
            {
                return wayAi.m_elevatedInfo;
            }
            else if (prefab.m_netAI is PedestrianBridgeAI)
            {
                return prefab;
            }
            else if (prefab.m_netAI is MonorailTrackAI)
            {
                return prefab;
            }
            else
            {
                return null;
            }
        }

        [CanBeNull]
        public static NetInfo GetBridgePrefab(NetInfo prefab)
        {
            if (prefab.m_netAI is RoadAI roadAi)
            {
                return roadAi.m_bridgeInfo;
            }
            else if (prefab.m_netAI is TrainTrackAI trainTrackAi)
            {
                return trainTrackAi.m_bridgeInfo;
            }
            else if (prefab.m_netAI is PedestrianPathAI pathAi)
            {
                return pathAi.m_bridgeInfo;
            }
            else if (prefab.m_netAI is PedestrianWayAI wayAi)
            {
                return wayAi.m_bridgeInfo;
            }
            else
            {
                return null;
            }
        }

        [CanBeNull]
        public static NetLaneProps.Prop GetMatchingLaneProp(NetInfo prefab, Func<NetLaneProps.Prop, bool> matcher, LanePosition? position = null)
        {
            if (prefab?.m_lanes == null) return null;

            foreach (var lane in prefab.m_lanes)
            {
                if(position != null && !position.Value.IsCorrectSide(lane.m_position)) continue;

                var laneProps = lane?.m_laneProps?.m_props;
                if (laneProps == null) continue;

                foreach (var laneProp in laneProps)
                {
                    var finalProp = laneProp?.m_finalProp;
                    if (laneProp != null && matcher(laneProp))
                    {
                        return laneProp;
                    }
                }
            }

            return null;
        }
    }
}
