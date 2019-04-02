using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public class PillarList : ListBase<BuildingInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {

            Debug.LogWarning(PillarUtils.GetDefaultBridgePillar(netInfo));
        }
        protected override bool IsFavourite(BuildingInfo prefabInfo) {
            return false;
        }

        protected override bool IsSelected(BuildingInfo prefabInfo) {
            return false;
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {

        }

        protected override void SetupRowsData() {
            int prefabCount, selectedIndex = 0;
            fastList.RowsData = new FastList<object>();
            prefabCount = PrefabCollection<BuildingInfo>.LoadedCount();
            fastList.RowsData.SetCapacity(prefabCount + 1);
            ListItem noneItem = CreateListItem(null);
            fastList.RowsData.Add(noneItem);
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++) {
                BuildingInfo prefab = PrefabCollection<BuildingInfo>.GetLoaded(prefabIndex);
                if (true/*NetUtil.IsPillar(prefab)*/) {
                    ListItem listItem = CreateListItem(prefab);
                    if (listItem.IsSelected) selectedIndex = (int)prefabIndex + 1;
                    fastList.RowsData.Add(listItem);
                }
            }
            fastList.DisplayAt(selectedIndex);
        }
    }
}