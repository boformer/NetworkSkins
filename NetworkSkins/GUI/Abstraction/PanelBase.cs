using ColossalFramework.UI;
using NetworkSkins.Persistence;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class PanelBase : UIPanel
    {
        public delegate void DragEndEventHandler();
        public Layout Layout { get; set; }
        public int Spacing { get; set; }
        public static Color32 GUIColor { get; set; } = new Color32(128, 128, 128, 255);
        public static Color32 FocusedColor = new Color32(88, 181, 205, 255);
        public PanelType PanelType { get; private set; }
        protected SkinController SkinController => SkinController.Instance;
        protected PersistenceService Persistence => PersistenceService.Instance;


        public override void Awake() {
            base.Awake();
            if (SkinController != null) {
                SkinController.EventPrefabChanged += OnPrefabChanged;
            }
        }

        public override void OnDestroy() {
            SkinController.EventPrefabChanged -= OnPrefabChanged;
            base.OnDestroy();
        }
        public virtual void Build(PanelType panelType, Layout layout) {
            PanelType = panelType;
            Layout = layout;
            Layout.Apply(this);
        }

        protected abstract void RefreshUI(NetInfo netInfo);

        protected virtual void Refresh() {
            RefreshUI(SkinController.Prefab);
        }

        private void OnPrefabChanged(NetInfo netInfo) {
            RefreshUI(netInfo);
        }
    }
}
