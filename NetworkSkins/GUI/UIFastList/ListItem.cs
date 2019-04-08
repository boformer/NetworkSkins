using UnityEngine;

namespace NetworkSkins.GUI
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
        public ItemType Type;

        public ListItem(string id, string displayName, Texture2D thumbnail, bool isFavourite, ItemType type) {
            ID = id;
            DisplayName = displayName;
            Thumbnail = thumbnail;
            IsFavourite = isFavourite;
            Type = type;
        }
    }
}
