using ColossalFramework.UI;
using NetworkSkins.Persistence;
using UnityEngine;

namespace NetworkSkins.GUI.Abstraction
{
    public class PanelBase : UIPanel
    {
        public delegate void DragEndEventHandler();
        public Layout Layout { get; set; }
        public int Spacing { get; set; }
        public static Color32 GUIColor { get; set; } = new Color32(128, 128, 128, 255);
        public static Color32 FocusedColor = new Color32(88, 181, 205, 255);
        public PanelType PanelType { get; private set; }
        protected NetworkSkinPanelController NetworkSkinPanelController => NetworkSkinPanelController.Instance;
        protected PersistenceService Persistence => PersistenceService.Instance;


        public override void Awake() {
            base.Awake();
            if (NetworkSkinPanelController != null) {
                NetworkSkinPanelController.EventGUIDirty += OnPrefabChanged;
            }
        }

        public override void OnDestroy() {
            NetworkSkinPanelController.EventGUIDirty -= OnPrefabChanged;
            base.OnDestroy();
        }
        public virtual void Build(PanelType panelType, Layout layout) {
            PanelType = panelType;
            Layout = layout;
            Layout.Apply(this);
        }

        protected virtual void RefreshUI(NetInfo netInfo) {

        }

        protected void Refresh() {
            RefreshUI(NetworkSkinPanelController.Prefab);
        }

        private void OnPrefabChanged(NetInfo netInfo) {
            RefreshUI(netInfo);
        }
    }
}
