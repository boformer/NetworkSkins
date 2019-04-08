using ColossalFramework.Packaging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetworkSkins.Net
{
    public static class PillarUtils
    {
        public static bool SupportsPillars(NetInfo prefab, PillarType type)
        {
            var netAi = prefab.m_netAI;
            if (type == PillarType.Bridge)
            {
                return netAi is RoadBridgeAI
                       || netAi is TrainTrackBridgeAI
                       || netAi is PedestrianBridgeAI
                       || netAi is MonorailTrackAI;
            }
            else
            {
                return netAi.RequireDoubleSegments()
                       && (netAi is RoadBridgeAI
                       || netAi is TrainTrackBridgeAI
                       || netAi is MonorailTrackAI);
            }
        }

        public static List<BuildingInfo> GetAvailablePillars()
        {
            var uniquePillars = new HashSet<BuildingInfo>();

            // Get all pillars that are used in a network
            var prefabCount = PrefabCollection<NetInfo>.LoadedCount();
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                var prefab = PrefabCollection<NetInfo>.GetLoaded(prefabIndex);
                if (prefab == null) continue;

                var bridgePillar = GetDefaultBridgePillar(prefab);
                if (bridgePillar != null) uniquePillars.Add(bridgePillar);

                var bridgePillar2 = GetDefaultBridgePillar2(prefab);
                if (bridgePillar2 != null) uniquePillars.Add(bridgePillar2);

                var bridgePillar3 = GetDefaultBridgePillar3(prefab);
                if (bridgePillar3 != null) uniquePillars.Add(bridgePillar3);

                var bridgePillars = GetDefaultBridgePillars(prefab);
                if (bridgePillars != null)
                {
                    foreach (var pillar in bridgePillars)
                    {
                        if (pillar != null) uniquePillars.Add(pillar);
                    }
                }

                var middlePillar = GetDefaultMiddlePillar(prefab);
                if (middlePillar != null) uniquePillars.Add(middlePillar);
            }

            // Get additional pillars that were supported in old NetworkSkins
            foreach (var pillar in GetLegacyCustomPillars())
            {
                uniquePillars.Add(pillar);
            }

            var pillars = new List<BuildingInfo>(uniquePillars);

            // Sort by name
            pillars.Sort((a, b) => string.Compare(a.GetUncheckedLocalizedTitle(), b.GetUncheckedLocalizedTitle(), StringComparison.Ordinal));

            return pillars;
        }

        public static List<BuildingInfo> GetLegacyCustomPillars()
        {
            var pillars = new List<BuildingInfo>();

            var prefabCount = PrefabCollection<BuildingInfo>.LoadedCount();

            // support for custom pillars
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++)
            {
                var prefab = PrefabCollection<BuildingInfo>.GetLoaded(prefabIndex);
                if (prefab == null) continue;

                // only accept buildings with a basic AI
                if (prefab.m_buildingAI.GetType() != typeof(BuildingAI)) continue;

                var asset = PackageManager.FindAssetByName(prefab.name);

                var crpPath = asset?.package?.packagePath;
                if (crpPath == null) continue;

                var pillarConfigPath = Path.Combine(Path.GetDirectoryName(crpPath), "Pillar.xml");

                if (File.Exists(pillarConfigPath))
                {
                    pillars.Add(prefab);
                }
            }

            return pillars;
        }

        [CanBeNull]
        public static BuildingInfo GetDefaultPillar(NetInfo prefab, PillarType type)
        {
            switch (type)
            {
                case PillarType.Bridge:
                    return GetDefaultBridgePillar(prefab);
                case PillarType.Middle:
                    return GetDefaultMiddlePillar(prefab);
                default:
                    return null;
            }
        }

        [CanBeNull]
        public static BuildingInfo GetDefaultBridgePillar(NetInfo prefab)
        {
            var netAi = prefab.m_netAI;
            switch (netAi)
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
            var netAi = prefab.m_netAI;
            if (netAi is RoadBridgeAI roadBridgeAi)
            {
                roadBridgeAi.m_bridgePillarInfo = bridgePillarInfo;
            }
            else if (netAi is TrainTrackBridgeAI trainTrackBridgeAi)
            {
                trainTrackBridgeAi.m_bridgePillarInfo = bridgePillarInfo;
            }
            else if (netAi is PedestrianBridgeAI pedestrianBridgeAi)
            {
                pedestrianBridgeAi.m_bridgePillarInfo = bridgePillarInfo;
            }
            else if (netAi is MonorailTrackAI monorailTrackAi)
            {
                monorailTrackAi.m_bridgePillarInfo = bridgePillarInfo;
            }
        }

        // Monorail bend pillar
        [CanBeNull]
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
        [CanBeNull]
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
        [CanBeNull]
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

        [CanBeNull]
        public static BuildingInfo GetDefaultMiddlePillar(NetInfo prefab)
        {
            var netAi = prefab.m_netAI;
            switch (netAi)
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
            var netAi = prefab.m_netAI;
            if (netAi is RoadBridgeAI roadBridgeAi)
            {
                roadBridgeAi.m_middlePillarInfo = middlePillarInfo;
            }
            else if (netAi is TrainTrackBridgeAI trainTrackBridgeAi)
            {
                trainTrackBridgeAi.m_middlePillarInfo = middlePillarInfo;
            }
            else if (netAi is MonorailTrackAI monorailTrackAi)
            {
                monorailTrackAi.m_middlePillarInfo = middlePillarInfo;
            }
        }
    }
}
