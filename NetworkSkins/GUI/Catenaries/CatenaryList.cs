using NetworkSkins.Net;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class CatenaryList : ListBase<PropInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {

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
            prefabCount = PrefabCollection<PropInfo>.LoadedCount();
            fastList.RowsData.SetCapacity(prefabCount + 1);
            ListItem noneItem = CreateListItem(null);
            fastList.RowsData.Add(noneItem);
            favouritesList.Clear();
            nonFavouritesList.Clear();
            List<string> favList = Persistence.GetFavourites(UIUtil.PanelToItemType(PanelType));
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++) {
                PropInfo prefab = PrefabCollection<PropInfo>.GetLoaded(prefabIndex);
                if (CatenaryUtils.IsCatenaryProp(prefab)) {
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
                PropInfo prefab = favouritesList[i] as PropInfo;
                ListItem listItem = CreateListItem(prefab);
                if (listItem.IsSelected) selectedIndex = index + 1;
                fastList.RowsData.Add(listItem);
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                index++;
                PropInfo prefab = nonFavouritesList[i] as PropInfo;
                ListItem listItem = CreateListItem(prefab);
                if (listItem.IsSelected) selectedIndex = index + 1;
                fastList.RowsData.Add(listItem);
            }
            fastList.DisplayAt(selectedIndex);
        }
    }
}