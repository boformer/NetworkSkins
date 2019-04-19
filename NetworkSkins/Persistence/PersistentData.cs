using NetworkSkins.GUI;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace NetworkSkins.Persistence
{
    [XmlRoot("NetworkSkinsSettings")]
    public class PersistentData
    {
        public bool SaveActiveSelectionGlobally { get; set; } = true;
        public bool LanePositionLocked { get; set; } = false;
        public Vector2 ToolbarPosition { get; set; } = new Vector2(100.0f, 100.0f);
        public List<string>[] Favourites { get; set; } = new List<string>[(int)ItemType.Count];
        public List<Color32> Swatches { get; set; } = new List<Color32>(12);
        public List<KeyValuePair<ActiveSelectionData.ValueKey, string>> GlobalActiveSelectionData { get; set; } = new List<KeyValuePair<ActiveSelectionData.ValueKey, string>>();

        public PersistentData() {
            for (int i = 0; i < (int)ItemType.Count; i++) {
                Favourites[i] = new List<string>();
            }
        }

        public void OnPreSerialize() {
            GlobalActiveSelectionData.Clear();
            foreach (var kvp in ActiveSelectionData.Instance.GetGlobalData()) {
                GlobalActiveSelectionData.Add(kvp);
            }
        }

        public void OnPostDeserialize() {
            ActiveSelectionData.Instance.SetGlobalData(GlobalActiveSelectionData);
        }
    }
}
