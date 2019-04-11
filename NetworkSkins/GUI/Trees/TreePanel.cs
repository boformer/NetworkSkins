using NetworkSkins.Net;
using System.Linq;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class TreePanel : ListPanelBase<TreeList, TreeInfo>
    {
        private DistancePanel distancePanel;

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            distancePanel = AddUIComponent<DistancePanel>();
            distancePanel.Build(panelType, new Layout(new Vector2(390.0f, 0.0f), true, ColossalFramework.UI.LayoutDirection.Vertical, ColossalFramework.UI.LayoutStart.TopLeft, 5));
            SkinController.EventLaneChanged += OnLaneChanged;
        }

        private void OnLaneChanged(LanePosition lane) {
            list.RefreshRowsData();
            distancePanel.RefreshSlider();
        }

        protected override void RefreshUI(NetInfo netInfo) {
            laneTabStrip.isVisible = true;
            SetTabEnabled(LanePosition.Right, SkinController.RighTree.Enabled);
            SetTabEnabled(LanePosition.Middle, SkinController.MiddleTree.Enabled);
            SetTabEnabled(LanePosition.Left, SkinController.LeftTree.Enabled);
            int tabCount = laneTabs.Count(tab => tab.isVisible);
            if (tabCount != 0) {
                for (int i = 0; i < LanePositionExtensions.LanePositionCount; i++) {
                laneTabs[i].width = laneTabStrip.width / tabCount;
                }
            }
            list.RefreshRowsData();
            if (tabCount == 1) {
                laneTabStrip.isVisible = false;
            }
        }

        private void SetTabEnabled(LanePosition lanePos, bool enabled) {
            laneTabs[(int)lanePos].isVisible = enabled;
            if (enabled) {
                laneTabStrip.selectedIndex = (int)lanePos;
                laneTabs[(int)lanePos].Focus();
            } 
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
            switch (SkinController.LanePosition) {
                case LanePosition.Left: SkinController.LeftTree.SetSelectedItem(itemID); break;
                case LanePosition.Middle: SkinController.MiddleTree.SetSelectedItem(itemID); break;
                case LanePosition.Right: SkinController.RighTree.SetSelectedItem(itemID); break;
                default: break;
            }
            list.Select(itemID);
        }
    }
}
