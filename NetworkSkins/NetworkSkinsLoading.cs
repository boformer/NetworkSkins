using ICities;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins
{
    public class NetworkSkinsLoading : ILoadingExtension
    {
        public static bool SkinEnabled = true;
        private NetworkSkin _skinForBasicRoad;
        private NetworkSkin _skinForBasicRoadWithTrees;
        private NetworkSkin _skinForMediumRoad;

        public void OnCreated(ILoading loading)
        {
            NetworkSkinManager.Ensure();
            LoadingManager.instance.m_simulationDataReady += OnSimulationDataReady;
        }

        public void OnSimulationDataReady()
        {
           Debug.Log("OnSimulationDataReady"); // TODO
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            NetManagerHooks.EventSegmentCreate += OnSegmentCreate;
            NetManagerHooks.EventSegmentTransferData += OnSegmentTransferData;
            NetManagerHooks.EventSegmentRelease += OnSegmentRelease;
            NetManagerHooks.EventNodeRelease += OnNodeRelease;

            var basicRoadPrefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road");
            if (basicRoadPrefab != null)
            {
                _skinForBasicRoad = new NetworkSkin(basicRoadPrefab);

                var airportLightPrefab = PrefabCollection<PropInfo>.FindLoaded("Airport Light");
                _skinForBasicRoad.ApplyModifier(new StreetLightModifier(streetLight: airportLightPrefab));

                _skinForBasicRoad.ApplyModifier(new TerrainSurfaceModifier(groundType: NetworkGroundType.Gravel));

                _skinForBasicRoad.ApplyModifier(new ColorModifier(color: new Color(90f / 255f, 90f / 255f, 90f / 255f)));

                Debug.Log($"Built skin for basic road: {_skinForBasicRoad}");
            }

            var basicRoadWithTreesPrefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road Decoration Trees");
            if (basicRoadWithTreesPrefab != null)
            {
                _skinForBasicRoadWithTrees = new NetworkSkin(basicRoadWithTreesPrefab);

                _skinForBasicRoadWithTrees.ApplyModifier(new StreetLightModifier(null));

                var flowerTreePrefab = PrefabCollection<TreeInfo>.FindLoaded("Flower Tree 01");
                _skinForBasicRoadWithTrees.ApplyModifier(new SimpleTreeModifier(tree: flowerTreePrefab));

                _skinForBasicRoadWithTrees.ApplyModifier(new TerrainSurfaceModifier(groundType: NetworkGroundType.Gravel));

                _skinForBasicRoadWithTrees.ApplyModifier(new ColorModifier(color: new Color(173f / 255f, 158f / 255f, 147f / 255f)));

                Debug.Log($"Built skin for basic road with tree deco: {_skinForBasicRoadWithTrees}");
            }

            var mediumRoadPrefab = PrefabCollection<NetInfo>.FindLoaded("Medium Road");
            if (mediumRoadPrefab != null)
            {
                _skinForMediumRoad = new NetworkSkin(mediumRoadPrefab);

                var streetLampPrefab = PrefabCollection<PropInfo>.FindLoaded("StreetLamp02");
                _skinForMediumRoad.ApplyModifier(new StreetLightModifier(streetLight: streetLampPrefab));

                _skinForMediumRoad.ApplyModifier(new TerrainSurfaceModifier(groundType: NetworkGroundType.None));

                _skinForMediumRoad.ApplyModifier(new ColorModifier(color: new Color(160f / 255f, 160f / 255f, 160f / 255f)));

                Debug.Log($"Built skin for medium road: {_skinForMediumRoad}");
            }
        }

        public void OnLevelUnloading()
        {
            NetManagerHooks.EventSegmentCreate -= OnSegmentCreate;
            NetManagerHooks.EventSegmentTransferData -= OnSegmentTransferData;
            NetManagerHooks.EventSegmentRelease -= OnSegmentRelease;
            NetManagerHooks.EventNodeRelease -= OnNodeRelease;

            for (var s = 0; s < NetworkSkinManager.SegmentSkins.Length; s++)
            {
                NetworkSkinManager.SegmentSkins[s] = null;
            }
        }

        public void OnReleased()
        {
            LoadingManager.instance.m_simulationDataReady -= OnSimulationDataReady;
        }

        private void OnSegmentCreate(ushort segment)
        {
            var prefab = NetManager.instance.m_segments.m_buffer[segment].Info;
            if (SkinEnabled)
            {
                if (prefab.name == "Basic Road")
                {
                    Debug.Log($"Is basic road! {_skinForBasicRoad}");
                    NetworkSkinManager.SegmentSkins[segment] = _skinForBasicRoad;
                }
                else if (prefab.name == "Basic Road Decoration Trees")
                {
                    Debug.Log($"Is basic road with tree deco! {_skinForMediumRoad}");
                    NetworkSkinManager.SegmentSkins[segment] = _skinForBasicRoadWithTrees;
                }
                else if (prefab.name == "Medium Road")
                {
                    Debug.Log($"Is medium road! {_skinForMediumRoad}");
                    NetworkSkinManager.SegmentSkins[segment] = _skinForMediumRoad;
                }
            }
        }

        private void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            NetworkSkinManager.SegmentSkins[newSegment] = NetworkSkinManager.SegmentSkins[oldSegment];
        }

        private void OnSegmentRelease(ushort segment)
        {
            NetworkSkinManager.SegmentSkins[segment] = null;
        }

        private void OnNodeRelease(ushort node)
        {
            NetworkSkinManager.NodeSkins[node] = null;
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

                NetworkSkinsLoading.SkinEnabled = !NetworkSkinsLoading.SkinEnabled;
                Debug.Log($"Setting SkinEnabled to {NetworkSkinsLoading.SkinEnabled}");

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
