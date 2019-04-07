using System.Linq;
using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class TreePanel : ListPanelBase<TreeList, TreeInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            SetTabEnabled(LanePosition.Left, SkinController.LeftTree.Enabled);
            SetTabEnabled(LanePosition.Middle, SkinController.MiddleTree.Enabled);
            SetTabEnabled(LanePosition.Right, SkinController.RighTree.Enabled);

            int tabCount = tabs.Count(tab => tab.isVisible);

            if (tabCount != 0) {
                for (int i = 0; i < LanePositionExtensions.LanePositionCount; i++) {
                    tabs[i].width = tabStrip.width / tabCount;
                }
            }
        }

        private void SetTabEnabled(LanePosition lanePos, bool enabled)
        {
            tabs[(int) lanePos].isVisible = enabled;
        }

        protected override void OnSearchLostFocus() {

        }

        protected override void OnSearchTextChanged(string text) {

        }

        protected override void OnPanelBuilt() {
            RefreshAfterBuild();
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if(!selected) return;

            var lanePosition = GetLanePosition();
            if (lanePosition == LanePosition.Left)
            {
                //SkinController.LeftTree.OnSelectedItemChanged();
            }
            else if (lanePosition == LanePosition.Middle)
            {
                //SkinController.MiddleTree.OnSelectedItemChanged();
            }
            else if (lanePosition == LanePosition.Right)
            {
                //SkinController.RighTree.OnSelectedItemChanged();
            }
        }

        private LanePosition GetLanePosition() {
            return (LanePosition)tabStrip.selectedIndex;
        }
    }
}
