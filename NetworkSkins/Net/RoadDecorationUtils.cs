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
            "Stop Sign",
            "Street Name Sign",
            			
	    //US DOT regulatory signs by Spence!"
				
	    "1779508928.R2 R1-1 Stop Sign_Data",
            "1779508928.R2 R-3A No Parking Sign_Data",
            "1779508928.R2 R3-1 No Right Turn Sign_Data",
            "1779508928.R2 R3-2 No Left Turn Sign_Data",
            "1779508928.R2 R2-1 Spd. Limit 15 Sign_Data",
            "1779508928.R2 R2-1 Spd. Limit 25 Sign_Data",
            "1779508928.R2 R2-1 Spd. Limit 30 Sign_Data",
            "1779508928.R2 R2-1 Speed Limit 35 Sign_Data",
            "1779508928.R2 R2-1 Spd. Limit 65 Sign_Data",
				
	    //UK Road Project: Essential Prop Pack by Macwelshman"
				
	    "1406118102.UKR-S GW_Data",
            "1406118102.UKR-S NR_Data",
            "1406118102.UKR-S NL_Data",
            "1611965517.UKR-S No Stopping_Data",
            "1406118102.UKR-MWS MW Start_Data",
            "1406118102.UKR-S 30_Data",
            "1406118102.UKR-S 40_Data",
            "1406118102.UKR-S 50_Data",
            "1406118102.UKR-S 60_Data",
            "1406118102.UKR-S NatS_Data",
							
	    //Latinoamerican Road Signs by zeldslayer"
				
	    "1774368440.ZDS RD StopA_Data",
            "1774368440.ZDS RD NR_Data",
            "1774368440.ZDS RD NL_Data",
            "1774368440.ZDS RD StopB_Data",
								
	    //UK Road Sign &amp; Street Furniture Pack by Sparky66"
				
	    "1293188397.UK Give Way Sign_Data",
            "1293188397.UK No Parking Sign_Data",
            "1293188397.UK No Right Turn Sign_Data",
            "1293188397.UK No Left Turn Sign_Data",
            "1293188397.UK 20MPH Sign_Data",
            "1293188397.UK 20MPH Sign_Data",
            "1293188397.UK 30MPH Sign_Data",
            "1293188397.UK 40MPH Sign_Data",
            "1293188397.UK National Speed Limit Sign_Data",
							
	    //Australian speed signs by Feare"
				
	    "1263918586.AUS Speed Limit 30 km/h small_Data",
            "1263918586.AUS Speed Limit 40 km/h small_Data",
            "1263918586.AUS Speed Limit 50 km/h small_Data",
            "1263918586.AUS Speed Limit 60 km/h small_Data",
            "1263918586.AUS Speed Limit 100 km/h small_Data",
							
	    //Singapore Road Sign Pack V1: Speed Limit by [MY] H4F1Z"
				
	    "1799160248.SG Speed Limit 30_Data",
            "1799160248.SG Speed Limit 40_Data",
            "1799160248.SG Speed Limit 50_Data",
            "1799160248.SG Speed Limit 60_Data",
            "1799160248.SG Speed Limit 90_Data",
							
	    //Italy road signs - PACK by Arnold J. Rimmer, Bsc. Ssc.
				
	    "1685731764.ITA Max speed 30_Data",
            "1685731764.ITA Max speed 50_Data",
            "1685731764.ITA Max speed 50_Data",
            "1685731764.ITA Max speed 60_Data",
            "1685731764.ITA Max speed 80_Data",
            "1685731764.ITA Stop_Data",
            "1685731764.ITA No parking_Data",
            "1685731764.ITA Fast-traffic highway_Data",
            "1685731764.ITA Drive straight or left_Data",
            "1685731764.ITA Drive straight or right_Data",
							
	    //German sign pack for Highway/National Road/Road-work-sites and other random signs by UFF
				
	    "1178771213.30km/h Höchstgeschwindigkeit_Data",
            "1178771213.50km/h Höchstgeschwindigkeit_Data",
            "1178771213.50km/h Höchstgeschwindigkeit_Data",
            "1178771213.60km/h Höchstgeschwindigkeit_Data",
            "1178771213.120km/h Höchstgeschwindigkeit_Data",
            "1178771213.Halt! Vorfahrt gewähren!_Data",
            "1178771213.Vorgeschriebene Fahrtrichtung GL_Data",
            "1178771213.Vorgeschriebene Fahrtrichtung GR_Data",
            "1178771213.AUTOBAHN 3Sp-2G,1R KP_Data",
								
	    //French road signs by Raymond Pelletier"
				
	    "1073554884.French Road Sign - B14.30_Data",
            "1073554884.French Road Sign - B14.50_Data",
            "1073554884.French Road Sign - B14.50_Data",
            "1073554884.French Road Sign - B14.70_Data",
            "1073554884.French Road Sign - B14.110_Data",
            "1003078914.French Road Sign - AB4_Data",
            "1073554884.French Road Sign - B6b_Data",
            "1073554884.French Road Sign - B2b_Data",
            "1073554884.French Road Sign - B2a_Data",
            "1074496614.French Road Sign - C207_Data",
							
	    //Traffic Signs (Custom) by Ellysmere Haven"
				
	    "2013164837.30km/h Speed Limit_Data",
            "2013164837.40km/h Speed Limit_Data",
            "2013164837.50km/h Speed Limit_Data",
            "2013164837.60km/h Speed Limit_Data",
            "2013164837.100km/h Speed Limit_Data",
            "2013164837.Stop_Data",
            "2013164837.No Parking_Data",
            "2013164837.No Right Turn_Data",
            "2013164837.No Left Turn_Data",
            "2013164837.Motorway Road_Data",
							
	    //Swedish road signs pack by SvenBerlin"
				
	    "2258591646.swedish road sign C31-3_Data",
            "2258591646.swedish road sign C31-4_Data",
            "2258591646.swedish road sign C31-5_Data",
            "2258591646.swedish road sign C31-6_Data",
            "2258591646.swedish road sign C31-10_Data",
            "2260057509.swedish road sign B2_Data",
            "2260057509.swedish road sign C39_Data",
            "2258591646.swedish road sign C25-2_Data",
            "2258591646.swedish road sign C25-1_Data"

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

        //private static readonly string[] TransportStopNames = new[]
        //{
        //    "Bus Stop Large",
        //    "Bus Stop Small",
        //    "Tram Stop",
        //    "Tram Stop Sign",
        //    "Trolleybus Stop",
        //    "Trolleybus Stop Large 01",
        //    "Sightseeing Bus Stop Small",
        //    "Sightseeing Bus Stop Large",
        //    "Taxiway Sign 01",
        //    "Taxiway Sign 02",

        //    //clus stops
        //    "1919887701.Bus Shelter #01 - Frame_Data",
        //    "1919887701.Bus Shelter 01 - Glass_Data",
        //    "1919887701.Bus Shelter without Time Table_Data",
        //    "1919887701.Tram Shelter #01 - Frame_Data",
        //    "1919887701.Tram Shelter #01 - Glass _Data",
        //    "1919887701.Tram Shelter #03 - Frame_Data",
        //    "1919887701.Tram Shelter #03 - Glass_Data",
        //    "1919887701.Tram Shelter #04 - Frame_Data",
        //    "1919887701.Tram Shelter #04 - Glass_Data",

        //    //Pewex stops (used also for clus stops)
        //    "1743534472.Bus Stop Sign_Data",
        //    "1743534472.Tram Stop Sign_Data"

        //};

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

        public static readonly NetLane.Flags stopFlags = NetLane.Flags.Stop | NetLane.Flags.Stop2 | NetLane.Flags.Stops;
        public static bool IsTransportStop(NetLaneProps.Prop laneProp) {
            if (laneProp.m_finalProp == null) return false;

            return (laneProp.m_flagsRequired & stopFlags) != NetLane.Flags.None;
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
