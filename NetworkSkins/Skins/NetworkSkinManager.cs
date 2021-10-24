using System;
using System.Collections.Generic;
using System.IO;
using ColossalFramework.IO;
using ColossalFramework.UI;
using NetworkSkins.API;
using NetworkSkins.Legacy;
using NetworkSkins.Net;
using NetworkSkins.Persistence;
using NetworkSkins.Skins.Modifiers;
using NetworkSkins.Skins.Serialization;
using UnityEngine;

namespace NetworkSkins.Skins
{
    public class NetworkSkinManager : MonoBehaviour
    {
        public const string DataKey = "NetworkSkins_APPLIED_SKINS";
        public const string LegacyDataKey = "NetworkSkins_SEGMENTS";

        private static NetworkSkinManager _instance;
        public static NetworkSkinManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<NetworkSkinManager>();
                    if (_instance == null)
                    {
                        var gameObject = new GameObject(nameof(NetworkSkinManager));
                        _instance = gameObject.AddComponent<NetworkSkinManager>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        public static NetworkSkinManager Ensure() => instance;

        public static void Uninstall()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }
        }

        // stores which data is applied to a network segment
        // this is an array field for high lookup performance
        // can contain null values!
        public static NetworkSkin[] SegmentSkins;
        public static NetworkSkin[] NodeSkins;

        // Skins that are currently in use on the map
        public readonly List<NetworkSkin> AppliedSkins = new List<NetworkSkin>();

        // Skins that are currently selected in the UI and will be applied to new segments and nodes
        private readonly Dictionary<NetInfo, NetworkSkin> _activeSkins = new Dictionary<NetInfo, NetworkSkin>();

        private LegacyLightPropEnabler _lightPropEnabler = new LegacyLightPropEnabler();

        private NetworkSkinLoadErrors _loadErrors;

        public delegate void SegmentPlacedEventHandler(NetworkSkin skin);
        public event SegmentPlacedEventHandler EventSegmentPlaced;

        #region Lifecycle
        public void Awake()
        {
            SegmentSkins = new NetworkSkin[NetManager.MAX_SEGMENT_COUNT];
            NodeSkins = new NetworkSkin[NetManager.MAX_NODE_COUNT];
        }

        public void OnDestroy()
        {
            ClearSkinData();

            NodeSkins = null;
            SegmentSkins = null;

            AppliedSkins.Clear();
            _activeSkins.Clear();
        }
        #endregion

        #region Level Events
        public void OnPreUpdateData(SimulationManager.UpdateMode mode)
        {
            _lightPropEnabler.OnPreUpdateData();

            ClearSkinData();

            var dataFound = LoadSkinData();

            if (!dataFound)
            {
                LoadLegacySkinData();
            }
        }

        public void OnLevelLoaded()
        {
            _lightPropEnabler.OnLevelLoaded();
            _loadErrors?.MaybeShowErrors();
        }

        public void OnPostLevelLoaded()
        {
            // Fix for various mods which modify lane props in OnLevelLoaded, 
            // long after we are doing it in OnPreUpdateData
            // New American Traffic Lights - NYC/NJ Style
            // New American Traffic Lights - Grey Style
            // New American Traffic Lights - Vanilla Side
            // New American Traffic Lights
            // American Sign Replacer
            // American Railroad Signal Replacer
            // American Traffic Lights
            // American RoadSigns v2.2.0
            // ...
            for (int i = 0; i < AppliedSkins.Count; i++)
            {
                AppliedSkins[i].Recalculate();
            }
        }

        public void OnSaveData()
        {
            if (AppliedSkins.Count > 0)
            {
                SaveSkinData();
            }
            else
            {
                EraseSkinData();
            }
        }

        public void OnLevelUnloading()
        {
            _loadErrors = null;
            ClearSkinData();
        }
        #endregion

        #region Active Skins
        public void SetActiveModifiers(Dictionary<NetInfo, List<NetworkSkinModifier>> prefabsWithModifiers)
        {
            var possiblyUnusedSkins = new List<NetworkSkin>(_activeSkins.Values);

            _activeSkins.Clear();

            foreach (var pair in prefabsWithModifiers)
            {
                var prefab = pair.Key;
                var modifiers = pair.Value;

                if (modifiers.Count == 0)
                {
                    continue;
                }

                var matchingActiveSkin = NetworkSkin.GetMatchingSkinFromList(possiblyUnusedSkins, prefab, modifiers);
                if (matchingActiveSkin != null)
                {
                    // exact same skin was already generated and is ready to use,
                    // use that one instead of generating a new skin!
                    _activeSkins[prefab] = matchingActiveSkin;
                    possiblyUnusedSkins.Remove(matchingActiveSkin);
                    continue;
                }

                var matchingAppliedSkin = NetworkSkin.GetMatchingSkinFromList(AppliedSkins, prefab, modifiers);
                if (matchingAppliedSkin != null)
                {
                    // exact same skin is already present in the city,
                    // use that one instead of generating a new skin!
                    _activeSkins[prefab] = matchingAppliedSkin;
                    continue;
                }

                // generate and use a new skin!
                _activeSkins[prefab] = new NetworkSkin(prefab, modifiers);
            }

            // Destroy all unused skins that are not present in the city
            foreach (var skin in possiblyUnusedSkins)
            {
                if (skin.UseCount <= 0)
                {
                    Debug.Log($"Destroying unused skin {skin}");
                    skin.Destroy();
                }
            }
        }

        public void ClearActiveModifiers()
        {
            SetActiveModifiers(new Dictionary<NetInfo, List<NetworkSkinModifier>>());
        }

        public NetworkSkin GetActiveSkin(NetInfo prefab)
        {
            return _activeSkins.TryGetValue(prefab, out var skin) ? skin : null;
        }
        #endregion

        #region Segment/Node Events
        public void OnSegmentPlaced(ushort segment)
        {
            // The maximum number of applied skins is 65535!
            if (AppliedSkins.Count >= ushort.MaxValue - 1)
            {
                return;
            }

            var netManager = NetManager.instance;
            var startNode = netManager.m_segments.m_buffer[segment].m_startNode;
            var endNode = netManager.m_segments.m_buffer[segment].m_endNode;
            var prefab = netManager.m_segments.m_buffer[segment].Info;

            var previousStartSkin = NodeSkins[startNode];
            var previousEndSkin = NodeSkins[endNode];

            _activeSkins.TryGetValue(prefab, out var skin);
            SegmentSkins[segment] = skin;
            NodeSkins[startNode] = skin;
            NodeSkins[endNode] = skin;

            // Make sure that the color map is updated when a skin with a different color is applied!
            if (previousStartSkin?.m_color != skin?.m_color || previousEndSkin?.m_color != skin?.m_color)
            {
                netManager.UpdateNodeColors(startNode);
                netManager.UpdateNodeColors(endNode);
            }

            UsageAdded(skin, count: 3);
            UsageRemoved(previousStartSkin);
            UsageRemoved(previousEndSkin);

            if (skin != null)
            {
                EventSegmentPlaced?.Invoke(skin);
            }

            NSAPI.Instance.OnSkinApplied(skin?.m_CustomDatas, new InstanceID { NetSegment = segment });
        }

        public void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            var oldSkin = SegmentSkins[oldSegment];

            SegmentSkins[newSegment] = oldSkin;

            UsageAdded(oldSkin);

            NSAPI.Instance.OnSkinApplied(oldSkin.m_CustomDatas, new InstanceID { NetSegment = newSegment });
        }

        public void OnSegmentRelease(ushort segment)
        {
            var skin = SegmentSkins[segment];

            SegmentSkins[segment] = null;

            UsageRemoved(skin);
        }

        public void UpdateNodeSkin(ushort node, NetworkSkin skin)
        {
            var previousSkin = NodeSkins[node];
            if (Equals(previousSkin, skin)) return;

            NodeSkins[node] = skin;

            if (previousSkin?.m_color != skin?.m_color)
            {
                NetManager.instance.UpdateNodeColors(node);
            }

            UsageAdded(skin);
            UsageRemoved(previousSkin);
        }

        public void OnNodeRelease(ushort node)
        {
            var skin = NodeSkins[node];
            NodeSkins[node] = null;

            UsageRemoved(skin);
        }
        #endregion
        
        #region Usage Tracking
        private void UsageAdded(NetworkSkin skin, int count = 1)
        {
            if (skin == null) return;

            if (skin.UseCount == 0)
            {
                AppliedSkins.Add(skin);
            }

            skin.UseCount += count;
        }

        private void UsageRemoved(NetworkSkin skin)
        {
            if (skin == null) return;

            skin.UseCount--;

            if (skin.UseCount <= 0)
            {
                AppliedSkins.Remove(skin);

                if (!IsActive(skin))
                {
                    skin.Destroy();
                }
            }
        }

        private bool IsActive(NetworkSkin skin)
        {
            return _activeSkins.TryGetValue(skin.Prefab, out var activeSkin) && Equals(activeSkin, skin);
        }
        #endregion

        #region Serialization
        private void SaveSkinData()
        {
            Debug.Log("NS: Saving skin data!");

            try
            {
                byte[] data;
                using (var stream = new MemoryStream())
                {
                    DataSerializer.Serialize(stream, DataSerializer.Mode.Memory, NetworkSkinDataContainer.Version, new NetworkSkinDataContainer());
                    data = stream.ToArray();
                }

                SimulationManager.instance.m_SerializableDataWrapper.SaveData(DataKey, data);
            }
            catch (Exception e)
            {
                Debug.LogError("NS: Failed to save skin data!");
                Debug.LogException(e);

                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    "Network Skins",
                    "Error while saving skin data! If you leave the game now, all network skins will be lost forever!\n\n" +
                    $"{e}",
                    true
                );
            }
        }

        private void EraseSkinData()
        {
            Debug.Log("NS: Erasing skin data!");

            SimulationManager.instance.m_SerializableDataWrapper.EraseData(DataKey);
        }

        private bool LoadSkinData()
        {
            Debug.Log("NS: Loading skin data!");

            try
            {
                var data = SimulationManager.instance.m_SerializableDataWrapper.LoadData(DataKey);
                if (data == null)
                {
                    Debug.Log("NS: No data found!");
                    return false;
                }

                NetworkSkinDataContainer dataContainer;
                using (var stream = new MemoryStream(data))
                {
                    dataContainer = DataSerializer.Deserialize<NetworkSkinDataContainer>(stream, DataSerializer.Mode.Memory, NetworkSkinsMod.ResolveSerializedType);
                }

                _loadErrors = dataContainer.Errors;
            }
            catch (Exception e)
            {
                _loadErrors = new NetworkSkinLoadErrors();
                _loadErrors.MajorException(e);
            }

            return true;
        }

        private void LoadLegacySkinData()
        {
            Debug.Log("NS: Loading legacy skin data!");

            try
            {
                var data = SimulationManager.instance.m_SerializableDataWrapper.LoadData(LegacyDataKey);
                if (data == null)
                {
                    Debug.Log("NS: No legacy data found!");
                    return;
                }

                LegacySegmentData[] legacyData;
                using (var stream = new MemoryStream(data))
                {
                    legacyData = DataSerializer.DeserializeArray<LegacySegmentData>(stream, DataSerializer.Mode.Memory, LegacySegmentData.ResolveSerializedType);
                }

                var netManager = new GameNetManager();

                var length = Math.Min(legacyData.Length, SegmentSkins.Length);
                for (ushort segment = 0; segment < length; segment++)
                {
                    var segmentData = legacyData[segment];
                    if (segmentData != null && netManager.IsSegmentCreated(segment))
                    {
                        segmentData.FindPrefabs();

                        var prefab = netManager.GetSegmentInfo(segment);

                        var modifiers = new List<NetworkSkinModifier>();

                        var customRepeatDistances =
                            (segmentData.Features & LegacySegmentData.FeatureFlags.RepeatDistances) != 0;

                        if ((segmentData.Features & LegacySegmentData.FeatureFlags.StreetLight) != 0)
                        {
                            var repeatDistance = customRepeatDistances
                                ? segmentData.RepeatDistances.w
                                : StreetLightUtils.GetDefaultRepeatDistance(prefab);

                            modifiers.Add(new StreetLightModifier(segmentData.StreetLightPrefab, repeatDistance));
                        }

                        if ((segmentData.Features & LegacySegmentData.FeatureFlags.TreeLeft) != 0)
                        {
                            var repeatDistance = customRepeatDistances
                                ? segmentData.RepeatDistances.x
                                : TreeUtils.GetDefaultRepeatDistance(prefab, LanePosition.Left);

                            modifiers.Add(new TreeModifier(LanePosition.Left, segmentData.TreeLeftPrefab, repeatDistance));
                        }

                        if ((segmentData.Features & LegacySegmentData.FeatureFlags.TreeMiddle) != 0)
                        {
                            var repeatDistance = customRepeatDistances
                                ? segmentData.RepeatDistances.y
                                : TreeUtils.GetDefaultRepeatDistance(prefab, LanePosition.Middle);

                            modifiers.Add(new TreeModifier(LanePosition.Middle, segmentData.TreeMiddlePrefab, repeatDistance));
                        }

                        if ((segmentData.Features & LegacySegmentData.FeatureFlags.TreeRight) != 0)
                        {
                            var repeatDistance = customRepeatDistances
                                ? segmentData.RepeatDistances.z
                                : TreeUtils.GetDefaultRepeatDistance(prefab, LanePosition.Right);

                            modifiers.Add(new TreeModifier(LanePosition.Right, segmentData.TreeRightPrefab, repeatDistance));
                        }

                        if (modifiers.Count > 0)
                        {
                            var skin = NetworkSkin.GetMatchingSkinFromList(AppliedSkins, prefab, modifiers);
                            if(skin == null)
                            {
                                skin = new NetworkSkin(prefab, modifiers);
                            }

                            SegmentSkins[segment] = skin;

                            UsageAdded(skin);
                        }
                    }
                }

                

                SimulationManager.instance.m_SerializableDataWrapper.EraseData(LegacyDataKey);
                Debug.Log("NS: Legacy data imported and erased");
            }
            catch (Exception e)
            {
                _loadErrors = new NetworkSkinLoadErrors();
                _loadErrors.MajorException(e);
            }
        }
        #endregion

        public List<NetworkSkinModifier> GetModifiersForSegment(ushort segmentId)
        {
            var segmentSkin = SegmentSkins[segmentId];
            return segmentSkin != null ? new List<NetworkSkinModifier>(segmentSkin.Modifiers) : new List<NetworkSkinModifier>();
        }

        private void ClearSkinData()
        {
            for (var n = 0; n < NodeSkins.Length; n++)
            {
                NodeSkins[n] = null;
            }

            for (var s = 0; s < SegmentSkins.Length; s++)
            {
                SegmentSkins[s] = null;
            }

            foreach (var skin in AppliedSkins)
            {
                skin.Destroy();
            }
            AppliedSkins.Clear();

            foreach (var skin in _activeSkins.Values)
            {
                skin.Destroy();
            }
            _activeSkins.Clear();
        }
    }
}
