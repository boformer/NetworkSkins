using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.UI;
using NetworkSkins.Skins.Serialization;
using UnityEngine;

// TODO same structure as old SegmentDataManager for compat with other mods
// TODO add clear applied skins, segment skins and node skins on level unload

namespace NetworkSkins.Skins
{
    public class NetworkSkinManager : Singleton<NetworkSkinManager>
    {
        public const string DataKey = "NetworkSkins_APPLIED_SKINS";

        // stores which data is applied to a network segment
        // this is an array field for high lookup performance
        // can contain null values!
        public static NetworkSkin[] SegmentSkins;
        public static NetworkSkin[] NodeSkins;

        // Skins that are currently in use on the map
        public ReadOnlyCollection<NetworkSkin> AppliedSkins => _appliedSkins.AsReadOnly();
        private List<NetworkSkin> _appliedSkins;

        // Skins that are currently selected in the UI and will be applied to new segments and nodes
        private Dictionary<NetInfo, NetworkSkin> _activeSkins;

        private NetworkSkinLoadErrors _loadErrors;

        #region Lifecycle
        public void Awake()
        {
            SegmentSkins = new NetworkSkin[NetManager.MAX_SEGMENT_COUNT];
            NodeSkins = new NetworkSkin[NetManager.MAX_NODE_COUNT];

            _appliedSkins = new List<NetworkSkin>();
            _activeSkins = new Dictionary<NetInfo, NetworkSkin>();
        }

        public void OnDestroy()
        {
            ClearSkinData();

            NodeSkins = null;
            SegmentSkins = null;
        }
        #endregion

        #region Level Events
        public void OnPreUpdateData(SimulationManager.UpdateMode updateMode)
        {
            // TODO this is not really necessary if the OnLevelUnloading hook works correctly
            ClearSkinData();
            LoadSkinData();
        }

        public void OnLevelLoaded()
        {
            _loadErrors?.MaybeShowErrors();
        }

        public void OnSaveData()
        {
            if (_appliedSkins.Count > 0)
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
            // TODO Destroy skins which are no longer in use

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

                var matchingAppliedSkin = NetworkSkin.GetMatchingSkinFromList(_appliedSkins, prefab, modifiers);
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
            if (_appliedSkins.Count >= ushort.MaxValue - 1)
            {
                return;
            }

            var netManager = NetManager.instance;
            var startNode = netManager.m_segments.m_buffer[segment].m_startNode;
            var endNode = netManager.m_segments.m_buffer[segment].m_endNode;
            var prefab = netManager.m_segments.m_buffer[segment].Info;

            var previousStartSkin = NetworkSkinManager.NodeSkins[startNode];
            var previousEndSkin = NetworkSkinManager.NodeSkins[endNode];

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

            Debug.Log($"OnSegmentCreate {segment}, startNode: {startNode}, endNode {endNode}, prefab: {prefab}, skin: {skin}");
        }

        public void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            var oldSkin = SegmentSkins[oldSegment];

            SegmentSkins[newSegment] = oldSkin;

            UsageAdded(oldSkin);

            Debug.Log($"OnSegmentTransferData {oldSegment} --> {newSegment}, skin: {oldSkin}");
        }

        public void OnSegmentRelease(ushort segment)
        {
            var skin = SegmentSkins[segment];

            SegmentSkins[segment] = null;

            UsageRemoved(skin);

            Debug.Log($"OnSegmentRelease {segment}, skin: {NetworkSkinManager.SegmentSkins[segment]}");
        }

        public void UpdateNodeSkin(ushort node, NetworkSkin skin)
        {
            var previousSkin = NodeSkins[node];
            if (Equals(previousSkin, skin)) return;

            NetworkSkinManager.NodeSkins[node] = skin;

            // Make sure that the color map is updated when a skin with a different color is applied!
            if (previousSkin?.m_color != skin?.m_color)
            {
                NetManager.instance.UpdateNodeColors(node);
            }

            UsageAdded(skin);
            UsageRemoved(previousSkin);
        }

        public void OnNodeRelease(ushort node)
        {
            var skin = NetworkSkinManager.NodeSkins[node];
            NetworkSkinManager.NodeSkins[node] = null;

            UsageRemoved(skin);

            Debug.Log($"OnNodeRelease {node}, skin: {NetworkSkinManager.NodeSkins[node]}");
        }
        #endregion
        
        #region Usage Tracking
        private void UsageAdded(NetworkSkin skin, int count = 1)
        {
            if (skin == null) return;

            if (skin.UseCount == 0)
            {
                Debug.Log($"Adding skin to applied skins: {skin}");
                _appliedSkins.Add(skin);
            }

            skin.UseCount += count;

            Debug.Log($"Usage added: {skin}");
        }

        private void UsageRemoved(NetworkSkin skin)
        {
            if (skin == null) return;

            skin.UseCount--;

            Debug.Log($"Usage removed: {skin}");

            if (skin.UseCount <= 0)
            {
                Debug.Log($"Removing skin from applied skins: {skin}");
                _appliedSkins.Remove(skin);

                if (!IsActive(skin))
                {
                    Debug.Log($"Destroying unused skin {skin}");
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
                    DataSerializer.Serialize(stream, DataSerializer.Mode.Memory, NetworkSkinData.Version, new NetworkSkinData());
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

        private void LoadSkinData()
        {
            Debug.Log("NS: Loading skin data!");

            try
            {
                var data = SimulationManager.instance.m_SerializableDataWrapper.LoadData(DataKey);
                if (data == null)
                {
                    Debug.Log("NS: No data found!");
                    return;
                }

                NetworkSkinData dataContainer;
                using (var stream = new MemoryStream(data))
                {
                    dataContainer = DataSerializer.Deserialize<NetworkSkinData>(stream, DataSerializer.Mode.Memory);
                }

                _loadErrors = dataContainer.Errors;
            }
            catch (Exception e)
            {
                _loadErrors = new NetworkSkinLoadErrors();
                _loadErrors.MajorException(e);
            }
        }
        #endregion

        private void ClearSkinData()
        {
            for (var n = 0; n < NetworkSkinManager.NodeSkins.Length; n++)
            {
                NodeSkins[n] = null;
            }

            for (var s = 0; s < NetworkSkinManager.SegmentSkins.Length; s++)
            {
                SegmentSkins[s] = null;
            }

            foreach (var skin in _appliedSkins)
            {
                skin.Destroy();
            }
            _appliedSkins.Clear();

            foreach (var skin in _activeSkins.Values)
            {
                skin.Destroy();
            }
            _activeSkins.Clear();
        }
    }
}
