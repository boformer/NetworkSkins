using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Net {
    public static class RoadDecorationUtils {
        private static readonly string[] ArrowNames = new[]
        {
            "Road Arrow F",
            "Road Arrow FR",
            "Road Arrow L",
            "Road Arrow LF",
            "Road Arrow LFR",
            "Road Arrow LR",
            "Road Arrow R",
            "Tram Arrow"
        };
        private static readonly string[] SignNames = new[]
        {
            "30 Speed Limit",
            "40 Speed Limit",
            "50 Speed Limit",
            "60 Speed Limit",
            "100 Speed Limit",
            "Motorway Overroad Signs",
            "Motorway Sign",
            "No Left Turn Sign",
            "No Parking Sign",
            "No Right Turn Sign",
            "Street Name Sign"
        };

        private static readonly string[] DecorationNames = new[]
        {
            "Electricity Box",
            "Fire Hydrant",
            "Info Terminal",
            "Parking Meter",
            "Random Street Prop",
            "Random Industrial Street Prop",
            "Delineator 01",
            "Delineator 02",
            "Manhole"
        };

        private static readonly string[] TransportStopNames = new[]
        {
            "Bus Stop Large",
            "Bus Stop Small",
            "Tram Stop",
            "Tram Stop Sign",
            "Trolleybus Stop",
            "Trolleybus Stop Large 01",
            "Taxiway Sign 01",
            "Taxiway Sign 02"
        };

        private static readonly string[] TrafficLightNames = new[]
        {
            "Traffic Light 01",
            "Traffic Light 02",
            "Traffic Light 01 Mirror",
            "Traffic Light 02 Mirror",
            "Traffic Light Pedestrian",
            "Traffic Light European 01",
            "Traffic Light European 02",
            "Traffic Light European 01 Mirror",
            "Traffic Light European 02 Mirror",
            "Traffic Light Pedestrian European"
        };

        public static bool IsArrow(PropInfo prop) {
            if (prop == null) return false;
            return Array.IndexOf(ArrowNames, prop.name) != -1;
        }

        public static bool IsSign(PropInfo prop) {
            if (prop == null) return false;
            return Array.IndexOf(SignNames, prop.name) != -1;
        }

        public static bool IsDecoration(PropInfo prop) {
            if (prop == null) return false;
            return Array.IndexOf(DecorationNames, prop.name) != -1;
        }

        public static bool IsTransportStop(PropInfo prop) {
            if (prop == null) return false;
            return Array.IndexOf(TransportStopNames, prop.name) != -1;
        }
        
        public static bool IsTrafficLight(PropInfo prop) {
            if (prop == null) return false;
            return Array.IndexOf(TrafficLightNames, prop.name) != -1;
        }
    }
}
