using UnityEngine;

namespace NetworkSkins.GUI.UIFastList
{
    public class ListItem
    {
        /// <summary>
        /// This is the prefab name or other unique identifier.
        /// </summary>
        public readonly string ID;
        public readonly string DisplayName;
        public readonly Texture2D Thumbnail;
        public bool IsFavourite;
        public bool IsBlacklisted;
        public bool IsDefault;
        public ItemType Type;

        public ListItem(string id, string displayName, Texture2D thumbnail, bool isFavourite, bool isBlacklisted, bool isDefault, ItemType type) {
            ID = id;
            DisplayName = displayName;
            Thumbnail = thumbnail;
            IsFavourite = isFavourite;
            IsBlacklisted = isBlacklisted;
            IsDefault = isDefault;
            Type = type;
        }
    }
}
