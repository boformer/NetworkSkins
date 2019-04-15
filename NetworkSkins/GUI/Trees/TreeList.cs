using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Net;

namespace NetworkSkins.GUI.Trees
{
    public class TreeList : ListBase<TreeInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            SetupRowsData();
        }

        protected override bool IsFavourite(string itemID) {
            return Persistence.IsFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected override bool IsDefault(string itemID) {
            switch (NetworkSkinPanelController.LanePosition) {
                case LanePosition.Left: return NetworkSkinPanelController.LeftTree.DefaultItem.Id == itemID;
                case LanePosition.Middle: return NetworkSkinPanelController.MiddleTree.DefaultItem.Id == itemID;
                case LanePosition.Right: return NetworkSkinPanelController.RighTree.DefaultItem.Id == itemID;
                default: return false;
            }
        }
    }
}