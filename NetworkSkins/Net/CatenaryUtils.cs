using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NetworkSkins.Net
{
    public static class CatenaryUtils
    {
        // Tram Pole Center
        // 
        private static readonly string[] TramPoleSideNames = new[]
        {
            "Tram Pole Side"
        };

        private static readonly string[] TramPoleCenterNames = new[]
{
            "Tram Pole Center"
        };

        private static readonly string[] DoubleCatenaryNames = new[]
        {
            "RailwayPowerline",
            "774449380.Catenary Type NL2A_Data",
            "774449380.Catenary Type NL2B_Data",
            "774449380.Catenary Type DE2A_Data",
            "774449380.Catenary Type PRR2A_Data",
            "774449380.Catenary Type PRR2B_Data",
            "774449380.Catenary Type JP2A_Data",
            "774449380.Catenary Type EXPO2A_Data",
            "2444511901.Catenary Type NL2A_Data",
            "2444511901.Catenary Type NL2B_Data",
            "2444511901.Catenary Type DE2A_Data",
            "2444511901.Catenary Type PRR2A_Data",
            "2444511901.Catenary Type PRR2B_Data",
            "2444511901.Catenary Type JP2A_Data",
            "2444511901.Catenary Type EXPO2A_Data",            
        };

        private static readonly string[] SingleCatenaryNames = new[]
        {
            "RailwayPowerline Singular",
            "774449380.Catenary Type NL1A_Data",
            "774449380.Catenary Type NL1B_Data",
            "774449380.Catenary Type DE1A_Data",
            "774449380.Catenary Type PRR 1A_Data",
            "774449380.Catenary Type PRR 1A_Data",
            "774449380.Catenary Type JP1A_Data",
            "774449380.Catenary Type EXPO1A_Data",
            "2444511901.Catenary Type NL1A_Data",
            "2444511901.Catenary Type NL1B_Data",
            "2444511901.Catenary Type DE1A_Data",
            "2444511901.Catenary Type PRR 1A_Data",
            "2444511901.Catenary Type PRR 1A_Data",
            "2444511901.Catenary Type JP1A_Data",
            "2444511901.Catenary Type EXPO1A_Data",            
        };

        private const string R69_DOUBLE_NORMAL = "cat2n";
        private const string R69_DOUBLE_END = "cat2e";
        private const string R69_DOUBLE_TUNNEL = "cat2t";
        private const string R69_SINGLE_NORMAL = "cat1n";
        private const string R69_SINGLE_END = "cat1e";
        private const string R69_SINGLE_TUNNEL = "cat1t";

        public static PropInfo GetDefaultNormalCatenary(NetInfo prefab)
        {
            return NetUtils.GetMatchingLaneProp(prefab, laneProp => IsNormalCatenaryProp(laneProp.m_finalProp))?.m_finalProp;
        }

        public static bool IsNormalCatenaryProp(PropInfo prop)
        {
            return IsDoubleRailNormalCatenaryProp(prop) || IsSingleRailNormalCatenaryProp(prop) || IsTramPoleSideProp(prop) || IsTramPoleCenterProp(prop);
        }

        public static bool IsDoubleRailNormalCatenaryProp(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(DoubleCatenaryNames, prop.name) != -1 || ParseR69RailwayType(prop, out var type) && type == R69_DOUBLE_NORMAL;
        }

        public static bool IsSingleRailNormalCatenaryProp(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(SingleCatenaryNames, prop.name) != -1 || ParseR69RailwayType(prop, out var type) && type == R69_SINGLE_NORMAL;
        }

        public static bool IsTramPoleSideProp(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(TramPoleSideNames, prop.name) != -1;
        }

        public static bool IsTramPoleCenterProp(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(TramPoleCenterNames, prop.name) != -1;
        }

        public static bool IsEndCatenaryProp(PropInfo prop)
        {
            return IsDoubleRailEndCatenaryProp(prop) || IsSingleRailEndCatenaryProp(prop);
        }

        public static bool IsDoubleRailEndCatenaryProp(PropInfo prop)
        {
            if (prop == null) return false;
            return ParseR69RailwayType(prop, out var type) && type == R69_DOUBLE_END;
        }

        public static bool IsSingleRailEndCatenaryProp(PropInfo prop)
        {
            if (prop == null) return false;
            return ParseR69RailwayType(prop, out var type) && type == R69_SINGLE_END;
        }

        public static bool IsTunnelCatenaryProp(PropInfo prop)
        {
            return IsDoubleRailTunnelCatenaryProp(prop) || IsSingleRailTunnelCatenaryProp(prop);
        }

        public static bool IsDoubleRailTunnelCatenaryProp(PropInfo prop)
        {
            if (prop == null) return false;
            return ParseR69RailwayType(prop, out var type) && type == R69_DOUBLE_TUNNEL;
        }

        public static bool IsSingleRailTunnelCatenaryProp(PropInfo prop)
        {
            if (prop == null) return false;
            return ParseR69RailwayType(prop, out var type) && type == R69_SINGLE_TUNNEL;
        }

        public static PropInfo GetEndCatenary(PropInfo prop)
        {
            if (ParseR69RailwayTag(prop, out var type, out var styleName, out var subStyleName, out var variationName))
            {
                if(type == R69_DOUBLE_NORMAL)
                {
                    return GetR69RailwayPropWithFallback(R69_DOUBLE_END, styleName, subStyleName, variationName) ?? prop;
                }
                else if(type == R69_SINGLE_NORMAL)
                {
                    return GetR69RailwayPropWithFallback(R69_SINGLE_END, styleName, subStyleName, variationName) ?? prop;
                }
            }

            return prop;
        }

        public static PropInfo GetTunnelCatenary(PropInfo prop)
        {
            if (ParseR69RailwayTag(prop, out var type, out var styleName, out var subStyleName, out var variationName))
            {
                if (type == R69_DOUBLE_NORMAL)
                {
                    return GetR69RailwayPropWithFallback(R69_DOUBLE_TUNNEL, styleName, subStyleName, variationName);
                }
                else if (type == R69_SINGLE_NORMAL)
                {
                    return GetR69RailwayPropWithFallback(R69_SINGLE_TUNNEL, styleName, subStyleName, variationName);
                }
            }

            return null;
        }

        public static void CorrectCatenaryPropAngleAndPosition(NetLaneProps.Prop laneProp)
        {
            var prop = laneProp.m_finalProp;
            if (prop == null) return;

            // Vanilla catenaries
            if(prop.name == "RailwayPowerline")
            {
                laneProp.m_angle = 0f;
                //laneProp.m_position.y = -0.15f;
            }
            else if (prop.name == "RailwayPowerline Singular")
            {
                laneProp.m_angle = 0f;
                laneProp.m_position.x = 2f;
                //laneProp.m_position.y = -0.15f;
            }

            // Tim's catenaries
            else if (prop.name.StartsWith("774449380"))
            {
                //laneProp.m_position.y = -0.15f;
                laneProp.m_angle = 180f;

                if(Array.IndexOf(SingleCatenaryNames, prop.name) != -1)
                {
                    laneProp.m_position.x = 2f;
                }
            }
            
              // cylis's catenaries
            else if (prop.name.StartsWith("2444511901"))
            {
                //laneProp.m_position.y = -0.15f;
                laneProp.m_angle = 180f;

                if(Array.IndexOf(SingleCatenaryNames, prop.name) != -1)
                {
                    laneProp.m_position.x = 2f;
                }
            }         

            // Ronyx Railway catenaries
            else if(ParseR69RailwayType(prop, out var type))
            {
                //laneProp.m_position.y = 0.09f;

                if (type == R69_DOUBLE_NORMAL)
                {
                    laneProp.m_angle = 0f;
                }
                else if (type == R69_SINGLE_NORMAL)
                {
                    laneProp.m_angle = 180f;
                    laneProp.m_position.x = 0f;
                }
                else if (type == R69_DOUBLE_END)
                {
                    laneProp.m_angle = (laneProp.m_segmentOffset == -1f) ? 180f : 0f;
                }
                else if (type == R69_SINGLE_END)
                {
                    laneProp.m_angle = (laneProp.m_segmentOffset == -1f) ? 180f : 0f;
                    laneProp.m_position.x = 0f;
                }
            }
        }

        public static bool IsWireSegment(NetInfo.Segment segment)
        {
            return segment?.m_material?.shader?.name == "Custom/Net/Electricity";
        }

        // Fallback to variationName = null, then subStyleName = null if no prop has been found
        private static PropInfo GetR69RailwayPropWithFallback(string type, string styleName, string subStyleName, string variationName)
        {
            var prefab = GetR69RailwayProp(type, styleName, subStyleName, variationName);
            if (prefab != null) return prefab;

            if (variationName != null)
            {
                prefab = GetR69RailwayProp(type, styleName, subStyleName, null);
                if (prefab != null) return prefab;
            }

            if (subStyleName != null)
            {
                prefab = GetR69RailwayProp(type, styleName, null, null);
                if (prefab != null) return prefab;
            }

            return null;
        }

        private static PropInfo GetR69RailwayProp(string type, string styleName, string subStyleName, string variationName)
        {
            var prefabCount = PrefabCollection<PropInfo>.LoadedCount();
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                var prefab = PrefabCollection<PropInfo>.GetLoaded(prefabIndex);
                if (ParseR69RailwayTag(prefab, out var prefabType, out var prefabStyleName, out var prefabSubStyleName, out var prefabVariationName)
                    && prefabType == type && prefabStyleName == styleName && prefabSubStyleName == subStyleName && prefabVariationName == variationName)
                {
                    return prefab;
                }
            }

            return null;
        }

        private static bool ParseR69RailwayType(PropInfo prop, out string type)
        {
            return ParseR69RailwayTag(prop, out type, out var styleName, out string subStyleName, out string variationName);
        }

        private static bool ParseR69RailwayTag(PropInfo prop, out string type, out string styleName, out string subStyleName, out string variationName)
        {
            // Allowed tag layouts:
            // r69rwp-cat2n#STYLE#
            // r69rwp-cat2n#STYLE-SUBSTYLE#
            // r69rwp-cat2n#STYLE-SUBSTYLE (VARIATION)#
            // Also see: https://gist.github.com/ronyx69/b4cd41742803eaeac6d19322384a0f4a

            if (prop?.m_material?.name != null)
            {
                // do not match end of string because Unity might have appended extra text like " (Instance)" 
                var match = Regex.Match(prop.m_material.name, @"\Ar69rwp-([a-z0-9]+)#(.+?)(-.+?)?( \(.+\))?#");
                if (match.Success)
                {
                    type = match.Groups[1].Value;
                    styleName = match.Groups[2].Value;
                    subStyleName = match.Groups[3].Success ? match.Groups[3].Value : null;
                    variationName = match.Groups[4].Success ? match.Groups[4].Value : null;
                    return true;
                }
            }

            type = null;
            styleName = null;
            subStyleName = null;
            variationName = null;
            return false;
        }

        public static bool IsCatenaryPropVisibeInUI(PropInfo prop)
        {
            return prop == null || !prop.name.StartsWith("1530376523"); // don't show R69 default (invisible) catenary
        }
    }
}
