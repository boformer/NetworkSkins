using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace NetworkSkins.Net
{
    public static class NetUtil
    {
        public static readonly string[] NET_TYPE_NAMES = { "Tunnel", "Ground", "Elevated", "Bridge" };

        // Get the specific variations of a network for tunnel, ground, elevated and bridge.
        // Each network type has a distint AI, which stores this info.
        // To support other network types (e.g. monorail), you have to add them here.
        // Otherwise the NS settings window will not show up!
        public static NetInfo[] GetSubPrefabs(NetInfo prefab) {
            var subPrefabs = new NetInfo[NET_TYPE_NAMES.Length];

            if (prefab.m_netAI is TrainTrackAI trackAI) {
                subPrefabs[(int)NetType.Tunnel] = trackAI.m_tunnelInfo;
                subPrefabs[(int)NetType.Ground] = trackAI.m_info;
                subPrefabs[(int)NetType.Elevated] = trackAI.m_elevatedInfo;
                subPrefabs[(int)NetType.Bridge] = trackAI.m_bridgeInfo;
            } else if (prefab.m_netAI is RoadAI roadAI) {
                subPrefabs[(int)NetType.Tunnel] = roadAI.m_tunnelInfo;
                subPrefabs[(int)NetType.Ground] = roadAI.m_info;
                subPrefabs[(int)NetType.Elevated] = roadAI.m_elevatedInfo;
                subPrefabs[(int)NetType.Bridge] = roadAI.m_bridgeInfo;
            } else if (prefab.m_netAI is PedestrianPathAI pedAI) {
                subPrefabs[(int)NetType.Tunnel] = pedAI.m_tunnelInfo;
                subPrefabs[(int)NetType.Ground] = pedAI.m_info;
                subPrefabs[(int)NetType.Elevated] = pedAI.m_elevatedInfo;
                subPrefabs[(int)NetType.Bridge] = pedAI.m_bridgeInfo;
            } else if (prefab.m_netAI is PedestrianWayAI pedWayAI) {
                subPrefabs[(int)NetType.Tunnel] = pedWayAI.m_tunnelInfo;
                subPrefabs[(int)NetType.Ground] = pedWayAI.m_info;
                subPrefabs[(int)NetType.Elevated] = pedWayAI.m_elevatedInfo;
                subPrefabs[(int)NetType.Bridge] = pedWayAI.m_bridgeInfo;
            }
            return subPrefabs;
        }

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

        public static bool HasTrees(NetInfo netInfo) {
            if (netInfo == null || netInfo.m_lanes == null) return false;
            foreach (NetInfo.Lane lane in netInfo.m_lanes)
                if (lane?.m_laneProps?.m_props != null)
                    foreach (var laneProp in lane.m_laneProps.m_props) {
                        if (laneProp?.m_finalTree != null) return true;
                    }
            return false;
        }

        public static bool HasTreesInLane(NetInfo netInfo, LanePosition position) {
            if (netInfo == null || netInfo.m_lanes == null) return false;
            foreach (var lane in netInfo.m_lanes)
                if (lane?.m_laneProps?.m_props != null && position.IsCorrectSide(lane.m_position))
                    foreach (var laneProp in lane.m_laneProps.m_props) {
                        if (laneProp?.m_finalTree != null) return true;
                    }
            return false;
        }

        internal static bool CanHaveNoneSurface(NetInfo prefab) {
            // TODO
            return false;
        }

        public static TreeInfo GetDefaultTree(NetInfo netInfo, LanePosition position) {
            if (netInfo == null || netInfo.m_lanes == null) return null;
            foreach (var lane in netInfo.m_lanes)
                if (lane?.m_laneProps?.m_props != null && position.IsCorrectSide(lane.m_position))
                    foreach (var laneProp in lane.m_laneProps.m_props) {
                        if (laneProp?.m_finalTree != null) return laneProp.m_finalTree;
                    }
            return null;
        }

        public static bool IsColorable(NetInfo netInfo) {
            if (netInfo != null && netInfo.m_class.m_service == ItemClass.Service.Road) {
                Texture2D texture = netInfo.m_segments[0].m_material.GetTexture("_APRMap") as Texture2D;
                if (texture != null) {
                    Color[] pixels = texture.MakeReadable().GetPixels();
                    for (int i = 0; i < pixels.Length; i++) {
                        if (pixels[i].b > 0.0f) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool HasCatenaries(NetInfo netInfo) {
            return netInfo != null && netInfo.m_class.m_service == ItemClass.Service.PublicTransport 
            && netInfo.m_class.m_subService == ItemClass.SubService.PublicTransportTrain;
        }

        public static bool HasPillars(NetInfo netInfo) {
            if (netInfo == null) return false;
            if (netInfo.m_netAI is RoadAI roadAI) {
                return roadAI.m_elevatedInfo.m_netAI is RoadBridgeAI;
            }
            if (netInfo.m_netAI is TrainTrackAI trainTrackAI) {
                return trainTrackAI.m_elevatedInfo.m_netAI is TrainTrackBridgeAI;
            }
            if (netInfo.m_netAI is PedestrianPathAI pedestrianAI) {
                return pedestrianAI.m_elevatedInfo.m_netAI is PedestrianBridgeAI;
            }
            return netInfo.m_netAI is MonorailTrackAI;
        }

        public static bool HasSurfaces(NetInfo netInfo) {
            // TODO
            return true;
        }

        public static bool HasStreetLights(NetInfo netInfo) {
            if (netInfo == null || netInfo.m_lanes == null) return false;

            foreach (var lane in netInfo.m_lanes)
                if (lane?.m_laneProps?.m_props != null)
                    foreach (var laneProp in lane.m_laneProps.m_props) {
                        if (laneProp?.m_finalProp != null && IsStreetLight(laneProp.m_finalProp)) return true;
                    }

            return false;
        }

        public static bool IsStreetLight(PropInfo propInfo) {
            if (propInfo == null) return false;
            if (propInfo.m_class.m_service == ItemClass.Service.Road 
            || propInfo.m_class.m_subService == ItemClass.SubService.PublicTransportPlane 
            || propInfo.name.ToLower().Contains("streetlamp") 
            || propInfo.name.ToLower().Contains("streetlight") 
            || propInfo.name.ToLower().Contains("lantern")) {
                if (propInfo.m_effects != null && propInfo.m_effects.Length > 0) {
                    if (propInfo.name.ToLower().Contains("taxiway")) return false;
                    if (propInfo.name.ToLower().Contains("runway")) return false;
                    if (propInfo.m_effects.Where(effect => effect.m_effect != null).Any(effect => effect.m_effect is LightEffect)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
