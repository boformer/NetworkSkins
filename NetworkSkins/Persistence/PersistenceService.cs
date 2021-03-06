﻿using ColossalFramework.IO;
using NetworkSkins.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace NetworkSkins.Persistence
{
    public class PersistenceService : MonoBehaviour
    {
        public static PersistenceService Instance { get; set; }

        public bool SaveActiveSelectionGlobally {
            get => Data.SaveActiveSelectionGlobally;
            set {
                Data.SaveActiveSelectionGlobally = value;
                SaveData();
            }
        }
        public bool HideBlacklisted {
            get => Data.HideBlacklisted;
            set {
                Data.HideBlacklisted = value;
                SaveData();
            }
        }
        public bool DisplayAtSelected {
            get => Data.DisplayAtSelected;
            set {
                Data.DisplayAtSelected = value;
                SaveData();
            }
        }

        private const string FILE_NAME = "NetworkSkinsSettings.xml";
        private string FilePath => Path.Combine(DataLocation.localApplicationData, FILE_NAME);

        private PersistentData _data;
        private PersistentData Data {
            get {
                if (_data == null) {
                    _data = LoadData() ?? new PersistentData();
                }
                return _data;
            }
        }

        public List<SavedSwatch> GetSavedSwatches() {
            return new List<SavedSwatch>(Data.SavedSwatches);
        }
        public void UpdateSavedSwatches(List<SavedSwatch> savedSwatches) {
            Data.SavedSwatches = new List<SavedSwatch>(savedSwatches);
            SaveData();
        }

        public List<Color32> GetSwatches() {
            return new List<Color32>(Data.Swatches);
        }

        public void UpdateSwatches(List<Color32> swatches) {
            Data.Swatches = new List<Color32>(swatches);
            SaveData();
        }

        public void AddFavourite(string itemID, ItemType itemType) {
            if (itemType == ItemType.None) return;
            if (!Data.Favourites[(int)itemType].Contains(itemID)) {
                Data.Favourites[(int)itemType].Add(itemID);
                SaveData();
            }
        }
        public void RemoveFavourite(string itemID, ItemType itemType) {
            if (itemType == ItemType.None) return;
            if (Data.Favourites[(int)itemType].Contains(itemID)) {
                Data.Favourites[(int)itemType].Remove(itemID);
                SaveData();
            }
        }

        public List<string> GetFavourites(ItemType itemType) {
            return Data.Favourites[(int)itemType];
        }

        public bool IsFavourite(string name, ItemType itemType) {
            return Data.Favourites[(int)itemType].Contains(name);
        }


        public void AddToBlacklist(string itemID, ItemType itemType) {
            if (itemType == ItemType.None) return;
            if (!Data.Blacklisted[(int)itemType].Contains(itemID)) {
                Data.Blacklisted[(int)itemType].Add(itemID);
                SaveData();
            }
        }

        public void RemoveFromBlacklist(string itemID, ItemType itemType) {
            if (itemType == ItemType.None) return;
            if (Data.Blacklisted[(int)itemType].Contains(itemID)) {
                Data.Blacklisted[(int)itemType].Remove(itemID);
                SaveData();
            }
        }

        public List<string> GetBlacklisted(ItemType itemType) {
            return Data.Blacklisted[(int)itemType];
        }

        public bool IsBlacklisted(string name, ItemType itemType) {
            return Data.Blacklisted[(int)itemType].Contains(name);
        }

        public Vector2? GetToolbarPosition() {
            return Data.ToolbarPosition;
        }

        public void SetToolbarPosition(Vector3? position) {
            Data.ToolbarPosition = position;
            SaveData();
        }

        public bool LanePositionLocked {
            get {
                return Data.LanePositionLocked;
            }
            set {
                Data.LanePositionLocked = value;
                SaveData();
            }
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

        public class SavedSwatch
        {
            public Color Color;
            public string Name;
        }
    }
}
