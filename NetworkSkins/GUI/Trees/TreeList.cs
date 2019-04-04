using System.Collections.Generic;

namespace NetworkSkins.GUI
{
    public class TreeList : ListBase<TreeInfo>
    {
        protected override void RefreshUI(NetInfo netInfo) {

        }

        protected override bool IsFavourite(TreeInfo prefabInfo) {
            return Persistence.IsFavourite(prefabInfo?.name, PanelType);
        }

        protected override bool IsSelected(TreeInfo prefabInfo) {
            return false; // TODO
        }

        protected override void SetupRowsData() {
            int prefabCount, selectedIndex = 0;
            fastList.RowsData = new FastList<object>();
            prefabCount = PrefabCollection<TreeInfo>.LoadedCount();
            fastList.RowsData.SetCapacity(prefabCount + 1);
            ListItem noneItem = CreateListItem(null);
            fastList.RowsData.Add(noneItem);
            favouritesList.Clear();
            nonFavouritesList.Clear();
            List<string> favList = Persistence.GetFavourites(PanelType);
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++) {
                TreeInfo prefab = PrefabCollection<TreeInfo>.GetLoaded(prefabIndex);
                if (favList.Contains(prefab.name)) {
                    favouritesList.Add(prefab);
                } else nonFavouritesList.Add(prefab);
            }
            favouritesList.Sort((t1, t2) => t1.name.CompareTo(t2.name));
            nonFavouritesList.Sort((t1, t2) => t1.name.CompareTo(t2.name));
            int index = 0;
            for (int i = 0; i < favouritesList.Count; i++) {
                index++;
                TreeInfo prefab = favouritesList[i] as TreeInfo;
                ListItem listItem = CreateListItem(prefab);
                if (listItem.IsSelected) selectedIndex = index + 1;
                fastList.RowsData.Add(listItem);
            }
            for (int i = 0; i < nonFavouritesList.Count; i++) {
                index++;
                TreeInfo prefab = nonFavouritesList[i] as TreeInfo;
                ListItem listItem = CreateListItem(prefab);
                if (listItem.IsSelected) selectedIndex = index + 1;
                fastList.RowsData.Add(listItem);
            }
            fastList.DisplayAt(selectedIndex);
        }
    }
}