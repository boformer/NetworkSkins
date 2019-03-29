using ICities;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins
{
    public class NetworkSkinsLoading : ILoadingExtension
    {
        public static bool SkinEnabled = true;
        private NetworkSkin _skinForBasicRoad;
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
                var streetLightModifier = new StreetLightModifier(streetLight: airportLightPrefab);
                streetLightModifier.Apply(_skinForBasicRoad);

                var terrainSurface = new TerrainSurfaceModifier(groundType: NetworkGroundType.Gravel);
                terrainSurface.Apply(_skinForBasicRoad);

                var color = new ColorModifier(color: new Color(90f / 255f, 90f / 255f, 90f / 255f));
                color.Apply(_skinForBasicRoad);

                Debug.Log($"Built skin for basic road: {_skinForBasicRoad}");
            }

            var mediumRoadPrefab = PrefabCollection<NetInfo>.FindLoaded("Medium Road");
            if (mediumRoadPrefab != null)
            {
                _skinForMediumRoad = new NetworkSkin(mediumRoadPrefab);

                var streetLampPrefab = PrefabCollection<PropInfo>.FindLoaded("StreetLamp02");
                var propBuilder = new StreetLightModifier(streetLight: streetLampPrefab);
                propBuilder.Apply(_skinForMediumRoad);

                var groundBuilder = new TerrainSurfaceModifier(groundType: NetworkGroundType.None);
                groundBuilder.Apply(_skinForMediumRoad);

                var color = new ColorModifier(color: new Color(160f / 255f, 160f / 255f, 160f / 255f));
                color.Apply(_skinForMediumRoad);

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
