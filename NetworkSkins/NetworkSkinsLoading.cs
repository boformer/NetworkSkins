using System.Collections.Generic;
using ICities;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins
{
    public class NetworkSkinsLoading : ILoadingExtension
    {
        public static bool SkinEnabled = false;

        public static readonly List<NetworkSkin> SampleSkins = new List<NetworkSkin>();

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
            BuildBasicRoadSampleSkin();

            BuildBasicRoadElevatedSampleSkin();

            BuildBasicRoadWithTreesSampleSkin();

            BuildMediumRoadSampleSkin();

            BuildTrainTrackSampleSkin();

            BuildTrainOnewayTrackSampleSkin();

            SkinEnabled = true;
            NetworkSkinManager.instance.SetActiveSkins(SampleSkins);
        }

        public void OnLevelUnloading()
        {
            SampleSkins.Clear();
        }

        public void OnReleased()
        {
            LoadingManager.instance.m_simulationDataReady -= OnSimulationDataReady;
        }

        private static void BuildBasicRoadSampleSkin()
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road");
            if (prefab == null) return;

            var skin = new NetworkSkin(prefab);

            var airportLightPrefab = PrefabCollection<PropInfo>.FindLoaded("Airport Light");
            skin.ApplyModifier(new StreetLightModifier(streetLight: airportLightPrefab));

            skin.ApplyModifier(new TerrainSurfaceModifier(groundType: NetworkGroundType.Gravel));

            skin.ApplyModifier(new ColorModifier(color: new Color(90f / 255f, 90f / 255f, 90f / 255f)));

            SampleSkins.Add(skin);

            Debug.Log($"Built skin for basic road: {skin}");
        }

        private static void BuildBasicRoadElevatedSampleSkin()
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road Elevated");
            if (prefab == null) return;

            var skin = new NetworkSkin(prefab);

            skin.ApplyModifier(new StreetLightModifier(streetLight: null));

            var railwayPillar = PrefabCollection<BuildingInfo>.FindLoaded("RailwayElevatedPillar");
            skin.ApplyModifier(new PillarModifier(bridgePillarInfo: railwayPillar));

            skin.ApplyModifier(new ColorModifier(color: new Color(110f / 255f, 90f / 255f, 90f / 255f)));

            SampleSkins.Add(skin);

            Debug.Log($"Built skin for basic road elevated: {skin}");
        }

        private static void BuildBasicRoadWithTreesSampleSkin()
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Basic Road Decoration Trees");
            if (prefab == null) return;

            var skin = new NetworkSkin(prefab);

            skin.ApplyModifier(new StreetLightModifier(null));

            var flowerTreePrefab = PrefabCollection<TreeInfo>.FindLoaded("Flower Tree 01");
            skin.ApplyModifier(new SimpleTreeModifier(tree: flowerTreePrefab));

            skin.ApplyModifier(new TerrainSurfaceModifier(groundType: NetworkGroundType.Gravel));

            skin.ApplyModifier(new ColorModifier(color: new Color(173f / 255f, 158f / 255f, 147f / 255f)));

            SampleSkins.Add(skin);

            Debug.Log($"Built skin for basic road with tree deco: {skin}");
        }

        private static void BuildMediumRoadSampleSkin()
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Medium Road");
            if (prefab == null) return;

            var skin = new NetworkSkin(prefab);

            var streetLampPrefab = PrefabCollection<PropInfo>.FindLoaded("StreetLamp02");
            skin.ApplyModifier(new StreetLightModifier(streetLight: streetLampPrefab));

            skin.ApplyModifier(new TerrainSurfaceModifier(groundType: NetworkGroundType.None));

            skin.ApplyModifier(new ColorModifier(color: new Color(160f / 255f, 160f / 255f, 160f / 255f)));

            SampleSkins.Add(skin);

            Debug.Log($"Built skin for medium road: {skin}");
        }

        private static void BuildTrainTrackSampleSkin()
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            if (prefab == null) return;

            var skin = new NetworkSkin(prefab);

            skin.ApplyModifier(new TerrainSurfaceModifier(groundType: NetworkGroundType.Pavement));

            skin.ApplyModifier(new CatenaryModifier(catenary: null));

            SampleSkins.Add(skin);

            Debug.Log($"Built skin for train track: {skin}");
        }

        private static void BuildTrainOnewayTrackSampleSkin()
        {
            var prefab = PrefabCollection<NetInfo>.FindLoaded("Train Oneway Track");
            if (prefab == null) return;

            var skin = new NetworkSkin(prefab);

            skin.ApplyModifier(new CatenaryModifier(catenary: null));

            SampleSkins.Add(skin);

            Debug.Log($"Built skin for oneway train track: {skin}");
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

                if (NetworkSkinsLoading.SkinEnabled)
                {
                    NetworkSkinManager.instance.SetActiveSkins(NetworkSkinsLoading.SampleSkins);
                }
                else
                {
                    NetworkSkinManager.instance.SetActiveSkins(new List<NetworkSkin>());
                }

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
