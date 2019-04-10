using System.Collections.Generic;
using static NetworkSkins.Controller.ItemListFeatureController<PropInfo>;


namespace NetworkSkins.GUI
{
    public class CatenaryList : ListBase<PropInfo>
    {
        public void RefreshRowsData() {
            SetupRowsData();
        }

        protected override void RefreshUI(NetInfo netInfo) {
            SetupRowsData();
        }
        protected override bool IsFavourite(string itemID) {
            return Persistence.IsFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected override bool IsDefault(string itemID) {
            return SkinController.Catenary.DefaultItem.Id == itemID;
        }

        protected override void SetupRowsData() {
            int itemCount, selectedIndex = 0;
            if (fastList.RowsData == null) {
                fastList.RowsData = new FastList<object>();
            }
            fastList.RowsData.Clear();
            itemCount = SkinController.Catenary.Items.Count;
            fastList.RowsData.SetCapacity(itemCount);
            favouritesList.Clear();
            nonFavouritesList.Clear();
            int index = 0;
            List<string> favList = Persistence.GetFavourites(UIUtil.PanelToItemType(PanelType));
            foreach (SimpleItem item in SkinController.Catenary.Items) {
                if (item.Id == "#NONE#") {
                    ListItem listItem = CreateListItem(null);
                    if (SkinController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                    fastList.RowsData.Add(listItem);
                    index++;
                    continue;
                }
                if (favList.Contains(item.Id)) {
                    favouritesList.Add(item.Value);
                } else nonFavouritesList.Add(item.Value);
            }
            for (int i = 0; i < favouritesList.Count; i++) {
                PropInfo prefab = favouritesList[i] as PropInfo;
                ListItem listItem = CreateListItem(prefab);
                if (SkinController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                PropInfo prefab = nonFavouritesList[i] as PropInfo;
                ListItem listItem = CreateListItem(prefab);
                if (SkinController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            fastList.DisplayAt(selectedIndex);
        }
    }
}