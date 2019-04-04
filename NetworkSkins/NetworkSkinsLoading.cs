using System.Collections.Generic;
using ICities;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

// TODO remove when the UI is working

namespace NetworkSkins
{
    public class NetworkSkinsLoading : ILoadingExtension
    {
        private static int _skinEnabled = 0;

        public void OnCreated(ILoading loading) {}

        public void OnLevelLoaded(LoadMode mode)
        {
            ToggleSampleSkins();
        }

        public void OnLevelUnloading() {}

        public void OnReleased() {}

        public static void ToggleSampleSkins()
        {
            _skinEnabled++;
            _skinEnabled %= 3;

            var m = new Dictionary<NetInfo, List<NetworkSkinModifier>>();
            if (_skinEnabled == 1)
            {
                BuildBasicRoadSampleSkin(m);
                BuildBasicRoadElevatedSampleSkin(m);
                BuildBasicRoadWithTreesSampleSkin(m);
                BuildMediumRoadSampleSkin(m);
                BuildTrainTrackSampleSkin(m);
                BuildTrainOnewayTrackSampleSkin(m);
            }
            else if(_skinEnabled == 2)
            {
                BuildAlternativeBasicRoadSampleSkin(m);
                BuildAlternativeBasicRoadWithTreesSampleSkin(m);
                BuildAlternativeTrainTrackSampleSkin(m);
                BuildAlternativeTrainOnewayTrackSampleSkin(m);
                BuildAlternativeMediumRoadSampleSkin(m);
            }
            NetworkSkinManager.instance.SetActiveModifiers(m);

            Debug.Log($"Setting SkinEnabled to {_skinEnabled}");
        }

        public static void BuildBasicRoadSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: Surface.Gravel),
                new ColorModifier(color: new Color32(90, 90, 90, 255)),
                new StreetLightModifier(streetLight: PrefabCollection<PropInfo>.FindLoaded("Airport Light"), repeatDistance: 80)
            });
        }

        public static void BuildAlternativeBasicRoadSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new ColorModifier(color: new Color32(118, 130, 118, 255)),
                new StreetLightModifier(streetLight: PrefabCollection<PropInfo>.FindLoaded("StreetLamp02"), repeatDistance: 80)
            });
        }

        public static void BuildBasicRoadElevatedSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road Elevated");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new ColorModifier(color: new Color32(110, 90, 90, 255)),
                new StreetLightModifier(streetLight: null),
                new PillarModifier(bridgePillarInfo: PrefabCollection<BuildingInfo>.FindLoaded("RailwayElevatedPillar"))
            });
        }

        public static void BuildBasicRoadWithTreesSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road Decoration Trees");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: Surface.Gravel),
                new ColorModifier(color: new Color32(173, 158, 147, 255)),
                new StreetLightModifier(streetLight: null),
                new TreeModifier(
                    leftTree: PrefabCollection<TreeInfo>.FindLoaded("Flower Tree 01"), 
                    leftTreeRepeatDistance: 5,
                    rightTree: PrefabCollection<TreeInfo>.FindLoaded("Flower Tree 01"),
                    righTreeRepeatDistance: 10
                )
            });
        }

        public static void BuildAlternativeBasicRoadWithTreesSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road Decoration Trees");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new ColorModifier(color: new Color32(160, 160, 160, 255)),
                new TreeModifier(tree: PrefabCollection<TreeInfo>.FindLoaded("909448182.Royal Palm_Data"), repeatDistance: 20)
            });
        }

        public static void BuildMediumRoadSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Medium Road");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: Surface.None),
                new ColorModifier(color: new Color32(160, 160, 160, 255)),
                new StreetLightModifier(streetLight: PrefabCollection<PropInfo>.FindLoaded("StreetLamp02"))
            });
        }

        public static void BuildAlternativeMediumRoadSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Medium Road");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: Surface.Gravel),
                new ColorModifier(color: new Color32(130, 90, 90, 255))
            });
        }

        public static void BuildTrainTrackSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: Surface.Pavement),
                new CatenaryModifier(catenary: null)
            });
        }

        public static void BuildAlternativeTrainTrackSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new CatenaryModifier(catenary: PrefabCollection<PropInfo>.FindLoaded("774449380.Catenary Type DE2A_Data"))
            });
        }

        public static void BuildTrainOnewayTrackSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Train Oneway Track");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new TerrainSurfaceModifier(groundType: Surface.Ruined),
                new CatenaryModifier(catenary: null)
            });
        }

        public static void BuildAlternativeTrainOnewayTrackSampleSkin(Dictionary<NetInfo, List<NetworkSkinModifier>> m)
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Train Oneway Track");
            m.Add(prefab, new List<NetworkSkinModifier>
            {
                new CatenaryModifier(catenary: PrefabCollection<PropInfo>.FindLoaded("774449380.Catenary Type DE1A_Data"))
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
