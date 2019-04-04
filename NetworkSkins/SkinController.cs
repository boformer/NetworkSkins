using NetworkSkins.Net;
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

        public bool NetInfoHasTrees => NetUtil.HasTrees(Prefab);
        public bool NetInfoHasCatenaries => NetUtil.HasCatenaries(Prefab);
        public bool NetInfoHasPillars => NetUtil.HasPillars(Prefab);
        public bool NetInfoIsColorable => NetUtil.IsColorable(Prefab);
        public bool NetInfoHasSurfaces => NetUtil.HasSurfaces(Prefab);
        public bool NetInfoHasStreetLights => NetUtil.HasStreetLights(Prefab);

        public NetInfo Prefab { get; private set; }
        private bool isNetToolEnabled;

        public bool NetInfoDefaultEquals<Info>(Info prefab) where Info : PrefabInfo {
            return false;
        }

        /// <summary>
        /// LanePostion.None is passed when the "Lock lanes" options is enabled.
        /// </summary>
        /// <param name="prefabID">The prefab name usable to get the tree from PrefabCollection<TreeInfo></param>
        /// <param name="lanePosition"></param>
        public void SetTree(string prefabID, LanePosition lanePosition) {

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
                    EventPrefabChanged?.Invoke(Prefab);
                }
            } else {
                if (isNetToolEnabled) {
                    isNetToolEnabled = false;
                    EventToolStateChanged?.Invoke(false);
                }
            }
        }
    }
}
