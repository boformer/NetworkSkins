namespace NetworkSkins.GUI
{
    public class TreeList : ListBase<TreeInfo>
    {
        protected override bool IsFavourite(TreeInfo prefabInfo) {
            return false;
        }

        protected override bool IsSelected(TreeInfo prefabInfo) {
            return false;
        }

        protected override void OnFavouriteChanged(string itemID, bool favourite) {

        }

        protected override void OnSelectedChanged(string itemID, bool selected) {

        }

        protected override void SetupRowsData() {
            int prefabCount, selectedIndex = 0;
            fastList.RowsData = new FastList<object>();
            prefabCount = PrefabCollection<TreeInfo>.LoadedCount();
            fastList.RowsData.SetCapacity(prefabCount + 1);
            ListItem noneItem = CreateListItem(null);
            fastList.RowsData.Add(noneItem);
            for (uint prefabIndex = 0; prefabIndex < prefabCount; prefabIndex++) {
                TreeInfo prefab = PrefabCollection<TreeInfo>.GetLoaded(prefabIndex);
                ListItem listItem = CreateListItem(prefab);
                if (listItem.IsSelected) selectedIndex = (int)prefabIndex + 1;
                fastList.RowsData.Add(listItem);
            }
            fastList.DisplayAt(selectedIndex);
        }
    }
}