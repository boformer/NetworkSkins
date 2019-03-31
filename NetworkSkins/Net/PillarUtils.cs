namespace NetworkSkins.Net
{
    public static class PillarUtils
    {
        // nullable
        public static BuildingInfo GetDefaultBridgePillar(NetInfo prefab)
        {
            var buildingAi = prefab.m_netAI;
            switch (buildingAi)
            {
                case RoadBridgeAI roadBridgeAi:
                    return roadBridgeAi.m_bridgePillarInfo;
                case TrainTrackBridgeAI trainTrackBridgeAi:
                    return trainTrackBridgeAi.m_bridgePillarInfo;
                case PedestrianBridgeAI pedestrianBridgeAi:
                    return pedestrianBridgeAi.m_bridgePillarInfo;
                case MonorailTrackAI monorailTrackAi:
                    return monorailTrackAi.m_bridgePillarInfo;
                default:
                    return null;
            }
        }

        // Monorail bend pillar
        // nullable
        public static BuildingInfo GetDefaultBridgePillar2(NetInfo prefab)
        {
            if (prefab.m_netAI is MonorailTrackAI monorailTrackAi)
            {
                return monorailTrackAi.m_bridgePillarInfo2;
            }
            else
            {
                return null;
            }
        }

        // Monorail junction pillar
        // nullable
        public static BuildingInfo GetDefaultBridgePillar3(NetInfo prefab)
        {
            if (prefab.m_netAI is MonorailTrackAI monorailTrackAi)
            {
                return monorailTrackAi.m_bridgePillarInfo3;
            }
            else
            {
                return null;
            }
        }

        // Pedestrian path elevation-dependent pillars
        // nullable
        public static BuildingInfo[] GetDefaultBridgePillars(NetInfo prefab)
        {
            if (prefab.m_netAI is PedestrianBridgeAI pedestrianBridgeAi)
            {
                return pedestrianBridgeAi.m_bridgePillarInfos;
            }
            else
            {
                return null;
            }
        }

        // nullable
        public static BuildingInfo GetDefaultMiddlePillar(NetInfo prefab)
        {
            var buildingAi = prefab.m_netAI;
            switch (buildingAi)
            {
                case RoadBridgeAI roadBridgeAi:
                    return roadBridgeAi.m_middlePillarInfo;
                case TrainTrackBridgeAI trainTrackBridgeAi:
                    return trainTrackBridgeAi.m_middlePillarInfo;
                case MonorailTrackAI monorailTrackAi:
                    return monorailTrackAi.m_middlePillarInfo;
                default:
                    return null;
            }
        }
    }
}
