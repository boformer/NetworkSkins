using ColossalFramework.UI;
using NetworkSkins.GUI.UIFastList;
using UnityEngine;

namespace NetworkSkins.GUI.Abstraction
{
    public abstract class ListBase : PanelBase
    {
        public delegate void ItemClickEventHandler(string itemID);
        public event ItemClickEventHandler EventItemClick;

        protected UIFastList.UIFastList fastList;
        protected abstract Vector2 ListSize { get; }
        protected abstract float RowHeight { get; }

        protected bool IsDefault(string itemID) {
            return NetworkSkinPanelController.IsDefault(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected bool IsFavourite(string itemID) {
            return Persistence.IsFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        protected bool IsBlacklisted(string itemID) {
            return Persistence.IsBlacklisted(itemID, UIUtil.PanelToItemType(PanelType)) && !IsDefault(itemID);
        }



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

        private void OnItemClick(UIComponent component, int itemIndex)
        {
            ListItem item = fastList.RowsData[itemIndex] as ListItem;
            if (item != null)
            {
                EventItemClick?.Invoke(item.ID);
            }
        }

        private void OnFavouriteChanged(string itemID, bool favourite) {
            if (favourite) {
                Persistence.AddFavourite(itemID, UIUtil.PanelToItemType(PanelType));
            } else Persistence.RemoveFavourite(itemID, UIUtil.PanelToItemType(PanelType));
        }

        private void OnBlacklistedChanged(string itemID, bool favourite) {
            if (favourite) {
                Persistence.AddToBlacklist(itemID, UIUtil.PanelToItemType(PanelType));
            } else Persistence.RemoveFromBlacklist(itemID, UIUtil.PanelToItemType(PanelType));
        }

        private void CreateFastList(Vector2 size, float rowHeight) {
            fastList = UIFastList.UIFastList.Create<ListRow>(this);
            fastList.BackgroundSprite = "UnlockingPanel";
            fastList.size = size;
            fastList.RowHeight = rowHeight;
            fastList.CanSelect = true;
            fastList.AutoHideScrollbar = true;
            fastList.EventItemClick += OnItemClick;
        }

        private void BindEvents() {
            for (int rowIndex = 0; rowIndex < fastList.Rows.m_size; rowIndex++) {
                if (fastList.Rows[rowIndex] is ListRow row) {
                    //row.EventSelectedChanged += OnSelectedChanged;
                    row.EventFavouriteChanged += OnFavouriteChanged;
                    row.EventBlacklistedChanged += OnBlacklistedChanged;
                }
            }
        }

        private void UnbindEvents() {
            for (int rowIndex = 0; rowIndex < fastList.Rows.m_size; rowIndex++) {
                if (fastList.Rows[rowIndex] is ListRow row) {
                    //row.EventSelectedChanged -= OnSelectedChanged;
                    row.EventFavouriteChanged -= OnFavouriteChanged;
                    row.EventBlacklistedChanged -= OnBlacklistedChanged;
                }
            }
        }
    }
}
