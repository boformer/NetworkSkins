using ColossalFramework.UI;
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
        public readonly UITextureAtlas ThumbnailAtlas;
        public readonly string ThumbnailSprite;
        public readonly Texture2D ThumbnailTexture;
        public bool IsFavourite;
        public bool IsBlacklisted;
        public bool IsDefault;
        public ItemType Type;
        public Color LightColor;

        public ListItem(string id, string displayName, UITextureAtlas thumbnailAtlas, string thumbnailSprite, Texture2D thumbnailTexture, bool isFavourite, bool isBlacklisted, bool isDefault, ItemType type, Color color) {
            ID = id;
            DisplayName = displayName;
            ThumbnailAtlas = thumbnailAtlas;
            ThumbnailSprite = thumbnailSprite;
            ThumbnailTexture = thumbnailTexture;
            IsFavourite = isFavourite;
            IsBlacklisted = isBlacklisted;
            IsDefault = isDefault;
            Type = type;
            LightColor = color;
        }
    }
}
