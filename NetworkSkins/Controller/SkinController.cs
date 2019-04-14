using System.Collections.Generic;
using System.Linq;
using NetworkSkins.Controller;
using NetworkSkins.GUI;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins
{
    public class SkinController : MonoBehaviour {
        public static SkinController Instance;

        public delegate void ToolStateChangedEventHandler(bool state);
        public event ToolStateChangedEventHandler EventToolStateChanged;
        public delegate void PrefabChangedEventHandler(NetInfo netInfo);
        public event PrefabChangedEventHandler EventPrefabChanged;

        private bool _ignoreModifierEvents = false;
        public bool TabClicked { get; set; } = false;

        public TerrainSurfaceFeatureController TerrainSurface;

        public ColorFeatureController Color;

        public StreetLightFeatureController StreetLight;

        public bool TreesEnabled => LeftTree.Enabled || MiddleTree.Enabled || RighTree.Enabled;
        public LanePosition LanePosition { get; set; } = LanePosition.Left;
        public TreeFeatureController LeftTree;
        public TreeFeatureController MiddleTree;
        public TreeFeatureController RighTree;
        
        public bool PillarsEnabled => ElevatedBridgePillar.Enabled || ElevatedMiddlePillar.Enabled || BridgeBridgePillar.Enabled || BridgeMiddlePillar.Enabled;
        public Pillar PillarElevationCombination { get; set; } = Pillar.Bridge;
        public PillarFeatureController ElevatedBridgePillar;
        public PillarFeatureController ElevatedMiddlePillar;
        public PillarFeatureController BridgeBridgePillar;
        public PillarFeatureController BridgeMiddlePillar;

        public CatenaryFeatureController Catenary;

        public NetInfo Prefab { get; private set; }

        private bool isNetToolEnabled;

        private void Awake() {
            Instance = this;

            NetworkSkinManager.instance.EventSegmentPlaced += OnSegmentPlaced;

            TerrainSurface = new TerrainSurfaceFeatureController();
            TerrainSurface.EventModifiersChanged += OnModifiersChanged;

            Color = new ColorFeatureController();
            Color.EventModifiersChanged += OnModifiersChanged;

            StreetLight = new StreetLightFeatureController();
            StreetLight.EventModifiersChanged += OnModifiersChanged;

            LeftTree = new TreeFeatureController(LanePosition.Left);
            LeftTree.EventModifiersChanged += OnModifiersChanged;

            MiddleTree = new TreeFeatureController(LanePosition.Middle);
            MiddleTree.EventModifiersChanged += OnModifiersChanged;

            RighTree = new TreeFeatureController(LanePosition.Right);
            RighTree.EventModifiersChanged += OnModifiersChanged;
            
            var availablePillars = PillarUtils.GetAvailablePillars();

            ElevatedBridgePillar = new PillarFeatureController(PillarType.Bridge, availablePillars);
            ElevatedBridgePillar.EventModifiersChanged += OnModifiersChanged;

            ElevatedMiddlePillar = new PillarFeatureController(PillarType.Middle, availablePillars);
            ElevatedMiddlePillar.EventModifiersChanged += OnModifiersChanged;

            BridgeBridgePillar = new PillarFeatureController(PillarType.Bridge, availablePillars);
            BridgeBridgePillar.EventModifiersChanged += OnModifiersChanged;

            BridgeMiddlePillar = new PillarFeatureController(PillarType.Middle, availablePillars);
            BridgeMiddlePillar.EventModifiersChanged += OnModifiersChanged;

            Catenary = new CatenaryFeatureController();
            Catenary.EventModifiersChanged += OnModifiersChanged;
        }

        public void SetActivePillarElevation(Pillar pillar) {
            PillarElevationCombination = pillar;
            EventPrefabChanged?.Invoke(Prefab);
        }

        public void SetActiveLane(LanePosition value) {
            LanePosition = value;
            EventPrefabChanged?.Invoke(Prefab);
        }

        private void Update() {
            if (ToolsModifierControl.toolController.CurrentTool is NetTool netTool) {
                if (!isNetToolEnabled) {
                    isNetToolEnabled = true;
                    EventToolStateChanged?.Invoke(true);
                }
                if (netTool.Prefab != null && Prefab != netTool.Prefab) {
                    Prefab = netTool.m_prefab;
                    //UpdateSelectedOptions();
                    OnPrefabChanged(Prefab);
                    EventPrefabChanged?.Invoke(Prefab);
                }
            } else {
                if (isNetToolEnabled) {
                    Prefab = null;
                    isNetToolEnabled = false;
                    EventToolStateChanged?.Invoke(false);
                    NetworkSkinManager.instance.ClearActiveModifiers();
                }
            }
        }

        private void OnDestroy()
        {
            NetworkSkinManager.instance.EventSegmentPlaced -= OnSegmentPlaced;
        }

        private void OnPrefabChanged(NetInfo prefab)
        {
            _ignoreModifierEvents = true;

            TerrainSurface.OnPrefabChanged(prefab);

            Color.OnPrefabChanged(prefab);

            StreetLight.OnPrefabChanged(prefab);

            LeftTree.OnPrefabChanged(prefab);
            MiddleTree.OnPrefabChanged(prefab);
            RighTree.OnPrefabChanged(prefab);
            
            var elevatedPrefab = NetUtils.GetElevatedPrefab(prefab);
            ElevatedBridgePillar.OnPrefabChanged(elevatedPrefab);
            ElevatedMiddlePillar.OnPrefabChanged(elevatedPrefab);

            var bridgePrefab = NetUtils.GetBridgePrefab(prefab);
            BridgeBridgePillar.OnPrefabChanged(bridgePrefab);
            BridgeMiddlePillar.OnPrefabChanged(bridgePrefab);

            Catenary.OnPrefabChanged(prefab);

            _ignoreModifierEvents = false;

            UpdateActiveModifiers();
        }

        private void OnModifiersChanged()
        {
            if (_ignoreModifierEvents) return;

            UpdateActiveModifiers();
        }

        private void OnSegmentPlaced(NetworkSkin skin)
        {
            Color.OnSegmentPlaced(skin);
        }

        internal bool IsSelected(string id, ItemType type) {
            switch (type) {
                case ItemType.Trees: {
                    switch (LanePosition) {
                        case LanePosition.Left: return LeftTree.SelectedItem?.Id == id;
                        case LanePosition.Middle: return MiddleTree.SelectedItem?.Id == id;
                        case LanePosition.Right: return RighTree.SelectedItem?.Id == id;
                        default: return false;
                    }
                }
                case ItemType.Lights: return StreetLight.SelectedItem?.Id == id;
                case ItemType.Surfaces: return TerrainSurface.SelectedItem?.Id == id;
                case ItemType.Pillars: {
                    switch (PillarElevationCombination) {
                        case Pillar.Elevated: return ElevatedBridgePillar.SelectedItem?.Id == id;
                        case Pillar.ElevatedMiddle: return ElevatedMiddlePillar.SelectedItem?.Id == id;
                        case Pillar.Bridge: return BridgeBridgePillar.SelectedItem?.Id == id;
                        case Pillar.BridgeMiddle: return BridgeMiddlePillar.SelectedItem?.Id == id;
                        default: return false;
                    }
                }
                case ItemType.Catenary: return Catenary.SelectedItem?.Id == id;
                default: return false;
            }
        }

        private void UpdateActiveModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            MergeModifiers(modifiers, TerrainSurface.Modifiers);

            MergeModifiers(modifiers, Color.Modifiers);

            MergeModifiers(modifiers, StreetLight.Modifiers);

            MergeModifiers(modifiers, LeftTree.Modifiers);
            MergeModifiers(modifiers, MiddleTree.Modifiers);
            MergeModifiers(modifiers, RighTree.Modifiers);
            
            MergeModifiers(modifiers, ElevatedBridgePillar.Modifiers);
            MergeModifiers(modifiers, ElevatedMiddlePillar.Modifiers);
            MergeModifiers(modifiers, BridgeBridgePillar.Modifiers);
            MergeModifiers(modifiers, BridgeMiddlePillar.Modifiers);

            MergeModifiers(modifiers, Catenary.Modifiers);

            Debug.Log($"Built {modifiers.Values.Sum(p => p.Count)} modifiers for {modifiers.Count} prefabs.");

            NetworkSkinManager.instance.SetActiveModifiers(modifiers);
        }

        private static void MergeModifiers(Dictionary<NetInfo, List<NetworkSkinModifier>> mergedModifiers,
            Dictionary<NetInfo, List<NetworkSkinModifier>> featureModifiers)
        {
            foreach (var pair in featureModifiers)
            {
                if (mergedModifiers.TryGetValue(pair.Key, out var prefabModifiers))
                {
                    prefabModifiers.AddRange(pair.Value);
                }
                else
                {
                    mergedModifiers[pair.Key] = new List<NetworkSkinModifier>(pair.Value);
                }
            }
        }
    }
}
