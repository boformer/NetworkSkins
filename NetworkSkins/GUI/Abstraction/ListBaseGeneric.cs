using ColossalFramework.UI;
using NetworkSkins.GUI.UIFastList;
using NetworkSkins.Locale;
using NetworkSkins.TranslationFramework;
using UnityEngine;

namespace NetworkSkins.GUI.Abstraction
{
    public abstract class ListBase<T> : ListBase
        where T : PrefabInfo
    {
        protected override Vector2 ListSize => new Vector2(390.0f, 500.0f);
        protected override float RowHeight => 50.0f;

        protected ListItem CreateListItem(T prefabInfo) {
            var id = prefabInfo == null ? "#NONE#" : prefabInfo.name;
            var isFavourite = IsFavourite(id);
            var isDefault = IsDefault(id);
            var prefix = isDefault
                ? string.Concat("(", Translation.Instance.GetTranslation(TranslationID.LABEL_DEFAULT), ") ")
                : string.Empty;
            var name = prefabInfo == null
                ? Translation.Instance.GetTranslation(TranslationID.LABEL_NONE)
                : prefabInfo.GetUncheckedLocalizedTitle();
            var displayName = string.Concat(prefix, name);
            var thumbnail = prefabInfo == null
                ? UIView.GetAView()?.defaultAtlas?.GetSpriteTexture("Niet")
                : prefabInfo.m_Atlas?.GetSpriteTexture(prefabInfo.m_Thumbnail);
            ItemType type = UIUtil.PanelToItemType(PanelType);
            return new ListItem(id, displayName, thumbnail, isFavourite, type);
        }
    }
}
