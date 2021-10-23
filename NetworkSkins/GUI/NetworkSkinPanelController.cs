﻿using System;
using System.Collections.Generic;
using System.Linq;
using NetworkSkins.GUI.Catenaries;
using NetworkSkins.GUI.Custom;
using NetworkSkins.GUI.Colors;
using NetworkSkins.GUI.Lights;
using NetworkSkins.GUI.Pillars;
using NetworkSkins.GUI.RoadDecoration;
using NetworkSkins.GUI.Surfaces;
using NetworkSkins.GUI.Trees;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Tool;
using UnityEngine;

namespace NetworkSkins.GUI
{
    using NetworkSkins.API;
    using NetworkSkins.Skins.Modifiers;

    public class NetworkSkinPanelController : MonoBehaviour {
        public static NetworkSkinPanelController Instance;

        public delegate void ToolStateChangedEventHandler(bool state);
        public event ToolStateChangedEventHandler EventToolStateChanged;
        public delegate void GUIDirtyEventHandler(NetInfo netInfo);
        public event GUIDirtyEventHandler EventGUIDirty;

        public TerrainSurfacePanelController TerrainSurface;

        public ColorPanelController Color;

        public StreetLightPanelController StreetLight;

        public Dictionary<string, CustomPanelController> CustomPanelControllers = new Dictionary<string, CustomPanelController>();

        public bool TreesEnabled => LeftTree.Enabled || MiddleTree.Enabled || RighTree.Enabled;
        public LanePosition LanePosition { get; set; } = LanePosition.Left;
        public Pillar Pillar { get; set; } = Pillar.Bridge;

        public TreePanelController LeftTree;
        public TreePanelController MiddleTree;
        public TreePanelController RighTree;
        
        public bool PillarsEnabled => ElevatedBridgePillar.Enabled || ElevatedMiddlePillar.Enabled || BridgeBridgePillar.Enabled || BridgeMiddlePillar.Enabled;
        public PillarPanelController ElevatedBridgePillar;
        public PillarPanelController ElevatedMiddlePillar;
        public PillarPanelController BridgeBridgePillar;
        public PillarPanelController BridgeMiddlePillar;

        public CatenaryPanelController Catenary;

        public RoadDecorationPanelController RoadDecoration;

        public Dictionary<string, CustomPanelController> CustomControllers;

        public NetInfo Prefab { get; private set; }

        public PipetteTool Tool { get; private set; }

        private bool _ignoreModifierEvents = false;

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

            RoadDecoration = new RoadDecorationPanelController();
            RoadDecoration.EventModifiersChanged += OnModifiersChanged;

            
            foreach(var impl in NSAPI.Instance.ImplementationWrappers) {
                var cpc = CustomPanelControllers[impl.ID] = new CustomPanelController(impl);
                cpc.EventModifiersChanged += OnModifiersChanged;
            }

            Tool = ToolsModifierControl.toolController.gameObject.AddComponent<PipetteTool>();
            Tool.EventNetInfoPipetted += OnNetInfoPipetted;
        }

        private void OnNetInfoPipetted(NetInfo info) {
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
            Tool.EventNetInfoPipetted -= OnNetInfoPipetted;
            PipetteTool.Destroy();
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
            RoadDecoration.Reset();
            foreach(var controller in CustomControllers.Values)
                controller?.Reset();

            _ignoreModifierEvents = false;

            UpdateActiveModifiers();

            EventGUIDirty?.Invoke(Prefab);
        }

        public void SetPillarAndRefreshUI(Pillar pillar)
        {
            Pillar = pillar;
            ValidatePillar();
            EventGUIDirty?.Invoke(Prefab);
        }

        public void SetLaneAndRefreshUI(LanePosition value)
        {
            LanePosition = value;
            ValidateLane();
            EventGUIDirty?.Invoke(Prefab);
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
                    switch (Pillar)
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

        public bool IsDefault(string id, ItemType type) {
            switch (type) {
                case ItemType.Trees: {
                    switch (LanePosition) {
                        case LanePosition.Left: return LeftTree.DefaultItem?.Id == id;
                        case LanePosition.Middle: return MiddleTree.DefaultItem?.Id == id;
                        case LanePosition.Right: return RighTree.DefaultItem?.Id == id;
                        default: return false;
                    }
                }
                case ItemType.Lights: return StreetLight.DefaultItem?.Id == id;
                case ItemType.Surfaces: return TerrainSurface.DefaultItem?.Id == id;
                case ItemType.Pillars: {
                    switch (Pillar) {
                        case Pillar.Elevated: return ElevatedBridgePillar.DefaultItem?.Id == id;
                        case Pillar.ElevatedMiddle: return ElevatedMiddlePillar.DefaultItem?.Id == id;
                        case Pillar.Bridge: return BridgeBridgePillar.DefaultItem?.Id == id;
                        case Pillar.BridgeMiddle: return BridgeMiddlePillar.DefaultItem?.Id == id;
                        default: return false;
                    }
                }
                case ItemType.Catenary: return Catenary.DefaultItem?.Id == id;
                default: return false;
            }
        }

        public void OnPrefabWithModifiersSelected(NetInfo prefab, List<NetworkSkinModifier> modifiers)
        {
            _ignoreModifierEvents = true;

            TerrainSurface.OnPrefabWithModifiersSelected(prefab, modifiers);

            Color.OnPrefabWithModifiersSelected(prefab, modifiers);

            StreetLight.OnPrefabWithModifiersSelected(prefab, modifiers);

            LeftTree.OnPrefabWithModifiersSelected(prefab, modifiers);
            MiddleTree.OnPrefabWithModifiersSelected(prefab, modifiers);
            RighTree.OnPrefabWithModifiersSelected(prefab, modifiers);

            foreach(var controller in CustomControllers.Values) {
                controller?.OnPrefabWithModifiersSelected(prefab, modifiers);
            }


            var elevatedPrefab = NetUtils.GetElevatedPrefab(prefab);
            ElevatedBridgePillar.OnPrefabWithModifiersSelected(elevatedPrefab, modifiers);
            ElevatedMiddlePillar.OnPrefabWithModifiersSelected(elevatedPrefab, modifiers);

            var bridgePrefab = NetUtils.GetBridgePrefab(prefab);
            BridgeBridgePillar.OnPrefabWithModifiersSelected(bridgePrefab, modifiers);
            BridgeMiddlePillar.OnPrefabWithModifiersSelected(bridgePrefab, modifiers);

            Catenary.OnPrefabWithModifiersSelected(prefab, modifiers);

            RoadDecoration.OnPrefabWithModifiersSelected(prefab, modifiers);

            _ignoreModifierEvents = false;

            UpdateActiveModifiers();

            ValidateLane();
            ValidatePillar();

            EventGUIDirty?.Invoke(Prefab);
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

            foreach(var controller in CustomControllers.Values) {
                controller?.OnPrefabChanged(prefab);
            }

            var elevatedPrefab = NetUtils.GetElevatedPrefab(prefab);
            ElevatedBridgePillar.OnPrefabChanged(elevatedPrefab);
            ElevatedMiddlePillar.OnPrefabChanged(elevatedPrefab);

            var bridgePrefab = NetUtils.GetBridgePrefab(prefab);
            BridgeBridgePillar.OnPrefabChanged(bridgePrefab);
            BridgeMiddlePillar.OnPrefabChanged(bridgePrefab);

            Catenary.OnPrefabChanged(prefab);

            RoadDecoration.OnPrefabChanged(prefab);

            _ignoreModifierEvents = false;

            UpdateActiveModifiers();

            ValidateLane();
            ValidatePillar();

            EventGUIDirty?.Invoke(Prefab);
        }

        private void ValidateLane() {
            switch (LanePosition) {
                case LanePosition.Left: {
                    if (!LeftTree.Enabled) {
                        LanePosition = MiddleTree.Enabled ? LanePosition.Middle : LanePosition.Right;
                    }
                    break;
                }
                case LanePosition.Middle: {
                    if (!MiddleTree.Enabled) {
                        LanePosition = LeftTree.Enabled ? LanePosition.Left : LanePosition.Right;
                    }
                    break;
                }
                case LanePosition.Right: {
                    if (!RighTree.Enabled) {
                        LanePosition = MiddleTree.Enabled ? LanePosition.Middle : LanePosition.Left;
                    }
                    break;
                }
            }
        }

        private void ValidatePillar() {
            switch (Pillar) {
                case Pillar.Elevated: {
                    if (!ElevatedBridgePillar.Enabled) {
                        Pillar = ElevatedMiddlePillar.Enabled ? Pillar.ElevatedMiddle : BridgeBridgePillar.Enabled ? Pillar.Bridge : Pillar.BridgeMiddle;
                    }
                    break;
                }
                case Pillar.ElevatedMiddle: {
                    if (!ElevatedMiddlePillar.Enabled) {
                        Pillar = ElevatedBridgePillar.Enabled ? Pillar.Elevated : BridgeBridgePillar.Enabled ? Pillar.Bridge : Pillar.BridgeMiddle;
                    }
                    break;
                }
                case Pillar.Bridge: {
                    if (!BridgeBridgePillar.Enabled) {
                        Pillar = ElevatedMiddlePillar.Enabled ? Pillar.ElevatedMiddle : ElevatedBridgePillar.Enabled ? Pillar.Elevated : Pillar.BridgeMiddle;
                    }
                    break;
                }
                case Pillar.BridgeMiddle: {
                    if (!BridgeMiddlePillar.Enabled) {
                        Pillar = ElevatedMiddlePillar.Enabled ? Pillar.ElevatedMiddle : BridgeBridgePillar.Enabled ? Pillar.Bridge : Pillar.Elevated;
                    }
                    break;
                }
            }
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

            MergeModifiers(modifiers, RoadDecoration.Modifiers);

            foreach(var cpc in CustomPanelControllers.Values) {
                MergeCustom(modifiers, cpc.CustomDatas);
            }

            //Debug.Log($"Built {modifiers.Values.Sum(p => p.Count)} modifiers for {modifiers.Count} prefabs.");

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

        private static void MergeCustom(Dictionary<NetInfo, List<NetworkSkinModifier>> mergedModifiers,
            Dictionary<NetInfo, CustomDataCollectionModifier> datas) {
            foreach(var pair in datas) {
                List<NetworkSkinModifier> prefabModifiers;
                if(!mergedModifiers.TryGetValue(pair.Key, out prefabModifiers)) {
                    prefabModifiers = mergedModifiers[pair.Key] = new List<NetworkSkinModifier>();
                }

                var customModifier = prefabModifiers.OfType<CustomDataCollectionModifier>().FirstOrDefault();
                if(customModifier == null) {
                    prefabModifiers.Add(pair.Value);
                } else {
                    foreach(var pair2 in pair.Value.Data) {
                        customModifier.Data.Add(pair2.Key, pair2.Value);
                    }
                }

            }
        }
    }
}
