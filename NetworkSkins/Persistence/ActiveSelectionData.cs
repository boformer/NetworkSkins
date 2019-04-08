using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ColossalFramework.IO;
using ICities;
using UnityEngine;

namespace NetworkSkins.Controller
{
    public class ActiveSelectionData : SerializableDataExtensionBase
    {
        public static ActiveSelectionData Instance { get; private set; }

        private const string DataKey = "NetworkSkins_ACTIVE_SELECTION";
        private const int DataVersion = 0;

        private readonly Dictionary<ValueKey, string> _values = new Dictionary<ValueKey, string>();

        #region Lifecycle
        public override void OnCreated(ISerializableData serializableData)
        {
            base.OnCreated(serializableData);
            Instance = this;
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            byte[] data = serializableDataManager.LoadData(DataKey);
            if (data == null) return;

            Data dataContainer;
            using (var stream = new MemoryStream(data))
            {
                dataContainer = DataSerializer.Deserialize<Data>(stream, DataSerializer.Mode.Memory);
            }

            foreach (var pair in dataContainer.Values)
            {
                _values[pair.Key] = pair.Value;
            }
        }

        public override void OnSaveData()
        {
            base.OnSaveData();

            var dataContainer = new Data {Values = _values};

            byte[] data;
            using (var stream = new MemoryStream())
            {
                DataSerializer.Serialize(stream, DataSerializer.Mode.Memory, DataVersion, dataContainer);
                data = stream.ToArray();
            }
            serializableDataManager.SaveData(DataKey, data);
        }

        public override void OnReleased()
        {
            base.OnReleased();
            Instance = null;
        }
        #endregion

        public string GetValue(NetInfo prefab, string key)
        {
            return _values.TryGetValue(new ValueKey(prefab.name, key), out var value) ? value : null;
        }

        public void SetValue(NetInfo prefab, string key, string value)
        {
            _values[new ValueKey(prefab.name, key)] = value;
        }

        public float? GetFloatValue(NetInfo prefab, string key)
        {
            var value = GetValue(prefab, key);
            if (value == null) return null;

            try
            {
                return float.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                Debug.LogError($"Error while parsing {key} float value for {prefab.name}");
                Debug.LogException(e);
                return null;
            }
        }

        public void SetFloatValue(NetInfo prefab, string key, float value)
        {
            _values[new ValueKey(prefab.name, key)] = value.ToString("R", CultureInfo.InvariantCulture);
        }

        public Color? GetColorValue(NetInfo prefab, string key)
        {
            var value = GetValue(prefab, key);
            if (value == null) return null;

            if (ColorUtility.TryParseHtmlString(value, out var color))
            {
                return color;
            }
            else
            {
                Debug.LogError($"Error while parsing {key} color value for {prefab.name}");
                return null;
            }
        }

        public void SetColorValue(NetInfo prefab, string key, Color value)
        {
            _values[new ValueKey(prefab.name, key)] = ColorUtility.ToHtmlStringRGB(value);
        }

        public void ClearValue(NetInfo prefab, string key)
        {
            _values.Remove(new ValueKey(prefab.name, key));
        }

        private class ValueKey
        {
            public readonly string PrefabName;
            public readonly string Key;

            public ValueKey(string prefabName, string key)
            {
                PrefabName = prefabName;
                Key = key;
            }

            private bool Equals(ValueKey other)
            {
                return string.Equals(PrefabName, other.PrefabName) && string.Equals(Key, other.Key);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((ValueKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((PrefabName != null ? PrefabName.GetHashCode() : 0) * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                }
            }
        }

        private class Data : IDataContainer
        {
            public Dictionary<ValueKey, string> Values { get; set; }

            public void Serialize(DataSerializer s)
            {
                s.WriteInt32(Values.Count);
                foreach (var pair in Values)
                {
                    s.WriteUniqueString(pair.Key.PrefabName);
                    s.WriteUniqueString(pair.Key.Key);
                    s.WriteUniqueString(pair.Value);
                }
            }

            public void Deserialize(DataSerializer s)
            {
                Values = new Dictionary<ValueKey, string>();

                var valuesCount = s.ReadInt32();
                for (var i = 0; i < valuesCount; i++)
                {
                    var prefabName = s.ReadUniqueString();
                    var key = s.ReadUniqueString();
                    var value = s.ReadUniqueString();
                    Values[new ValueKey(prefabName, key)] = value;
                }
            }

            public void AfterDeserialize(DataSerializer s) {}
        }
    }
}
