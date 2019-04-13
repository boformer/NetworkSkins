using NetworkSkins.Net;
using NetworkSkins.Controller;
using System.Collections.Generic;
using static NetworkSkins.Controller.ItemListFeatureController<BuildingInfo>;

namespace NetworkSkins.GUI
{
    public class PillarList : ListBase<BuildingInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {
            SetupRowsData();
        }

        protected override bool IsFavourite(string itemID) {
            return Persistence.IsFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected override bool IsDefault(string itemID) {
            switch (SkinController.PillarElevationCombination) {
                case Pillar.Elevated: return SkinController.ElevatedBridgePillar.DefaultItem.Id == itemID;
                case Pillar.ElevatedMiddle: return SkinController.ElevatedMiddlePillar.DefaultItem.Id == itemID;
                case Pillar.Bridge: return SkinController.BridgeBridgePillar.DefaultItem.Id == itemID;
                case Pillar.BridgeMiddle: return SkinController.BridgeMiddlePillar.DefaultItem.Id == itemID;
                default: return false;
            }
        }

        protected override void SetupRowsData() {
            int itemCount = 0, selectedIndex = 0;
            if (fastList.RowsData == null) {
                fastList.RowsData = new FastList<object>();
            }
            fastList.RowsData.Clear();
            PillarFeatureController controller = null;
            switch (SkinController.PillarElevationCombination) {
                case Pillar.Elevated: controller = SkinController.ElevatedBridgePillar; break;
                case Pillar.ElevatedMiddle: controller = SkinController.ElevatedMiddlePillar; break;
                case Pillar.Bridge: controller = SkinController.BridgeBridgePillar; break;
                case Pillar.BridgeMiddle: controller = SkinController.BridgeMiddlePillar; break;
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
                BuildingInfo prefab = favouritesList[i] as BuildingInfo;
                ListItem listItem = CreateListItem(prefab);
                if (SkinController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                BuildingInfo prefab = nonFavouritesList[i] as BuildingInfo;
                ListItem listItem = CreateListItem(prefab);
                if (SkinController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            fastList.DisplayAt(selectedIndex);
            fastList.SelectedIndex = selectedIndex;
        }
    }
}