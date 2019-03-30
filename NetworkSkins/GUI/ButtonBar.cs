using ColossalFramework.UI;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class ButtonBar : PanelBase
    {
        private UIButton treesButton;
        private UIButton lightsButton;
        private UIButton surfaceButton;
        private UIButton pillarButton;
        private UIButton catenariesButton;
        private UIButton colorButton;
        private UIButton extrasButton;

        public override void Build(Layout layout) {
            base.Build(layout);
            Vector2 buttonSize = new Vector2(layout.Size.x - layout.Spacing * 2, size.x - layout.Spacing * 2);
            treesButton = MakeButton(buttonSize, "InfoIconEntertainment");
            lightsButton = MakeButton(buttonSize, "IconPolicyEvenMoreFun");
            surfaceButton = MakeButton(buttonSize, "SubBarDistrictSpecializationPaint");
            pillarButton = MakeButton(buttonSize, "InfoIconTours");
            catenariesButton = MakeButton(buttonSize, "Options");
            colorButton = MakeButton(buttonSize, "ToolbarIconProps");
            extrasButton = MakeButton(buttonSize, "Options");
            UIPanel space = AddUIComponent<UIPanel>();
            space.size = new Vector2(0.1f, 0.0f);
        }

        public override void Update() {
            base.Update();
            if (ToolsModifierControl.toolController.CurrentTool is NetTool netTool && MainPanel.Prefab != netTool.m_prefab) {
                MainPanel.Prefab = netTool.m_prefab;
                RefreshUI();
            }
        }

        protected override void RefreshUI() {
            base.RefreshUI();
            treesButton.isVisible = HasTrees();
        }

        private bool HasTrees() {
            if (MainPanel.Prefab == null || MainPanel.Prefab.m_lanes == null) return false;

            foreach (NetInfo.Lane lane in MainPanel.Prefab.m_lanes)
                if (lane?.m_laneProps?.m_props != null)
                    foreach (var laneProp in lane.m_laneProps.m_props) {
                        if (laneProp?.m_finalTree != null) return true;
                    }
            return false;
        }
    }
}