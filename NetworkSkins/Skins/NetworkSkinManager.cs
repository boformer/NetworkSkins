using System.Collections.Generic;
using ColossalFramework;

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
            NetworkSkinManager.NodeSkins[node] = skin;
            // TODO update use count
        }


        public void OnSegmentCreate(ushort segment)
        {
            var prefab = NetManager.instance.m_segments.m_buffer[segment].Info;

            if (_activeSkins.TryGetValue(prefab, out var skin))
            {
                NetworkSkinManager.SegmentSkins[segment] = skin;
                // TODO update use count
            }
        }

        public void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            NetworkSkinManager.SegmentSkins[newSegment] = NetworkSkinManager.SegmentSkins[oldSegment];
            // TODO update use count
        }

        public void OnSegmentRelease(ushort segment)
        {
            NetworkSkinManager.SegmentSkins[segment] = null;
            // TODO update use count
        }

        public void OnNodeRelease(ushort node)
        {
            NetworkSkinManager.NodeSkins[node] = null;
            // TODO update use count
        }
    }
}
