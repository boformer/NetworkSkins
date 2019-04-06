using NetworkSkins.GUI;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace NetworkSkins.Persistence
{
    [XmlRoot("NetworkSkinsSettings")]
    public class PersistentData
    {
        public Vector2 ToolbarPosition { get; set; } = new Vector2(100.0f, 100.0f);
        public List<string>[] Favourites { get; set; } = new List<string>[(int)ItemType.Count];
        public List<Color32> Swatches { get; internal set; } = new List<Color32>(20);

        public PersistentData() {
            for (int i = 0; i < (int)ItemType.Count; i++) {
                Favourites[i] = new List<string>();
            }
        }

        public void OnPreSerialize() { }

        public void OnPostDeserialize() { }
    }
}
