using System.Collections.Generic;
using ICities;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins
{
    public class NetworkSkinsLoading : ILoadingExtension
    {
        private static bool _skinEnabled = false;

        public void OnCreated(ILoading loading)
        {
            NetworkSkinManager.Ensure(); // TODO remove here? it's on OnEnabled
            LoadingManager.instance.m_simulationDataReady += OnSimulationDataReady;
        }

        public void OnSimulationDataReady()
        {
           Debug.Log("OnSimulationDataReady"); // TODO
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            ToggleSampleSkins();
        }

        public void OnLevelUnloading()
        {
            NetworkSkinManager.instance.ClearActiveModifiers();
        }

        public void OnReleased()
        {
            LoadingManager.instance.m_simulationDataReady -= OnSimulationDataReady;
        }

        public static void ToggleSampleSkins()
        {
            _skinEnabled = !_skinEnabled;

            var m = new Dictionary<NetInfo, List<NetworkSkinModifier>>();
            if (_skinEnabled)
            {
                BuildBasicRoadSampleSkin(m);
                BuildBasicRoadElevatedSampleSkin(m);
                BuildBasicRoadWithTreesSampleSkin(m);
                BuildMediumRoadSampleSkin(m);
                BuildTrainTrackSampleSkin(m);
                BuildTrainOnewayTrackSampleSkin(m);
            }
            else
            {
                BuildBasicRoadSampleSkin(m);
            }
            NetworkSkinManager.instance.SetActiveModifiers(m);

            Debug.Log($"Setting SkinEnabled to {_skinEnabled}");
        }

        public static void BuildBasicRoadSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: NetworkGroundType.Gravel),
                new ColorModifier(color: new Color(90f / 255f, 90f / 255f, 90f / 255f)),
                new StreetLightModifier(streetLight: PrefabCollection<PropInfo>.FindLoaded("Airport Light"), repeatDistance: 80)
            });
        }

        public static void BuildBasicRoadElevatedSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road Elevated");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new ColorModifier(color: new Color(110f / 255f, 90f / 255f, 90f / 255f)),
                new StreetLightModifier(streetLight: null),
                new PillarModifier(bridgePillarInfo: PrefabCollection<BuildingInfo>.FindLoaded("RailwayElevatedPillar"))
            });
        }

        public static void BuildBasicRoadWithTreesSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road Decoration Trees");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: NetworkGroundType.Gravel),
                new ColorModifier(color: new Color(173f / 255f, 158f / 255f, 147f / 255f)),
                new StreetLightModifier(streetLight: null),
                new TreeModifier(tree: PrefabCollection<TreeInfo>.FindLoaded("Flower Tree 01"), repeatDistance: 5)
            });
        }

        public static void BuildMediumRoadSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Medium Road");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: NetworkGroundType.None),
                new ColorModifier(color: new Color(160f / 255f, 160f / 255f, 160f / 255f)),
                new StreetLightModifier(streetLight: PrefabCollection<PropInfo>.FindLoaded("StreetLamp02"))
            });
        }

        public static void BuildTrainTrackSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: NetworkGroundType.Pavement),
                new CatenaryModifier(catenary: null)
            });
        }

        public static void BuildTrainOnewayTrackSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Train Oneway Track");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: NetworkGroundType.Ruined),
                new CatenaryModifier(catenary: null)
            });
        }
    }

    public class KeyInputThreading : ThreadingExtensionBase
    {
        private bool _processed = false;

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            var control = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
            if (control && Input.GetKey(KeyCode.N))
            {
                if (_processed) return;

                NetworkSkinsLoading.ToggleSampleSkins();

                _processed = true;
            }
            else
            {
                // not both keys pressed: Reset processed state
                _processed = false;
            }
        }
    }
}
