using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Net
{
    public static class RoadDecorationUtils
    {
        private static readonly string[] ArrowNames = new[]
        {
            //Vanilla Arrows
            "Road Arrow F",
            "Road Arrow FR",
            "Road Arrow L",
            "Road Arrow LF",
            "Road Arrow LFR",
            "Road Arrow LR",
            "Road Arrow R",
            "Tram Arrow",

            //Another American Highways Arrows by zeldslayer
            "2017154380.ZDS PM FLR HW_Data",
            "2017154380.ZDS PM FL HW_Data",
            "2017154380.ZDS PM FR HW_Data",
            "2017154380.ZDS PM F HW_Data",
            "2017154380.ZDS PM LR HW_Data",
            "2017154380.ZDS PM L HW_Data",
            "2017154380.ZDS PM R HW_Data",
            "2017154380.ZDS PM ML-R HW_Data",
            "2017154380.ZDS PM MR-L HW_Data",
		
            //Australian replacer Arrows
            "2071057587.AU Arrow Right_Data",
            "2071057587.AU Arrow Left_Data",
            "2071057587.AU Arrow Straight_Data",
            "2071057587.AU Arrow Straight Left_Data",
            "2071057587.AU Arrow Straight Right_Data",
            
            //BIG (Urban) Roads Arrows + Decals by hockenheim95
    	    "2228643473.BIG Urban Roads Decal Arrow F_Data",
            "2228643473.BIG Urban Roads Decal Arrow FL_Data",
            "2228643473.BIG Urban Roads Decal Arrow FR_Data",
            "2228643473.BIG Urban Roads Decal Arrow L_Data",
            "2228643473.BIG Urban Roads Decal Arrow R_Data",
            "2187238699.BIG Roads Arrow Left Decal_Data",
            "2187238699.BIG Roads Arrow Right Decal_Data",
            "2228643473.BIG Urban Roads Decal ONLY_Data",

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

            //Swedish replacer Arrows
            "2412512876.Swedish Arrow Right_Data",
            "2412512876.Swedish Arrow Left_Data",
            "2412512876.Swedish Arrow Forward_Data",
            "2412512876.Swedish Arrow Forward Left_Data",
            "2412512876.Swedish Arrow Forward Right_Data",
            "2412512876.Swedish Arrow Left Right_Data",
            "2412512876.Swedish Arrow Forward Left Right_Data",	
		
            //UK Road Project by Macwelshman
            "1406118102.UKR-D ArF_Data",
            "1406118102.UKR-D ArFL_Data",
            "1406118102.UKR-D ArFR_Data",
            "1406118102.UKR-D ArL_Data",
            "1406118102.UKR-D ArR_Data",
            "1611818494.UKR-D Arrow - Deflect Left_Data",
            "1611818494.UKR-D Arrow - Deflect Left_Data",
            "1611818494.UKR-D DC -1 Lane into 2_Data",

            //USRP replacer Arrows
            "2088550283.USRP Arrow Right_Data",
            "2088550283.USRP Arrow Left_Data",
            "2088550283.USRP Arrow Forward_Data",
            "2088550283.USRP Arrow Left-Forward_Data",
            "2088550283.USRP Arrow Forward-Right_Data"
        };

        private static readonly string[] SignNames = new[]
        {
            //Vanilla Signs		
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

    	    //Australian speed signs by Feare
            "1263918586.AUS Speed Limit 30 km/h small_Data",
            "1263918586.AUS Speed Limit 40 km/h small_Data",
            "1263918586.AUS Speed Limit 50 km/h small_Data",
            "1263918586.AUS Speed Limit 60 km/h small_Data",
            "1263918586.AUS Speed Limit 100 km/h small_Data",

            //BIG Urban Roads signs by hockenheim95
            "2228643473.BIG Roads R2 R3-7L LeftLaneTurn_Data",
            "2228643473.BIG Roads R2 R3-7R RightLaneTurn_Data",
            "2228643473.BIG Roads R2 R3-9B 2Way LeftTurn_Data",
            "2228643473.BIG Roads R2 R5-1 Do Not Enter_Data",
            "2228643473.BIG Roads R2 R-3A No Parking_Data",
            "2228643473.BIG Roads R2 R1-1 Stop Sign_Data",
            "2228643473.BIG Roads R2 R2-1 Speed Limit 25_Data",
            "2228643473.BIG Roads R2 R2-1 Speed Limit 30_Data",
            "2228643473.BIG Roads R2 R2-1 Speed Limit 35_Data",
            "2228643473.BIG Roads R2 R3-1 No Right Turn_Data",
            "2228643473.BIG Roads R2 R3-2 No Left Turn _Data",

            //Canadian (Ontario) Road Signs Pack by Nochin98
            "2287634130.100 KM/HR MAX SIGN_Data",
            "2287634130.60 KM/HR MAX SIGN_Data",
            "2287634130.50 KM/HR MAX SIGN_Data",
            "2287634130.40 KM/HR MAX SIGN_Data",
            "2287634130.30 KM/HR MAX SIGN_Data",
            "2287634130.STOP SIGN_Data",
            "2287634130.NO PARKING SIGN_Data",
            "2287634130.DO NOT ENTER SIGN_Data",
            "2287634130.HIGHWAY ROUTE SIGN_Data",
            "2287634130.NO LEFT TURN SIGN_Data",
            "2287634130.NO RIGHT TURN SIGN_Data",

            //French road signs by Raymond Pelletier
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

            //Latinoamerican Road Signs by zeldslayer
            "1774368440.ZDS RD StopA_Data",
            "1774368440.ZDS RD NR_Data",
            "1774368440.ZDS RD NL_Data",
            "1774368440.ZDS RD StopB_Data",

            //Singapore Road Sign Pack V1: Speed Limit by [MY] H4F1Z
            "1799160248.SG Speed Limit 30_Data",
            "1799160248.SG Speed Limit 40_Data",
            "1799160248.SG Speed Limit 50_Data",
            "1799160248.SG Speed Limit 60_Data",
            "1799160248.SG Speed Limit 90_Data",

            //Swedish road signs pack by SvenBerlin
            "2258591646.swedish road sign C31-3_Data",
            "2258591646.swedish road sign C31-4_Data",
            "2258591646.swedish road sign C31-5_Data",
            "2258591646.swedish road sign C31-6_Data",
            "2258591646.swedish road sign C31-10_Data",
            "2260057509.swedish road sign B2_Data",
            "2260057509.swedish road sign C39_Data",
            "2258591646.swedish road sign C25-2_Data",
            "2258591646.swedish road sign C25-1_Data",

            //Traffic Signs (Custom) by Ellysmere Haven
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
	
            //UK Road Project: Essential Prop Pack by Macwelshman
            "1719290627.UKR-MWS MW Start_Data",
            "1719290627.UKR-MWS MW End_Data",
            "1611965517.UKR-S No Stopping_Data",
            "1611965517.UKR-S 20mph_Data",
            "1406118102.UKR-MWS MW End_Data",
            "1406118102.UKR-MWS MW Start_Data",
            "1406118102.UKR-S 1W_Data",
            "1406118102.UKR-S 30_Data",
            "1406118102.UKR-S 40_Data",
            "1406118102.UKR-S 50_Data",
            "1406118102.UKR-S 60_Data",
            "1406118102.UKR-S Bk&Bs_Data",
            "1406118102.UKR-S Bk&Pd_Data",
            "1406118102.UKR-S Bks_Data",
            "1406118102.UKR-S ChevL_Data",
            "1406118102.UKR-S ChevR_Data",
            "1406118102.UKR-S GW_Data",
            "1406118102.UKR-S LC_Data",
            "1406118102.UKR-S NatS_Data",
            "1406118102.UKR-S NE_Data",
            "1406118102.UKR-S NL_Data",
            "1406118102.UKR-S NLR_Data",
            "1406118102.UKR-S NPd_Data",
            "1406118102.UKR-S NR_Data",
            "1406118102.UKR-S Tn_Data",
							
            //UK Road Sign &amp; Street Furniture Pack by Sparky66
            "1293188397.UK Give Way Sign_Data",
            "1293188397.UK No Parking Sign_Data",
            "1293188397.UK No Right Turn Sign_Data",
            "1293188397.UK No Left Turn Sign_Data",
            "1293188397.UK 20MPH Sign_Data",
            "1293188397.UK 20MPH Sign_Data",
            "1293188397.UK 30MPH Sign_Data",
            "1293188397.UK 40MPH Sign_Data",
            "1293188397.UK National Speed Limit Sign_Data",

            //US CA Freeway Props by GhostRaiderMX
            "2322292112.3L US CA I Highway Gantry Medium_Data",
            "2322292540.US CA I Highway Sign Entrance_Data",
							
            //US DOT regulatory signs by Spence!
            "1779508928.R2 R1-1 Stop Sign_Data",
            "1779508928.R2 R-3A No Parking Sign_Data",
            "1779508928.R2 R3-1 No Right Turn Sign_Data",
            "1779508928.R2 R3-2 No Left Turn Sign_Data",
            "1779508928.R2 R2-1 Spd. Limit 15 Sign_Data",
            "1779508928.R2 R2-1 Spd. Limit 25 Sign_Data",
            "1779508928.R2 R2-1 Spd. Limit 30 Sign_Data",
            "1779508928.R2 R2-1 Speed Limit 35 Sign_Data",
            "1779508928.R2 R2-1 Spd. Limit 65 Sign_Data"

        };

        private static readonly string[] DecorationNames = new[]
        {
            //Vanilla Props
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
		
            //Another American Highways Decals and Props by zeldslayer
            "2017154380.ZDS manhole 1_Data",
            "2017154380.ZDS Manhole _Data",
            "2017154380.ZDS PM ISA_Data",
            "2017154380.ZDS PM BK_Data",
            "2017154380.ZDS PM RR_Data",
            "2035340479.PostA_Data",	
            "2035340479.PostB_Data",
	
            //BIG Urban Roads Props + decals
            "2228643473.BIG Urban Roads Delineator_Data",
            "2228643473.BIG Urban Roads Tunnel Exit Door_Data",
            "2228643473.BIG Urban Roads Decal BUS_Data",
            "2228643473.BIG Urban Roads Decal ONLY_Data",
            "2228643473.BIG Urban Roads Decal RR XING_Data",
            "2228643473.BIG Urban Roads Decal STOP_Data",
            "2187238699.BIG Roads Railroad Xing_Data",
            "2187238699.BIG Roads ONLY Decal_Data",
            "2187238699.BIG Roads STOP Line Decal short_Data",
            "2187238699.BIG Roads STOP Line Decal_Data",
            "2187238699.BIG Roads Xing 10L Decal_Data",
            "2228643473.BBIG Urban Roads Decal Line 1L_Data",
            "2228643473.BIG Urban Roads Decal Line 2L_Data",

            //clus road props + decals
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
            "1919887701.Warning Light #01_Data",

            //German Delineators by Olegson
            "1832037749.German Delineator_Data",
            "1849355863.German Junction Delineator_Data",
		
	    //RWY2-USA Deco Lane Props
	    "2487537755.r2-ident01nec_data",
	    "2487537755.r2-ident01-4nec_data",
	    "2487537755.r2-ident02-4us_data",
	    "2487537755.r2-ident01us_data",
	    "2487537755.r2-ident01-4us_data",
	    "2487537755.r2-ident02us_data",
	    "2487537755.r2-weldus_data",

	    //RWY2-EUR Deco Lane Props
	    "2518959391.r2-fencebeam_data",
	    "2518959391.r2-ident01_data",
	    "2518959391.r2-ident01-4_data",
	    "2518959391.r2-ident02_data",
	    "2518959391.r2-ident02-4_data",
	    "2518959391.r2-weld_data",

	    //RWY1 Deco Lane Props
	    "1530376523.r69railway-fishplate_data",
	    "1530376523.r69railway-fence-beam_data",

            //UK Road Project by Macwelshman
            "1611818494.UKR-D Parking Text (Disabled)_Data",
            "1611818494.UKR-D Parking Text (Doctor)_Data",
            "1611818494.UKR-D Parking Text (Loading)_Data",
            "1611818494.UKR-D Parking Text (Taxis)_Data",
            "1611818494.UKR-D Slow_Data",
            "1406118102.UKR-D NE_Data",
            "1406118102.UKR-D GWTr_Data",
            "1406118102.UKR-D BkLn_Data",
            "1406118102.UKR-D BsLn_Data",
            "1406118102.UKR-D 30_Data",
            "1406118102.UKR-D 40_Data",
            "1406118102.UKR-D 50_Data",
            "1406118102.UKR-D 60_Data",
            "1406118102.UKR-P BsSt_Data",
            "1406118102.UKR-P CrIsl_Data",
            "1406118102.UKR-P LCSL_Data",
            "1406118102.UKR-P LCSR_Data",
            "1406118102.UKR-P MedIs_Data",
            "1406118102.UKR-P MedIsNr_Data",
            "1406118102.UKR-P PBx_Data",
            "1406118102.UKR-P PkMt_Data",
            "1406118102.UKR-P StCb_Data",
            "1406118102.UKR-P TrBol L_Data",
            "1406118102.UKR-P TrBol R_Data",
            "1611818494.UKR-D Tactile Pavement_Data",
            "1406118102.UKR-D TcSt10_Data",
            "1406118102.UKR-D TcSt11_Data",
            "1406118102.UKR-D TcSt13_Data",
            "1406118102.UKR-D TcSt4_Data",
            "1406118102.UKR-D TcSt5_Data",
            "1406118102.UKR-D TcSt6_Data",
            "1406118102.UKR-D TcSt7_Data",
            "1406118102.UKR-D TcSt8_Data",
            "1406118102.UKR-D TcSt9_Data",
            "1406118102.UKR-D DrBL_Data",
            "1406118102.UKR-D DrDYL_Data"

        };


        public static bool IsArrow(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(ArrowNames, prop.name) != -1;
        }

        public static bool IsSign(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(SignNames, prop.name) != -1;
        }

        public static bool IsDecoration(PropInfo prop)
        {
            if (prop == null) return false;
            return Array.IndexOf(DecorationNames, prop.name) != -1;
        }

        public static readonly NetLane.Flags stopFlags = NetLane.Flags.Stop | NetLane.Flags.Stop2 | NetLane.Flags.Stops;
        public static bool IsTransportStop(NetLaneProps.Prop laneProp)
        {
            if (laneProp.m_finalProp == null) return false;

            return (laneProp.m_flagsRequired & stopFlags) != NetLane.Flags.None;
        }

        public static bool IsTrafficLight(NetLaneProps.Prop laneProp)
        {
            if (laneProp.m_finalProp == null) return false;
            
            //|| laneProp.m_finalProp.m_material?.shader?.name != "Custom/Props/Prop/TrafficLight"

            return (laneProp.m_startFlagsRequired & NetNode.Flags.TrafficLights) != NetNode.Flags.None
                        && (laneProp.m_startFlagsForbidden & NetNode.Flags.LevelCrossing) != NetNode.Flags.None
                   || (laneProp.m_endFlagsRequired & NetNode.Flags.TrafficLights) != NetNode.Flags.None
                        && (laneProp.m_endFlagsForbidden & NetNode.Flags.LevelCrossing) != NetNode.Flags.None;
        }

        public static bool IsLevelCrossing(NetLaneProps.Prop laneProp)
        {
            if (laneProp.m_finalProp == null || laneProp.m_finalProp.m_material?.shader?.name != "Custom/Props/Prop/TrafficLight") return false;

            return (laneProp.m_startFlagsRequired & NetNode.Flags.LevelCrossing) != NetNode.Flags.None
                   || (laneProp.m_endFlagsRequired & NetNode.Flags.LevelCrossing) != NetNode.Flags.None;
        }

    }
}
