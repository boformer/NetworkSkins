using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class CatenaryList : ListBase<PropInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {

        }
        protected override bool IsFavourite(PropInfo prefabInfo) {
            return false;
        }

        protected override bool IsSelected(PropInfo prefabInfo) {
            return false;
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {

        }

        protected override void SetupRowsData() {
            int prefabCount, selectedIndex = 0;
            fastList.RowsData = new FastList<object>();
            prefabCount = PrefabCollection<PropInfo>.LoadedCount();
            fastList.RowsData.SetCapacity(prefabCount + 1);
            ListItem noneItem = CreateListItem(null);
            fastList.RowsData.Add(noneItem);
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++) {
                PropInfo prefab = PrefabCollection<PropInfo>.GetLoaded(prefabIndex);
                if (CatenaryUtils.IsCatenaryProp(prefab)) {
                    ListItem listItem = CreateListItem(prefab);
                    if (listItem.IsSelected) selectedIndex = (int)prefabIndex + 1;
                    fastList.RowsData.Add(listItem);
                }
            }
            fastList.DisplayAt(selectedIndex);
        }
    }
}