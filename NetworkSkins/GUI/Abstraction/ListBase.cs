using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class ListBase : PanelBase
    {
        protected UIFastList fastList;

        public override void OnDestroy() {
            base.OnDestroy();
            UnbindEvents();
        }

        public override void Build(Layout layout) {
            base.Build(layout);
            CreateFastList();
            SetupRowsData();
            BindEvents();
        }

        private void CreateFastList() {
            fastList = UIFastList.Create<ListRow>(this);
            fastList.BackgroundSprite = "UnlockingPanel";
            fastList.size = new Vector2(390.0f, 500.0f);
            fastList.RowHeight = 50.0f;
            fastList.CanSelect = true;
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

        /// <summary>
        /// Use this method to set up the UIFastList RowsData.
        /// </summary>
        protected abstract void SetupRowsData();

        /// <summary>
        /// Event that's invoked when the prefab is selected to be used in the skin.
        /// </summary>
        /// <param name="itemID">The PrefabInfo.name, used as a unique identifier.</param>
        /// <param name="selected"></param>
        protected abstract void OnSelectedChanged(string itemID, bool selected);

        /// <summary>
        /// Event that's invoked when the favourite checkbox's state changes.
        /// </summary>
        /// <param name="itemID">The PrefabInfo.name, used as a unique identifier.</param>
        /// <param name="favourite"></param>
        protected abstract void OnFavouriteChanged(string itemID, bool favourite);
    }

    public abstract class ListBase<T> : ListBase
        where T : PrefabInfo
    {
        /// <summary>
        /// Used to check whether the PrefabInfo is currently being used in the skin.
        /// </summary>
        /// <param name="prefabInfo"></param>
        /// <returns></returns>
        protected abstract bool IsSelected(T prefabInfo);

        /// <summary>
        /// Used to check whether the PrefabInfo is currently saved as a favourite.
        /// </summary>
        /// <param name="prefabInfo"></param>
        /// <returns></returns>
        protected abstract bool IsFavourite(T prefabInfo);

        protected ListItem CreateListItem(T prefabInfo) {
            Texture2D thumbnail;
            string id, displayName, prefix, name;
            bool isFavourite, isDefault, isSelected;
            isSelected = IsSelected(prefabInfo);
            isFavourite = IsFavourite(prefabInfo);
            isDefault = IsDefault(prefabInfo);
            prefix = isDefault
                ? Translation.Instance.GetTranslation(TranslationID.LABEL_DEFAULT)
                : string.Empty;
            name = prefabInfo == null
                ? Translation.Instance.GetTranslation(TranslationID.LABEL_NONE)
                : prefabInfo.GetUncheckedLocalizedTitle();
            id = prefabInfo == null ? "None" : prefabInfo.name;
            displayName = string.Concat(prefix, name);
            thumbnail = prefabInfo == null
                ? UIView.GetAView()?.defaultAtlas?.GetSpriteTexture("Niet")
                : prefabInfo.m_Atlas?.GetSpriteTexture(prefabInfo.m_Thumbnail);
            return new ListItem(id, displayName, thumbnail, isSelected, isFavourite);
        }

        private static bool IsDefault(T prefabInfo) {
            return NetToolMonitor.Instance.NetInfoDefaultEquals(prefabInfo);
        }
    }
}
