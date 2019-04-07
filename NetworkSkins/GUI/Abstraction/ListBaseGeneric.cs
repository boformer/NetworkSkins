using ColossalFramework.UI;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI
{
    public abstract class ListBase<T> : ListBase
        where T : PrefabInfo
    {
        protected override Vector2 ListSize => new Vector2(390.0f, 500.0f);
        protected override float RowHeight => 50.0f;

        protected abstract bool IsSelected(T prefabInfo);
        protected abstract bool IsFavourite(T prefabInfo);
        protected abstract bool IsDefault(T prefabInfo);

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
            id = prefabInfo == null ? "#None#" : prefabInfo.name;
            displayName = string.Concat(prefix, name);
            thumbnail = prefabInfo == null
                ? UIView.GetAView()?.defaultAtlas?.GetSpriteTexture("Niet")
                : prefabInfo.m_Atlas?.GetSpriteTexture(prefabInfo.m_Thumbnail);
            return new ListItem(id, displayName, thumbnail, isSelected, isFavourite);
        }
    }
}
