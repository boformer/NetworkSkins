using System;
using System.Collections.Generic;
using System.Linq;
using NetworkSkins.GUI.Catenaries;
using NetworkSkins.GUI.Colors;
using NetworkSkins.GUI.Lights;
using NetworkSkins.GUI.Pillars;
using NetworkSkins.GUI.Surfaces;
using NetworkSkins.GUI.Trees;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class NetworkSkinPanelController : MonoBehaviour {
        public static NetworkSkinPanelController Instance;

        public delegate void ToolStateChangedEventHandler(bool state);
        public event ToolStateChangedEventHandler EventToolStateChanged;
        public delegate void PrefabChangedEventHandler(NetInfo netInfo);
        public event PrefabChangedEventHandler EventPrefabChanged;

        private bool _ignoreModifierEvents = false;
        public bool TabClicked { get; set; } = false;

        public TerrainSurfacePanelController TerrainSurface;

        public ColorPanelController Color;

        public StreetLightPanelController StreetLight;

        public bool TreesEnabled => LeftTree.Enabled || MiddleTree.Enabled || RighTree.Enabled;
        public LanePosition LanePosition { get; set; } = LanePosition.Left;

        public TreePanelController LeftTree;
        public TreePanelController MiddleTree;
        public TreePanelController RighTree;
        
        public bool PillarsEnabled => ElevatedBridgePillar.Enabled || ElevatedMiddlePillar.Enabled || BridgeBridgePillar.Enabled || BridgeMiddlePillar.Enabled;
        public Pillar PillarElevationCombination { get; set; } = Pillar.Bridge;
        public PillarPanelController ElevatedBridgePillar;
        public PillarPanelController ElevatedMiddlePillar;
        public PillarPanelController BridgeBridgePillar;
        public PillarPanelController BridgeMiddlePillar;

        public CatenaryPanelController Catenary;

        public NetInfo Prefab { get; private set; }

        #region Lifecycle
        public void Awake() {
            Instance = this;

            NetworkSkinManager.instance.EventSegmentPlaced += OnSegmentPlaced;

            TerrainSurface = new TerrainSurfacePanelController();
            TerrainSurface.EventModifiersChanged += OnModifiersChanged;

            Color = new ColorPanelController();
            Color.EventModifiersChanged += OnModifiersChanged;

            StreetLight = new StreetLightPanelController();
            StreetLight.EventModifiersChanged += OnModifiersChanged;

            LeftTree = new TreePanelController(LanePosition.Left);
            LeftTree.EventModifiersChanged += OnModifiersChanged;

            MiddleTree = new TreePanelController(LanePosition.Middle);
            MiddleTree.EventModifiersChanged += OnModifiersChanged;

            RighTree = new TreePanelController(LanePosition.Right);
            RighTree.EventModifiersChanged += OnModifiersChanged;
            
            var availablePillars = PillarUtils.GetAvailablePillars();

            ElevatedBridgePillar = new PillarPanelController(PillarType.Bridge, availablePillars);
            ElevatedBridgePillar.EventModifiersChanged += OnModifiersChanged;

            ElevatedMiddlePillar = new PillarPanelController(PillarType.Middle, availablePillars);
            ElevatedMiddlePillar.EventModifiersChanged += OnModifiersChanged;

            BridgeBridgePillar = new PillarPanelController(PillarType.Bridge, availablePillars);
            BridgeBridgePillar.EventModifiersChanged += OnModifiersChanged;

            BridgeMiddlePillar = new PillarPanelController(PillarType.Middle, availablePillars);
            BridgeMiddlePillar.EventModifiersChanged += OnModifiersChanged;

            Catenary = new CatenaryPanelController();
            Catenary.EventModifiersChanged += OnModifiersChanged;
        }

        public void Update()
        {
            if (ToolsModifierControl.toolController.CurrentTool is NetTool netTool && netTool.Prefab != null) {
                if (Prefab == null) {
                    EventToolStateChanged?.Invoke(true);
                }

                if (netTool.Prefab != Prefab)
                {
                    Prefab = netTool.m_prefab;
                    OnPrefabChanged(Prefab);
                    EventPrefabChanged?.Invoke(Prefab);
                }
            }
            else
            {
                if (Prefab != null)
                {
                    Prefab = null;
                    EventToolStateChanged?.Invoke(false);
                    NetworkSkinManager.instance.ClearActiveModifiers();
                }
            }
        }

        public void OnDestroy()
        {
            NetworkSkinManager.instance.EventSegmentPlaced -= OnSegmentPlaced;
        }
        #endregion

        public void OnReset() {
            _ignoreModifierEvents = true;

            TerrainSurface.Reset();
            Color.Reset();
            StreetLight.Reset();
            LeftTree.Reset();
            MiddleTree.Reset();
            RighTree.Reset();
            ElevatedBridgePillar.Reset();
            ElevatedMiddlePillar.Reset();
            BridgeBridgePillar.Reset();
            BridgeMiddlePillar.Reset();
            Catenary.Reset();

            _ignoreModifierEvents = false;

            UpdateActiveModifiers();
        }

        public void SetActivePillarElevation(Pillar pillar)
        {
            PillarElevationCombination = pillar;
            EventPrefabChanged?.Invoke(Prefab);
        }

        public void SetActiveLane(LanePosition value)
        {
            LanePosition = value;
            EventPrefabChanged?.Invoke(Prefab);
        }

        public bool IsSelected(string id, ItemType type)
        {
            switch (type)
            {
                case ItemType.Trees:
                {
                    switch (LanePosition)
                    {
                        case LanePosition.Left: return LeftTree.SelectedItem?.Id == id;
                        case LanePosition.Middle: return MiddleTree.SelectedItem?.Id == id;
                        case LanePosition.Right: return RighTree.SelectedItem?.Id == id;
                        default: return false;
                    }
                }
                case ItemType.Lights: return StreetLight.SelectedItem?.Id == id;
                case ItemType.Surfaces: return TerrainSurface.SelectedItem?.Id == id;
                case ItemType.Pillars:
                {
                    switch (PillarElevationCombination)
                    {
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

            EventPrefabChanged?.Invoke(Prefab);
        }

        private void OnSegmentPlaced(NetworkSkin skin)
        {
            Color.OnSegmentPlaced(skin);
        }

        private void OnModifiersChanged()
        {
            if (_ignoreModifierEvents) return;

            UpdateActiveModifiers();
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
