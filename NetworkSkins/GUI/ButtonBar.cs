using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ButtonBar : PanelBase
    {
        private NetToolMonitor Monitor => NetToolMonitor.Instance;
        private UIButton treesButton;
        private UIButton lightsButton;
        private UIButton surfaceButton;
        private UIButton pillarButton;
        private UIButton catenariesButton;
        private UIButton colorButton;
        private UIButton extrasButton;

        public override void Awake() {
            base.Awake();
            Monitor.EventPrefabChanged += OnPrefabChanged;
        }

        public override void OnDestroy() {
            base.OnDestroy();
            Monitor.EventPrefabChanged -= OnPrefabChanged;
        }

        public override void Build(Layout layout) {
            base.Build(layout);
            Vector2 buttonSize = new Vector2(layout.Size.x - layout.Spacing * 2, size.x - layout.Spacing * 2);
            treesButton = MakeButton(buttonSize, "T", "Trees");
            lightsButton = MakeButton(buttonSize, "L", "Lights");
            surfaceButton = MakeButton(buttonSize, "S", "Sidewalk");
            pillarButton = MakeButton(buttonSize, "P", "Pillars");
            catenariesButton = MakeButton(buttonSize, "C", "Catenary");
            colorButton = MakeButton(buttonSize, "C", "Color");
            extrasButton = MakeButton(buttonSize, "E", "Extra");
            UIPanel space = AddUIComponent<UIPanel>();
            space.size = new Vector2(0.1f, 0.0f);
            RefreshUI();
        }

        protected override void RefreshUI() {
            base.RefreshUI();
            treesButton.isVisible = Monitor.NetInfoHasTrees;
            lightsButton.isVisible = Monitor.NetInfoHasStreetLights;
            surfaceButton.isVisible = Monitor.NetInfoHasSurfaces;
            pillarButton.isVisible = Monitor.NetInfoHasPillars;
            catenariesButton.isVisible = Monitor.NetInfoHasCatenaries;
            colorButton.isVisible = Monitor.NetInfoIsColorable;
        }

        private void OnPrefabChanged(NetInfo netInfo) {
            RefreshUI();
        }
    }
}