using NetworkSkins.Controller;
using NetworkSkins.Net;
using System.Collections.Generic;
using static NetworkSkins.Controller.ListPanelController<TreeInfo>;

namespace NetworkSkins.GUI
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

        protected override void SetupRowsData() {
            int itemCount, selectedIndex = 0;
            if (fastList.RowsData == null) {
                fastList.RowsData = new FastList<object>();
            }
            fastList.RowsData.Clear();
            TreePanelController controller = null;
            switch (NetworkSkinPanelController.LanePosition) {
                case LanePosition.Left: controller = NetworkSkinPanelController.LeftTree; break;
                case LanePosition.Middle: controller = NetworkSkinPanelController.MiddleTree; break;
                case LanePosition.Right: controller = NetworkSkinPanelController.RighTree; break;
            }
            itemCount = controller.Items.Count;
            fastList.RowsData.SetCapacity(itemCount);
            favouritesList.Clear();
            nonFavouritesList.Clear();
            int index = 0;
            List<string> favList = Persistence.GetFavourites(UIUtil.PanelToItemType(PanelType));
            foreach (SimpleItem item in controller.Items) {
                if (item.Id == "#NONE#") {
                    ListItem listItem = CreateListItem(null);
                    if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                    fastList.RowsData.Add(listItem);
                    index++;
                    continue;
                }
                if (favList.Contains(item.Id)) {
                    favouritesList.Add(item.Value);
                } else nonFavouritesList.Add(item.Value);
            }
            for (int i = 0; i < favouritesList.Count; i++) {
                TreeInfo prefab = favouritesList[i] as TreeInfo;
                ListItem listItem = CreateListItem(prefab);
                if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                TreeInfo prefab = nonFavouritesList[i] as TreeInfo;
                ListItem listItem = CreateListItem(prefab);
                if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            fastList.DisplayAt(selectedIndex);
            fastList.SelectedIndex = selectedIndex;
        }
    }
}