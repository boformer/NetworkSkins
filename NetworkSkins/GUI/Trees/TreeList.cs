using System.Collections.Generic;
using static NetworkSkins.Controller.ItemListFeatureController<TreeInfo>;

namespace NetworkSkins.GUI
{
    public class TreeList : ListBase<TreeInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
        }

        protected override bool IsFavourite(string itemID) {
            return Persistence.IsFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected override bool IsSelected(string itemID) {
            return SkinController.LeftTree.Enabled
                ? SkinController.LeftTree.SelectedItem.Id == itemID
                : SkinController.MiddleTree.Enabled
                ? SkinController.MiddleTree.SelectedItem.Id == itemID
                : false;
        }

        protected override bool IsDefault(string itemID) {
            return itemID == SkinController.DefaultTree().name;
        }

        protected override void SetupRowsData() {
            int itemCount, selectedIndex = 0;
            fastList.RowsData = new FastList<object>();
            itemCount = SkinController.LeftTree.Items.Count;
            fastList.RowsData.SetCapacity(itemCount);
            favouritesList.Clear();
            nonFavouritesList.Clear();
            int index = 0;
            List<string> favList = Persistence.GetFavourites(UIUtil.PanelToItemType(PanelType));
            foreach (SimpleItem item in SkinController.LeftTree.Items) {
                if (item.Id == "#NONE#") {
                    ListItem listItem = CreateListItem(null);
                    if (listItem.IsSelected) selectedIndex = index;
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
                if (listItem.IsSelected) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                TreeInfo prefab = nonFavouritesList[i] as TreeInfo;
                ListItem listItem = CreateListItem(prefab);
                if (listItem.IsSelected) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            fastList.DisplayAt(selectedIndex);
        }
    }
}