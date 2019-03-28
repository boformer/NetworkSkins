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

        // TODO not in use
        private void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            NetworkSkinManager.SegmentSkins[newSegment] = NetworkSkinManager.SegmentSkins[oldSegment];
        }

        private void OnSegmentRelease(ushort segment)
        {
            NetworkSkinManager.SegmentSkins[segment] = null;
        }
    }
}
