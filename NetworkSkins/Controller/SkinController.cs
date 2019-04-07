using System.Collections.Generic;
using NetworkSkins.Controller;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using UnityEngine;

namespace NetworkSkins
{


    public class SkinController : MonoBehaviour {
        public static SkinController Instance;

        public delegate void ToolStateChangedEventHandler(bool state);
        public event ToolStateChangedEventHandler EventToolStateChanged;
        public delegate void PrefabChangedEventHandler(NetInfo netInfo);
        public event PrefabChangedEventHandler EventPrefabChanged;

        public bool _ignoreEvents = false;

        public bool TreesEnabled => LeftTree.Enabled || MiddleTree.Enabled || RighTree.Enabled;
        public readonly TreeFeatureController LeftTree = new TreeFeatureController(LanePosition.Left);
        public readonly TreeFeatureController MiddleTree = new TreeFeatureController(LanePosition.Middle);
        public readonly TreeFeatureController RighTree = new TreeFeatureController(LanePosition.Right);


        public bool HasStreetLights { get; private set; } = false;
        public PropInfo DefaultStreetLight { get; private set; } = null;
        public PropInfo SelectedStreetLight { get; private set; } = null;
        public PropInfo[] StreetLights { get; private set; } = new PropInfo[0];

        public bool HasSurfaces { get; private set; } = false;
        public Surface DefaultSurface { get; private set; } = Surface.Pavement;
        public Surface SelectedSurface { get; private set; } = Surface.Pavement;
        public Surface[] Surfaces { get; private set; } = new Surface[0];

        public bool HasBridgePillar { get; private set; } = false;
        public BuildingInfo SelectedBridgePillar { get; private set; } = null;
        public BuildingInfo DefaultBridgePillar { get; private set; } = null;

        public bool HasMiddlePillar { get; private set; } = false;




        // pillars, color, catenary, extras


        public bool NetInfoHasTrees => NetUtil.HasTrees(Prefab);
        public bool NetInfoHasCatenaries => NetUtil.HasCatenaries(Prefab);
        public bool NetInfoHasPillars => NetUtil.HasPillars(Prefab);
        public bool NetInfoHasSurfaces => NetUtil.HasSurfaces(Prefab);
        public bool NetInfoHasStreetLights => NetUtil.HasStreetLights(Prefab);
        public bool NetInfoIsColorable => NetUtil.IsColorable(Prefab);

        public NetInfo Prefab { get; private set; }

        private bool isNetToolEnabled;

        public bool NetInfoDefaultEquals<Info>(Info prefab) where Info : PrefabInfo {
            // TODO return whether the passed prefab is the default for current net
            return false;
        }

        /// <summary>
        /// LanePostion.None is passed when the "Lock lanes" options is enabled.
        /// </summary>
        /// <param name="prefabID">The prefab name usable to get the tree from PrefabCollection<TreeInfo></param>
        /// <param name="lanePosition"></param>
        public void SetTree(string prefabID, LanePosition lanePosition)
        {
        }

        public void SetLight(string prefabID) {

        }

        public void SetPillar(string prefabID) {

        }

        public void SetCatenary(string prefabID) {

        }

        public void SetSurface(Surface surface) {

        }

        public void SetColor(Color32 color) {

        }

        private void Awake() {
            Instance = this;

            LeftTree.EventChanged += OnFeatureChanged;
            MiddleTree.EventChanged += OnFeatureChanged;
            RighTree.EventChanged += OnFeatureChanged;
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
                    isNetToolEnabled = false;
                    EventToolStateChanged?.Invoke(false);
                }
            }
        }

        private void OnPrefabChanged(NetInfo prefab)
        {
            _ignoreEvents = true;
            LeftTree.OnPrefabChanged(prefab);
            MiddleTree.OnPrefabChanged(prefab);
            RighTree.OnPrefabChanged(prefab);
            _ignoreEvents = false;

            UpdateActiveModifiers();
        }

        private void OnFeatureChanged()
        {
            if (_ignoreEvents) return;

            UpdateActiveModifiers();
        }

        private void UpdateActiveModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();
            modifiers.Merge(LeftTree.Modifiers);
            modifiers.Merge(MiddleTree.Modifiers);
            modifiers.Merge(RighTree.Modifiers);

            Debug.Log($"Built {modifiers.Count} modifiers");

            NetworkSkinManager.instance.SetActiveModifiers(modifiers);
        }
    }
}
