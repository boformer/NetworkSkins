using System.Collections.Generic;
using System.Runtime.InteropServices;
using ColossalFramework;
using UnityEngine;

namespace NetworkSkins.Skins
{
    // TODO same structure as old SegmentDataManager for compat with other mods
    public class NetworkSkinManager : Singleton<NetworkSkinManager>
    {
        // stores which data is applied to a network segment
        // this is an array field for high lookup performance
        // can contain null values!
        public static NetworkSkin[] SegmentSkins = new NetworkSkin[NetManager.MAX_SEGMENT_COUNT];
        public static NetworkSkin[] NodeSkins = new NetworkSkin[NetManager.MAX_NODE_COUNT];

        private readonly Dictionary<NetInfo, NetworkSkin> _activeSkins = new Dictionary<NetInfo, NetworkSkin>();

        #region Lifecycle
        public void Awake()
        {
            SegmentSkins = new NetworkSkin[NetManager.MAX_SEGMENT_COUNT];
            NodeSkins = new NetworkSkin[NetManager.MAX_NODE_COUNT];
        }

        public void OnDestroy()
        {
            for (var n = 0; n < NetworkSkinManager.NodeSkins.Length; n++)
            {
                NetworkSkinManager.NodeSkins[n]?.Destroy();
                NetworkSkinManager.NodeSkins[n] = null;
            }

            for (var s = 0; s < NetworkSkinManager.SegmentSkins.Length; s++)
            {
                NetworkSkinManager.SegmentSkins[s]?.Destroy();
                NetworkSkinManager.SegmentSkins[s] = null;
            }
        }
        #endregion

        public NetworkSkin GetActiveSkin(NetInfo prefab)
        {
            return _activeSkins.TryGetValue(prefab, out var skin) ? skin : null;
        }

        public void SetActiveSkins(List<NetworkSkin> skins)
        {
            // TODO Destroy skins which are no longer in use
            _activeSkins.Clear();

            // TODO make sure that no duplicate skins exist (with the exact same properties)
            foreach (var skin in skins)
            {
                _activeSkins[skin.Prefab] = skin;
            }
        }

        public void ClearActiveSkins()
        {
            // TODO Destroy skins which are no longer in use
            _activeSkins.Clear();
        }

        public void UpdateNodeSkin(ushort node, NetworkSkin skin)
        {
            var previousSkin = NodeSkins[node];
            // TODO update use count
            NetworkSkinManager.NodeSkins[node] = skin;
            // TODO update use count

            // Make sure that the color map is updated when a skin with a different color is applied!
            if (previousSkin?.m_color != skin?.m_color)
            {
                NetManager.instance.UpdateNodeColors(node);
            }
        }


        public void OnSegmentCreate(ushort segment)
        {
            var netManager = NetManager.instance;
            var startNode = netManager.m_segments.m_buffer[segment].m_startNode;
            var endNode = netManager.m_segments.m_buffer[segment].m_endNode;
            var prefab = netManager.m_segments.m_buffer[segment].Info;

            var previousStartSkin = NetworkSkinManager.NodeSkins[startNode];
            var previousEndSkin = NetworkSkinManager.NodeSkins[endNode];

            _activeSkins.TryGetValue(prefab, out var skin);
            NetworkSkinManager.SegmentSkins[segment] = skin;
            
            NetworkSkinManager.NodeSkins[startNode] = skin;
            NetworkSkinManager.NodeSkins[endNode] = skin;
            // TODO update use count

            // Make sure that the color map is updated when a skin with a different color is applied!
            if (previousStartSkin?.m_color != skin?.m_color || previousEndSkin?.m_color != skin?.m_color)
            {
                netManager.UpdateNodeColors(startNode);
                netManager.UpdateNodeColors(endNode);
            }

            Debug.Log($"OnSegmentCreate {segment}, startNode: {startNode}, endNode {endNode}, prefab: {prefab}, skin: {skin}");
        }

        public void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            Debug.Log($"OnSegmentTransferData {oldSegment} --> {newSegment}, skin: {NetworkSkinManager.SegmentSkins[oldSegment]}");

            NetworkSkinManager.SegmentSkins[newSegment] = NetworkSkinManager.SegmentSkins[oldSegment];
            // TODO update use count
        }

        public void OnSegmentRelease(ushort segment)
        {
            Debug.Log($"OnSegmentRelease {segment}, skin: {NetworkSkinManager.SegmentSkins[segment]}");
            NetworkSkinManager.SegmentSkins[segment] = null;
            // TODO update use count
        }

        public void OnNodeCreate(ushort node)
        {
            var prefab = NetManager.instance.m_nodes.m_buffer[node].Info;

            _activeSkins.TryGetValue(prefab, out var skin);
            NetworkSkinManager.NodeSkins[node] = skin;
            // TODO update use count

            Debug.Log($"OnNodeCreate {node}, prefab: {prefab}, skin: {skin}");
        }

        public void OnNodeTransferData(ushort oldNode, ushort newNode)
        {
            Debug.Log($"OnNodeTransferData {oldNode} --> {newNode}, skin: {NetworkSkinManager.NodeSkins[oldNode]}");

            NetworkSkinManager.NodeSkins[newNode] = NetworkSkinManager.NodeSkins[oldNode];
            // TODO update use count
        }

        public void OnNodeRelease(ushort node)
        {
            Debug.Log($"OnNodeRelease {node}, skin: {NetworkSkinManager.NodeSkins[node]}");

            NetworkSkinManager.NodeSkins[node] = null;
            // TODO update use count
        }
    }
}
