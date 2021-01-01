using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace NetworkSkins.Net
{
    public static class StreetLightUtils
    {
        private const string AmericanSignReplacerWorkshopId = "1523608557";
        public static List<PropInfo> GetAvailableStreetLights()
        {
            var streetLights = new List<PropInfo>();

            var prefabCount = PrefabCollection<PropInfo>.LoadedCount();
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                var prefab = PrefabCollection<PropInfo>.GetLoaded(prefabIndex);
                if (IsStreetLightProp(prefab))
                {
                    streetLights.Add(prefab);
                }
            }

            streetLights.Sort((a, b) => string.Compare(a.GetUncheckedLocalizedTitle(), b.GetUncheckedLocalizedTitle(), StringComparison.Ordinal));

            return streetLights;
        }

        public static bool HasSingularStreetLight(NetInfo prefab) {
            if (prefab?.m_lanes == null) return false;

            NetLaneProps.Prop lastLaneProp = null;

            foreach (var lane in prefab.m_lanes) {
                var laneProps = lane?.m_laneProps?.m_props;
                if (laneProps == null) continue;

                foreach (var laneProp in laneProps) {
                    if (laneProp != null && IsStreetLightProp(laneProp.m_finalProp)) {
                        if(lastLaneProp != null && (lastLaneProp.m_finalProp != laneProp.m_finalProp 
                            || lastLaneProp.m_repeatDistance != laneProp.m_repeatDistance)) {
                            return false;
                        }
                        lastLaneProp = laneProp;
                    }
                }
            }

            return lastLaneProp != null;
        }

        [CanBeNull]
        public static PropInfo GetDefaultStreetLight(NetInfo prefab)
        {
            return NetUtils.GetMatchingLaneProp(prefab, laneProp => IsStreetLightProp(laneProp.m_finalProp))?.m_finalProp;
        }

        public static float GetDefaultRepeatDistance(NetInfo prefab)
        {
            return NetUtils.GetMatchingLaneProp(prefab, laneProp => IsStreetLightProp(laneProp.m_finalProp))?.m_repeatDistance ?? 40f;
        }

        public static bool IsStreetLightProp(PropInfo prefab)
        {
            if (prefab == null) return false;

            if (prefab.m_class.m_service == ItemClass.Service.Road ||
                prefab.m_class.m_subService == ItemClass.SubService.PublicTransportPlane ||
                prefab.name.ToLower().Contains("streetlamp") 
                || prefab.name.ToLower().Contains("streetlight") 
                || prefab.name.ToLower().Contains("lantern")
                || prefab.name.ToLower().Contains("street light"))
            {
                if (prefab.m_effects != null && prefab.m_effects.Length > 0)
                {

                    if (prefab.name.ToLower().Contains("taxiway")) return false;
                    if (prefab.name.ToLower().Contains("runway")) return false;

                    // American highway signs with integrated lights are NOT street lights!
                    if (prefab.name.StartsWith(AmericanSignReplacerWorkshopId)) return false;

                    // All props with the traffic light shader are NOT street lights
                    if (prefab.m_material.shader == Shader.Find("Custom/Props/Prop/TrafficLight")) return false;

                    foreach (var effect in prefab.m_effects)
                    {
                        if (effect.m_effect is LightEffect)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static void CorrectStreetLightPropAngleAndPosition(NetInfo info, NetLaneProps.Prop laneProp, float halfWidth, float lanePosition)
        {
            if (info.name.StartsWith("Nature Reserve Path 02") && laneProp.m_angle == -90) {
                laneProp.m_angle += 90;
            }

            // Rotate street lights standing on left side of pedestrian paths
            float propX = laneProp.m_position.x + lanePosition;
            if(propX < 0 && halfWidth + propX < 3f)
            {
                laneProp.m_angle = 180;
            }

            // some props are rotated the wrong way!
            if(laneProp.m_prop?.name == "Zoo Streetlight 01" || laneProp.m_prop?.name == "Nature Reserve Streetlight 01" || laneProp.m_prop?.name == "Amusement Park Streetlight 01") {
                laneProp.m_angle -= 90;
            }
        }
    }
}
