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

            //Australian replacer Arrows
            "2071057587.AU Arrow Right_Data",
            "2071057587.AU Arrow Left_Data",
            "2071057587.AU Arrow Straight_Data",
            "2071057587.AU Arrow Straight Left_Data",
            "2071057587.AU Arrow Straight Right_Data",
            
            //BIG Urban Roads Arrows
            "2228643473.BIG Urban Roads Decal Arrow R_Data",
            "2228643473.BIG Urban Roads Decal Arrow L_Data",
            "2228643473.BIG Urban Roads Decal Arrow F_Data",
            "2228643473.BIG Urban Roads Decal Arrow FL_Data",
            "2228643473.BIG Urban Roads Decal Arrow FR_Data",            

            //clus road arrows
            "2128887855.Arrow Forward 2_Data",
            "2128887855.Arrow Forward - Right_Data",
            "2128887855.Arrow Left 2_Data",
            "2128887855.Arrow Forward - Left_Data",
            "2128887855.Arrow Forward - Right - Left_Data",
            "2128887855.Arrow - Right - Left_Data",
            "2128887855.Arrow Right 2_Data",

            //Danish replacer Arrows
            "2071058348.DK Arrow Right_Data",
            "2071058348.DK Arrow Left_Data",
            "2071058348.DK Arrow Straight_Data",
            "2071058348.DK Arrow Straight Left_Data",
            "2071058348.DK Arrow Straight Right_Data",

            //French replacer Arrows
            "1083162158.FR Arrow Right_Data",
            "1083162158.FR Arrow Left_Data",
            "1083162158.FR Arrow Straight_Data",
            "1083162158.FR Arrow Straight Left_Data",
            "1083162158.FR Arrow Straight Right_Data",

            //German replacer Arrows
            "1866391549.DE Arrow Right_Data",
            "1866391549.DE Arrow Left_Data",
            "1866391549.DE Arrow Straight_Data",
            "1866391549.DE Arrow Straight Left_Data",
            "1866391549.DE Arrow Straight Right_Data",

            //Japanese replacer Arrows
            "1568009578.JP Arrow Right_Data",
            "1568009578.JP Arrow Left_Data",
            "1568009578.JP Arrow Forward_Data",
            "1568009578.JP Arrow Forward Left_Data",
            "1568009578.JP Arrow Forward Right_Data",
            "1568009578.JP Arrow Left Right_Data",

            //Korean replacer Arrows
            "1787627069.tdb Right Arrow_Data",
            "1787627069.tdb Left Arrow_Data",
            "1787627069.tdb Straight Arrow_Data",
            "1787627069.tdb Left Straight Arrow_Data",
            "1787627069.tdb Right Straight Arrow_Data",

            //Malaysian replacer Arrows
            "2013521990.MY Arrow Right 2_Data",
            "2013521990.MY Arrow Left 2_Data",
            "2013521990.MY Arrow Straight_Data",
            "2013521990.MY Arrow Straight Left_Data",
            "2013521990.MY Arrow Straight Right_Data",

            //Spain replacer Arrows
            "2008960441.Spanish Arrow Forward_Data",
            "2008960441.Spanish Arrow Forward Left_Data",
            "2008960441.Spanish Arrow Forward Left Right_Data",
            "2008960441.Spanish Arrow Forward Right_Data",
            "2008960441.Spanish Arrow Left_Data",
            "2008960441.Spanish Arrow Left Right_Data",
            "2008960441.Spanish Arrow Right_Data",

            //USRP replacer Arrows
            "2088550283.USRP Arrow Right_Data",
            "2088550283.USRP Arrow Left_Data",
            "2088550283.USRP Arrow Forward_Data",
            "2088550283.USRP Arrow Left-Forward_Data",
            "2088550283.USRP Arrow Forward-Right_Data"
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
            "1919886860.Striped Yellow Line - Decal_Data",

            //clus warning lights
            "1919887701.Warning Light #01_Data"
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
            "Taxiway Sign 02",

            //clus stops
            "1919887701.Bus Shelter #01 - Frame_Data",
            "1919887701.Bus Shelter 01 - Glass_Data",
            "1919887701.Bus Shelter without Time Table_Data",
            "1919887701.Tram Shelter #01 - Frame_Data",
            "1919887701.Tram Shelter #01 - Glass _Data",
            "1919887701.Tram Shelter #03 - Frame_Data",
            "1919887701.Tram Shelter #03 - Glass_Data",
            "1919887701.Tram Shelter #04 - Frame_Data",
            "1919887701.Tram Shelter #04 - Glass_Data",

            //Pewex stops (used also for clus stops)
            "1743534472.Bus Stop Sign_Data",
            "1743534472.Tram Stop Sign_Data"

        };

        //private static readonly string[] TrafficLightNames = new[]
        //{
        //    "Traffic Light 01",
        //    "Traffic Light 02",
        //    "Traffic Light 01 Mirror",
        //    "Traffic Light 02 Mirror",
        //    "Traffic Light Pedestrian",
        //    "Traffic Light European 01",
        //    "Traffic Light European 02",
        //    "Traffic Light European 01 Mirror",
        //    "Traffic Light European 02 Mirror",
        //    "Traffic Light Pedestrian European",

        //    //BIG Roads Traffic Lights (old) [need to be added for compability]


        //    //BIG Urban Traffic Lights
        //    "2236570542.BIG Roads TL Single_Data",
        //    "2236570542.BIG Roads TL 1L+P_Data",
        //    "2236570542.BIG Roads TL 1L+P 1Way_Data",
        //    "2236570542.BIG Roads TL 2L+P_Data",
        //    "2236570542.BIG Roads TL 3L+P_Data",
        //    "2236570542.BIG Roads TL 3L+P Doghouse_Data",
        //    "2236570542.BIG Roads TL Pedestrians_Data",
        //    "2236570542.BIG Roads TL Pedestrians Mirror_Data",

        //    //BIG Suburban Traffic Lights (will be released soon)


        //    //clus traffic lights
        //    "2032407437.Traffic Light - TLB_01_Data",
        //    "2032407437.Traffic Light - TLBUS_B1_Data",
        //    "2032407437.Traffic Light - TLBUS_B2I_Data",
        //    "2032407437.Traffic Light - TLBUS_B2IM_Data",
        //    "2032407437.Traffic Light - TLBUS B3N1_Data",
        //    "2032407437.Traffic Light - TLBUS B3N1M_Data",
        //    "2032407437.Traffic Light - TLBUS_TllExchang_Data",
        //    "2032407437.Traffic Light - TLBUS_TllExch A_Data",
        //    "2032407437.Traffic Light - TLBUS_TllExch AM_Data",
        //    "2032407437.Traffic Light - TLBUS_TllExch M_Data",
        //    "2032407437.Traffic Light - TLD_D1_Data",
        //    "2032407437.Traffic Light - TLD_D1M_Data",
        //    "2032407437.Traffic Light - TLD_D2_Data",
        //    "2032407437.Traffic Light - TLD_D2M_Data",
        //    "2032407437.Traffic Light - TLD_D3_Data",
        //    "2032407437.Traffic Light - TLD_D3M_Data",
        //    "2032407437.Traffic Light - TLD_D4_Data",
        //    "2032407437.Traffic Light - TLD_D5_Data",
        //    "2032407437.Traffic Light - TLD_D5M_Data",
        //    "2032407437.Traffic Light - TLI_I01_Data",
        //    "2032407437.Traffic Light - TLI_I01M_Data",
        //    "2032407437.Traffic Light - TLI_I02_Data",
        //    "2032407437.Traffic Light - TLI_I02M_Data",
        //    "2032407437.Traffic Light - TLI_I03_Data",
        //    "2032407437.Traffic Light - TLI_I03M_Data",
        //    "2032407437.Traffic Light - TLI_I04_Data",
        //    "2032407437.Traffic Light - TLI_I04M_Data",
        //    "2032407437.Traffic Light - TLI_I05_Data",
        //    "2032407437.Traffic Light - TLI_I05M_Data",
        //    "2032407437.Traffic Light - TLM_Mhe1_Data",
        //    "2032407437.Traffic Light - TLM_Mhe1M_Data",
        //    "2032407437.Traffic Light - TLM_Mhe2_Data",
        //    "2032407437.Traffic Light - TLM_Mhe2M_Data",
        //    "2032407437.Traffic Light - TLM_Mhe3_Data",
        //    "2032407437.Traffic Light - TLM_Mhe3M_Data",
        //    "2032407437.Traffic Light - TLM_Mhe4_Data",
        //    "2032407437.Traffic Light - TLM_Mhe4M_Data",
        //    "2032407437.Traffic Light - TLM_Mhe5_Data",
        //    "2032407437.Traffic Light - TLM_Mhe5M_Data",
        //    "2032407437.Traffic Light - TLM_Mhe6_Data",
        //    "2032407437.Traffic Light - TLM_Mhe6M_Data",
        //    "2032407437.Traffic Light - TLP_P1_Data",
        //    "2032407437.Traffic Light - TLP_P2_Data",
        //    "2032407437.Traffic Light - TLP_P5_Data",
        //    "2032407437.Traffic Light - TLPedDBL_H_Data",
        //    "2032407437.Traffic Light - TLPedDBL_L_Data",
        //    "2032407437.Traffic Light - TLS_S1_Data",
        //    "2032407437.Traffic Light - TLS_S1M_Data",
        //    "2032407437.Traffic Light - TLS_S2_Data",
        //    "2032407437.Traffic Light - TLS_S2M_Data",
        //    "2032407437.Traffic Light - TLS_S3_Data",
        //    "2032407437.Traffic Light - TLS_S3M_Data",
        //    "2032407437.Traffic Light - TLS_S4_Data",
        //    "2032407437.Traffic Light - TLS_S4M_Data",
        //    "2032407437.Traffic Light - TLS_S5_Data",
        //    "2032407437.Traffic Light - TLS_S5M_Data",
        //    "2032407437.Traffic Light - TLS_S6_Data",
        //    "2032407437.Traffic Light - TLS_S7_Data",
        //    "2032407437.Traffic Light - TLT_T1_Data",
        //    "2032407437.Traffic Light - TLT_T1M_Data",
        //    "2032407437.Traffic Light - TLT_T2_Data",
        //    "2032407437.Traffic Light - TLT_T2M_Data",
        //    "2032407437.Traffic Light - TLT_T3_Data",
        //    "2032407437.Traffic Light - TLT_T3M_Data",
        //    "2032407437.Traffic Light - TLT_T4_Data",
        //    "2032407437.Traffic Light - TLT_T4M_Data",
        //    "2032407437.Traffic Light - TLT_T5_Data",
        //    "2032407437.Traffic Light - TLT_T5M_Data",
        //    "2032407437.Traffic Light - TLT_T6_Data",
        //    "2032407437.Traffic Light - TLT_T6M_Data",
        //    "2032407437.Traffic Light - TLT_T7_Data",
        //    "2032407437.Traffic Light - TLT_T7M_Data",
        //    "2032407437.Traffic Light - TLT_T8_Data",
        //    "2032407437.Traffic Light - TLT_T8M_Data",
        //    "2032407437.Traffic Light - TLTRAM_T1_Data",
        //    "2032407437.Traffic Light - TLTRAM_T1M_Data",
        //    "2032407437.Traffic Light - TLTRAM_T2_Data",
        //    "2032407437.Traffic Light - TLTRAM_T2M_Data",

        //   //Nouvilas Madrid Traffic Lights (will be released soon)

        //    "2270587845.Madrid TrafficLights M blink_Data",
        //    "2270587845.Madrid TrafficLights P Left_Data",
        //    "2270587845.Madrid TrafficLights P Right_Data",
        //    "2270587845.Madrid Traffic Lights - MSP Left_Data",
        //    "2270587845.Madrid Traffic Lights - MSP Righ_Data",
        //    "2270587845.Madrid TrafficLights LMSP Left_Data",
        //    "2270587845.Madrid TrafficLights LMSP Right_Data",
        //    "2270587845.Madrid TrafficLights dLMSP Left_Data",
        //    "2270587845.Madrid TrafficLights dLMSP Right_Data",
        //    "2270587845.MMadrid TrafficLights xLMSP Left_Data",
        //    "2270587845.Madrid TrafficLights xLMSP Right_Data",
        //    "2270587845.Madrid TrafficLights MS Left_Data",
        //    "2270587845.Madrid TrafficLights MS Right_Data",
        //    "2270587845.Madrid TrafficLights LMS Left_Data",
        //    "2270587845.Madrid TrafficLights LMS Right_Data"

        //};

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

        public static bool IsTrafficLight(NetLaneProps.Prop laneProp) {
            if (laneProp.m_finalProp == null || laneProp.m_finalProp.m_material?.shader?.name != "Custom/Props/Prop/TrafficLight") return false;

            return (laneProp.m_startFlagsRequired & NetNode.Flags.TrafficLights) != NetNode.Flags.None
                        && (laneProp.m_startFlagsForbidden & NetNode.Flags.LevelCrossing) != NetNode.Flags.None
                   || (laneProp.m_endFlagsRequired & NetNode.Flags.TrafficLights) != NetNode.Flags.None
                        && (laneProp.m_endFlagsForbidden & NetNode.Flags.LevelCrossing) != NetNode.Flags.None;
        }

        public static bool IsLevelCrossing(NetLaneProps.Prop laneProp) {
            if (laneProp.m_finalProp == null || laneProp.m_finalProp.m_material?.shader?.name != "Custom/Props/Prop/TrafficLight") return false;

            return (laneProp.m_startFlagsRequired & NetNode.Flags.LevelCrossing) != NetNode.Flags.None
                   || (laneProp.m_endFlagsRequired & NetNode.Flags.LevelCrossing) != NetNode.Flags.None;
        }
    }
}
