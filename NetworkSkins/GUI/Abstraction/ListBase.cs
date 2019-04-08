using NetworkSkins.Net;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class ListBase : PanelBase
    {
        public delegate void FavouriteChangedEventHandler(string itemID, bool favourite);
        public event FavouriteChangedEventHandler EventFavouriteChanged;
        public delegate void SelectedChangedEventHandler(string itemID, bool selected);
        public event SelectedChangedEventHandler EventSelectedChanged;
        protected UIFastList fastList;
        protected abstract Vector2 ListSize { get; }
        protected abstract float RowHeight { get; }
        protected static List<PrefabInfo> favouritesList = new List<PrefabInfo>();
        protected static List<PrefabInfo> nonFavouritesList = new List<PrefabInfo>();
        
        protected abstract bool IsFavourite(string itemID);
        protected abstract bool IsDefault(string itemID);

        public override void OnDestroy() {
            base.OnDestroy();
            UnbindEvents();
        }

        public override void Build(PanelType panelType, Layout layout) {
            base.Build(panelType, layout);
            CreateFastList(ListSize, RowHeight);
            SetupRowsData();
            BindEvents();
            RefreshAfterBuild();
        }

        public void Select(string itemID) {
            foreach (ListRow item in fastList.Rows) {
                item.UpdateColor(itemID);
            }
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
            fastList = UIFastList.Create<ListRow>(this);
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
