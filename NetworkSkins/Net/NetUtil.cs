using System.Linq;
using UnityEngine;

namespace NetworkSkins.Net
{
    public class NetUtil
    {
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
