using NetworkSkins.Net;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class PillarList : ListBase<BuildingInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {

            Debug.LogWarning(PillarUtils.GetDefaultBridgePillar(netInfo));
        }
        protected override bool IsFavourite(string itemID) {
            return false;
        }

        protected override bool IsSelected(string itemID) {
            return false;
        }

        protected override bool IsDefault(string itemID) {
            return false;
        }

        protected override void SetupRowsData() {
            int prefabCount, selectedIndex = 0;
            fastList.RowsData = new FastList<object>();
            prefabCount = PrefabCollection<BuildingInfo>.LoadedCount();
            fastList.RowsData.SetCapacity(prefabCount + 1);
            ListItem noneItem = CreateListItem(null);
            fastList.RowsData.Add(noneItem);
            favouritesList.Clear();
            nonFavouritesList.Clear();
            List<string> favList = Persistence.GetFavourites(UIUtil.PanelToItemType(PanelType));
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++) {
                BuildingInfo prefab = PrefabCollection<BuildingInfo>.GetLoaded(prefabIndex);
                if (true/*NetUtil.IsPillar(prefab)*/) {// TODO
                    if (favList.Contains(prefab.name)) {
                        favouritesList.Add(prefab);
                    } else nonFavouritesList.Add(prefab);
                }
            }
            favouritesList.Sort((t1, t2) => t1.name.CompareTo(t2.name));
            nonFavouritesList.Sort((t1, t2) => t1.name.CompareTo(t2.name));
            int index = 0;
            for (int i = 0; i < favouritesList.Count; i++) {
                index++;
                BuildingInfo prefab = favouritesList[i] as BuildingInfo;
                ListItem listItem = CreateListItem(prefab);
                if (listItem.IsSelected) selectedIndex = index + 1;
                fastList.RowsData.Add(listItem);
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                index++;
                BuildingInfo prefab = nonFavouritesList[i] as BuildingInfo;
                ListItem listItem = CreateListItem(prefab);
                if (listItem.IsSelected) selectedIndex = index + 1;
                fastList.RowsData.Add(listItem);
            }
            fastList.DisplayAt(selectedIndex);
        }
    }
}