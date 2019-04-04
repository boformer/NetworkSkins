using ColossalFramework.UI;
using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class TreePanel : ListPanelBase<TreeList, TreeInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            Vector2 size = Vector2.zero;
            int lanesWithTrees = 0;
            for (int i = 0; i < (int)LanePosition.Count; i++) {
                bool hasTree = NetUtil.HasTreesInLane(netInfo, (LanePosition)i);
                tabs[i].isVisible = hasTree;
                if (hasTree) ++lanesWithTrees;
            }
            if (lanesWithTrees != 0) {
                for (int i = 0; i < (int)LanePosition.Count; i++) {
                    tabs[i].width = tabStrip.width / lanesWithTrees;
                }
            }
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
            if(selected) SkinController.SetTree(itemID, GetLanePosition());
        }

        private LanePosition GetLanePosition() {
            return (LanePosition)tabStrip.selectedIndex;
        }
    }
}
