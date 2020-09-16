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
            "Tram Arrow",

            //clus road arrows
            "2128887855.Arrow Forward 2_Data",
            "2128887855.Arrow Forward - Right_Data",
            "2128887855.Arrow Left 2_Data",
            "2128887855.Arrow Forward - Left_Data ",
            "2128887855.Arrow Forward - Right - Left_Data",
            "2128887855.Arrow - Right - Left_Data",
            "2128887855.Arrow Right 2_Data"
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
            "Random Street Prop NoParking",
            "Random Industrial Street Prop",
            "Delineator 01",
            "Delineator 02",
            "Bus Lane",
            "Bike Lane",
            "Road Decal Slow",
            "Manhole",
            
            //Vanilla Props used on Parklife Pathes as Roads by Chamëleon
            "Plant Pot 02",
            "Plant Pot 03",

            //clus road decals
            "1919886860.Be Aware Stop Ahead - Decal #01_Data",
            "1919886860.Be Aware Stop Ahead - Decal #02_Data",
            "1919886860.Bus Stop - Decal #01_Data",
            "1919886860.Trolley Bus - Decal_Data",
            "1919886860.Trolley Bus Only - Decal_Data",
            "1919886860.Platform Marking - Decal_Data",
            "1919886860.Shared Tram Lane - Decal  #02_Data",
            "1919886860.Tram Stop - Decal #01_Data",
            "1919886860.Tram Sign - Decal_Data",
            "1919886860.Tram Only - Decal_Data",
            "1919886860.Shared Tram Lane - Decal _Data",
            "1919886860.Zig Zag - Decal #01_Data",
            "1919886860.Bike Lane Arrow - Decal _Data",
            "1919886860.Bike Lane Arrow - Decal #02_Data",
            "1919886860.Bike Sign - Decal_Data",
            "1919886860.Public Transport - Decal_Data",
            "1919886860.Single Bike - Decal_Data",
            "1919886860.Bike Path (only Stripes)- Decal _Data",
            "1919886860.Bike Path - Decal_Data",
            "1919886860.Bike Path Green Painting - Decal_Data",
            "1919886860.Cars Parked Caution - Decal_Data",
            "1919886860.Do not pass - Decal_Data",
            "1919886860.Keep Clear - Decal_Data",
            "1919886860.Manhole - Decal #01_Data",
            "1919886860.Marking #01_Data",
            "1919886860.Marking #02_Data",
            "1919886860.No Stopping - Sign_Data",
            "1919886860.Striped Bike Path Decal #01_Data",
            "1919886860.Striped Bike Path Decal #02_Data",
            "1919886860.Striped Bike Path Decal #03_Data",
            "1919886860.Striped Bike Path Decal #04_Data",
            "1919886860.Striped Yellow Line - Decal_Data"
        };

        private static readonly string[] TransportStopNames = new[]
        {
            "Bus Stop Large",
            "Bus Stop Small",
            "Tram Stop",
            "Tram Stop Sign",
            "Trolleybus Stop",
            "Trolleybus Stop Large 01",
            "Sightseeing Bus Stop Small",
            "Sightseeing Bus Stop Large",
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
