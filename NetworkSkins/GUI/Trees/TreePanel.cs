using NetworkSkins.Net;
using System.Linq;

namespace NetworkSkins.GUI
{
    public class TreePanel : ListPanelBase<TreeList, TreeInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            SetTabEnabled(LanePosition.Left, SkinController.LeftTree.Enabled);
            SetTabEnabled(LanePosition.Middle, SkinController.MiddleTree.Enabled);
            SetTabEnabled(LanePosition.Right, SkinController.RighTree.Enabled);
            int tabCount = laneTabs.Count(tab => tab.isVisible);
            if (tabCount != 0) {
                for (int i = 0; i < LanePositionExtensions.LanePositionCount; i++) {
                    laneTabs[i].width = laneTabStrip.width / tabCount;
                }
            }
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
            RefreshAfterBuild();
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {
            if (!selected) return;
            switch (SkinController.LanePosition) {
                case LanePosition.Left: SkinController.LeftTree.OnSelectedItemChanged(itemID); break;
                case LanePosition.Middle: SkinController.MiddleTree.OnSelectedItemChanged(itemID); break;
                case LanePosition.Right: SkinController.RighTree.OnSelectedItemChanged(itemID); break;
                default: break;
            }
        }
    }
}
