using System;

// TODO add support for railway

namespace NetworkSkins.Net
{
    public static class CatenaryUtils
    {
        private static readonly string[] DoubleCatenaryNames = new[]
        {
            "RailwayPowerline",
            "774449380.Catenary Type NL2A",
            "774449380.Catenary Type NL2B",
            "774449380.Catenary Type DE2A",
            "774449380.Catenary Type PRR2A",
            "774449380.Catenary Type PRR2B",
            "774449380.Catenary Type JP2A",
            "774449380.Catenary Type EXPO2A",
        };

        private static readonly string[] SingleCatenaryNames = new[]
        {
            "RailwayPowerline Singular",
            "774449380.Catenary Type NL1A",
            "774449380.Catenary Type NL1B",
            "774449380.Catenary Type DE1A",
            "774449380.Catenary Type PRR 1A",
            "774449380.Catenary Type PRR 1A",
            "774449380.Catenary Type JP1A",
            "774449380.Catenary Type EXPO1A",
        };

        public static bool IsCatenaryProp(PropInfo prop)
        {
            return IsDoubleRailCatenaryProp(prop) || IsSingleRailCatenaryProp(prop);
        }

        public static bool IsDoubleRailCatenaryProp(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(DoubleCatenaryNames, prop.name) != -1;
        }

        public static bool IsSingleRailCatenaryProp(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(SingleCatenaryNames, prop.name) != -1;
        }

        public static bool IsWireSegment(NetInfo.Segment segment)
        {
            return segment?.m_material?.shader?.name == "Custom/Net/Electricity";
        }
    }
}
