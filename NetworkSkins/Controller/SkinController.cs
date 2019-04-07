using System.Collections.Generic;
using System.Linq;
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

        private bool _ignoreModifierEvents = false;

        public bool TreesEnabled => LeftTree.Enabled || MiddleTree.Enabled || RighTree.Enabled;
        public readonly TreeFeatureController LeftTree = new TreeFeatureController(LanePosition.Left);
        public readonly TreeFeatureController MiddleTree = new TreeFeatureController(LanePosition.Middle);
        public readonly TreeFeatureController RighTree = new TreeFeatureController(LanePosition.Right);

        public readonly StreetLightFeatureController StreetLight = new StreetLightFeatureController();

        public readonly CatenaryFeatureController Catenary = new CatenaryFeatureController();

        public bool HasSurfaces { get; private set; } = false;
        public Surface DefaultSurface { get; private set; } = Surface.Pavement;
        public Surface SelectedSurface { get; private set; } = Surface.Pavement;
        public Surface[] Surfaces { get; private set; } = new Surface[0];

        public bool HasBridgePillar { get; private set; } = false;
        public BuildingInfo SelectedBridgePillar { get; private set; } = null;
        public BuildingInfo DefaultBridgePillar { get; private set; } = null;

        public bool HasMiddlePillar { get; private set; } = false;

        // pillars, color, catenary, extras


        public bool NetInfoHasCatenaries => NetUtil.HasCatenaries(Prefab);
        public bool NetInfoHasPillars => NetUtil.HasPillars(Prefab);
        public bool NetInfoHasSurfaces => NetUtil.HasSurfaces(Prefab);
        public bool NetInfoIsColorable => NetUtil.IsColorable(Prefab);
        public bool NetInfoCanHaveNoneSurface => NetUtil.CanHaveNoneSurface(Prefab);

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

            LeftTree.EventModifiersChanged += OnModifiersChanged;
            MiddleTree.EventModifiersChanged += OnModifiersChanged;
            RighTree.EventModifiersChanged += OnModifiersChanged;
            StreetLight.EventModifiersChanged += OnModifiersChanged;
            Catenary.EventModifiersChanged += OnModifiersChanged;
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
            _ignoreModifierEvents = true;

            LeftTree.OnPrefabChanged(prefab);
            MiddleTree.OnPrefabChanged(prefab);
            RighTree.OnPrefabChanged(prefab);
            StreetLight.OnPrefabChanged(prefab);
            Catenary.OnPrefabChanged(prefab);

            _ignoreModifierEvents = false;

            UpdateActiveModifiers();
        }

        private void OnModifiersChanged()
        {
            if (_ignoreModifierEvents) return;

            UpdateActiveModifiers();
        }

        private void UpdateActiveModifiers()
        {
            var modifiers = new Dictionary<NetInfo, List<NetworkSkinModifier>>();

            MergeModifiers(modifiers, LeftTree.Modifiers);
            MergeModifiers(modifiers, MiddleTree.Modifiers);
            MergeModifiers(modifiers, RighTree.Modifiers);
            MergeModifiers(modifiers, StreetLight.Modifiers);
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
