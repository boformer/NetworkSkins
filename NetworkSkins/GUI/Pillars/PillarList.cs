using NetworkSkins.Net;
using NetworkSkins.Controller;
using System.Collections.Generic;
using static NetworkSkins.Controller.ListPanelController<BuildingInfo>;

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
            switch (NetworkSkinPanelController.PillarElevationCombination) {
                case Pillar.Elevated: return NetworkSkinPanelController.ElevatedBridgePillar.DefaultItem.Id == itemID;
                case Pillar.ElevatedMiddle: return NetworkSkinPanelController.ElevatedMiddlePillar.DefaultItem.Id == itemID;
                case Pillar.Bridge: return NetworkSkinPanelController.BridgeBridgePillar.DefaultItem.Id == itemID;
                case Pillar.BridgeMiddle: return NetworkSkinPanelController.BridgeMiddlePillar.DefaultItem.Id == itemID;
                default: return false;
            }
        }

        protected override void SetupRowsData() {
            int itemCount = 0, selectedIndex = 0;
            if (fastList.RowsData == null) {
                fastList.RowsData = new FastList<object>();
            }
            fastList.RowsData.Clear();
            PillarPanelController controller = null;
            switch (NetworkSkinPanelController.PillarElevationCombination) {
                case Pillar.Elevated: controller = NetworkSkinPanelController.ElevatedBridgePillar; break;
                case Pillar.ElevatedMiddle: controller = NetworkSkinPanelController.ElevatedMiddlePillar; break;
                case Pillar.Bridge: controller = NetworkSkinPanelController.BridgeBridgePillar; break;
                case Pillar.BridgeMiddle: controller = NetworkSkinPanelController.BridgeMiddlePillar; break;
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
                BuildingInfo prefab = favouritesList[i] as BuildingInfo;
                ListItem listItem = CreateListItem(prefab);
                if (NetworkSkinPanelController.IsSelected(listItem.ID, listItem.Type)) selectedIndex = index;
                fastList.RowsData.Add(listItem);
                index++;
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                BuildingInfo prefab = nonFavouritesList[i] as BuildingInfo;
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