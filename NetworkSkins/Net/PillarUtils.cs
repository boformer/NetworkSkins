using JetBrains.Annotations;

namespace NetworkSkins.Net
{
    public static class PillarUtils
    {
        public static bool IsPillar(BuildingInfo prefab) {
            return false;
        }

        [CanBeNull]
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

        public static void SetBridgePillar(NetInfo prefab, BuildingInfo bridgePillarInfo)
        {
            var buildingAi = prefab.m_netAI;
            if (buildingAi is RoadBridgeAI roadBridgeAi)
            {
                roadBridgeAi.m_bridgePillarInfo = bridgePillarInfo;
            }
            else if (buildingAi is TrainTrackBridgeAI trainTrackBridgeAi)
            {
                trainTrackBridgeAi.m_bridgePillarInfo = bridgePillarInfo;
            }
            else if (buildingAi is PedestrianBridgeAI pedestrianBridgeAi)
            {
                pedestrianBridgeAi.m_bridgePillarInfo = bridgePillarInfo;
            }
            else if (buildingAi is MonorailTrackAI monorailTrackAi)
            {
                monorailTrackAi.m_bridgePillarInfo = bridgePillarInfo;
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

        public static void SetBridgePillar2(NetInfo prefab, BuildingInfo bridgePillarInfo2)
        {
            if (prefab.m_netAI is MonorailTrackAI monorailTrackAi)
            {
                monorailTrackAi.m_bridgePillarInfo2 = bridgePillarInfo2;
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

        public static void SetBridgePillar3(NetInfo prefab, BuildingInfo bridgePillarInfo3)
        {
            if (prefab.m_netAI is MonorailTrackAI monorailTrackAi)
            {
                monorailTrackAi.m_bridgePillarInfo3 = bridgePillarInfo3;
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

        public static void SetBridgePillars(NetInfo prefab, BuildingInfo[] bridgePillarInfos)
        {
            if (prefab.m_netAI is PedestrianBridgeAI pedestrianBridgeAi)
            {
                pedestrianBridgeAi.m_bridgePillarInfos = bridgePillarInfos;
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

        public static void SetMiddlePillar(NetInfo prefab, BuildingInfo middlePillarInfo)
        {
            var buildingAi = prefab.m_netAI;
            if (buildingAi is RoadBridgeAI roadBridgeAi)
            {
                roadBridgeAi.m_middlePillarInfo = middlePillarInfo;
            }
            else if (buildingAi is TrainTrackBridgeAI trainTrackBridgeAi)
            {
                trainTrackBridgeAi.m_middlePillarInfo = middlePillarInfo;
            }
            else if (buildingAi is MonorailTrackAI monorailTrackAi)
            {
                monorailTrackAi.m_middlePillarInfo = middlePillarInfo;
            }
        }
    }
}
