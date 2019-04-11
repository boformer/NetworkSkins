using ColossalFramework.IO;
using NetworkSkins.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace NetworkSkins.Persistence
{
    public class PersistenceService : MonoBehaviour {
        public static PersistenceService Instance { get; set; }
        private const string FILE_NAME = "NetworkSkinsSettings.xml";

        private string FilePath => Path.Combine(DataLocation.localApplicationData, FILE_NAME);
        private PersistentData Data {
            get {
                if (_data == null) {
                    _data = LoadData() ?? new PersistentData();
                }
                return _data;
            }
        }

        private PersistentData _data;

        public void RemoveFavourite(string itemID, ItemType itemType) {
            if (Data.Favourites[(int)itemType].Contains(itemID)) {
                Data.Favourites[(int)itemType].Remove(itemID);
                SaveData();
            }
        }

        public List<Color32> GetSwatches() {
            return new List<Color32>(Data.Swatches);
        }

        public void UpdateSwatches(List<Color32> swatches)
        {
            Data.Swatches = new List<Color32>(swatches);
            SaveData();
        }

        public void AddFavourite(string itemID, ItemType itemType) {
            if (!Data.Favourites[(int)itemType].Contains(itemID)) {
                Data.Favourites[(int)itemType].Add(itemID);
                SaveData();
            }
        }

        public List<string> GetFavourites(ItemType itemType) {
            return Data.Favourites[(int)itemType];
        }

        public bool IsFavourite(string name, ItemType itemType) {
            return Data.Favourites[(int)itemType].Contains(name);
        }

        public Vector3 GetToolbarPosition() {
            return Data.ToolbarPosition;
        }

        public void SetToolbarPosition(Vector3 position) {
            Data.ToolbarPosition = position;
            SaveData();
        }

        public void SaveData() {
            string fileName = FilePath;
            PersistentData data = Data;
            XmlSerializer serializer = new XmlSerializer(typeof(PersistentData));
            using (StreamWriter writer = new StreamWriter(fileName)) {
                data.OnPreSerialize();
                serializer.Serialize(writer, data);
            }
        }

        public PersistentData LoadData() {
            string fileName = FilePath;
            XmlSerializer serializer = new XmlSerializer(typeof(PersistentData));
            try {
                using (StreamReader reader = new StreamReader(fileName)) {
                    var data = serializer.Deserialize(reader) as PersistentData;
                    data.OnPostDeserialize();
                    return data;
                }
            } catch (Exception) {
                return null;
            }
        }

        private void Awake() {
            Instance = this;
        }
    }
}
