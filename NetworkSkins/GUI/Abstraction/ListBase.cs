using NetworkSkins.GUI.UIFastList;
using UnityEngine;

namespace NetworkSkins.GUI.Abstraction
{
    public abstract class ListBase : PanelBase
    {
        public delegate void FavouriteChangedEventHandler(string itemID, bool favourite);
        public event FavouriteChangedEventHandler EventFavouriteChanged;
        public delegate void SelectedChangedEventHandler(string itemID, bool selected);
        public event SelectedChangedEventHandler EventSelectedChanged;
        protected UIFastList.UIFastList fastList;
        protected abstract Vector2 ListSize { get; }
        protected abstract float RowHeight { get; }
        
        protected abstract bool IsFavourite(string itemID);
        protected abstract bool IsDefault(string itemID);

        public override void OnDestroy() {
            UnbindEvents();
            base.OnDestroy();
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            CreateFastList(ListSize, RowHeight);
            SetupRowsData();
            BindEvents();
            Refresh();
        }

        /// <summary>
        /// Use this method to set up the UIFastList RowsData.
        /// </summary>
        protected abstract void SetupRowsData();

        private void OnSelectedChanged(string itemID, bool selected) {
            EventSelectedChanged?.Invoke(itemID, selected);
        }

        private void OnFavouriteChanged(string itemID, bool favourite) {
            if (favourite) {
                Persistence.AddFavourite(itemID, UIUtil.PanelToItemType(PanelType));
            } else Persistence.RemoveFavourite(itemID, UIUtil.PanelToItemType(PanelType));
            EventFavouriteChanged?.Invoke(itemID, favourite);
        }

        private void CreateFastList(Vector2 size, float rowHeight) {
            fastList = UIFastList.UIFastList.Create<ListRow>(this);
            fastList.BackgroundSprite = "UnlockingPanel";
            fastList.size = size;
            fastList.RowHeight = rowHeight;
            fastList.CanSelect = true;
            fastList.AutoHideScrollbar = true;
        }

        private void BindEvents() {
            for (int rowIndex = 0; rowIndex < fastList.Rows.m_size; rowIndex++) {
                if (fastList.Rows[rowIndex] is ListRow row) {
                    row.EventSelectedChanged += OnSelectedChanged;
                    row.EventFavouriteChanged += OnFavouriteChanged;
                }
            }
        }

        private void UnbindEvents() {
            for (int rowIndex = 0; rowIndex < fastList.Rows.m_size; rowIndex++) {
                if (fastList.Rows[rowIndex] is ListRow row) {
                    row.EventSelectedChanged -= OnSelectedChanged;
                    row.EventFavouriteChanged -= OnFavouriteChanged;
                }
            }
        }
    }
}
