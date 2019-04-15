using System.Linq;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.GUI.Trees
{
    public class TreePanel : ListPanelBase<TreeList, TreeInfo>
    {
        private DistancePanel distancePanel;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            distancePanel = AddUIComponent<DistancePanel>();
            distancePanel.Build(panelType, new Layout(new Vector2(390.0f, 0.0f), true, ColossalFramework.UI.LayoutDirection.Vertical, ColossalFramework.UI.LayoutStart.TopLeft, 5));
        }

        protected override void RefreshUI(NetInfo netInfo) {
            laneTabStrip.isVisible = true;
            SetTabEnabled(LanePosition.Right, NetworkSkinPanelController.RighTree.Enabled);
            SetTabEnabled(LanePosition.Middle, NetworkSkinPanelController.MiddleTree.Enabled);
            SetTabEnabled(LanePosition.Left, NetworkSkinPanelController.LeftTree.Enabled);
            int tabCount = laneTabs.Count(tab => tab.isVisible);
            if (tabCount != 0) {
                for (int i = 0; i < LanePositionExtensions.LanePositionCount; i++) {
                    laneTabs[i].width = laneTabStrip.width / tabCount;
                }
            }
            if (tabCount == 1) {
                laneTabStrip.isVisible = false;
                laneTabStrip.selectedIndex = 1;
            } else if (!NetworkSkinPanelController.TabClicked) laneTabStrip.selectedIndex = 0;
            NetworkSkinPanelController.TabClicked = false;
        }

        private void SetTabEnabled(LanePosition lanePos, bool enabled) {
            laneTabs[(int)lanePos].isVisible = enabled;
        }

        protected override void OnSearchLostFocus() {

        }

        protected override void OnSearchTextChanged(string text) {

        }

        protected override void OnPanelBuilt() {
            pillarTabStrip.isVisible = false;
            Refresh();
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            switch (NetworkSkinPanelController.LanePosition) {
                case LanePosition.Left: NetworkSkinPanelController.LeftTree.SetSelectedItem(itemID); break;
                case LanePosition.Middle: NetworkSkinPanelController.MiddleTree.SetSelectedItem(itemID); break;
                case LanePosition.Right: NetworkSkinPanelController.RighTree.SetSelectedItem(itemID); break;
                default: break;
            }
        }
    }
}
