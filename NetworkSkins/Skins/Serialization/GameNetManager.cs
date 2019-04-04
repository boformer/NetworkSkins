using ColossalFramework;

namespace NetworkSkins.Skins.Serialization
{
    public class GameNetManager : INetManager
    {
        public bool IsSegmentCreated(ushort segment)
        {
            return NetManager.instance.m_segments.m_buffer[segment].m_flags.IsFlagSet(NetSegment.Flags.Created);
        }

        public NetInfo GetSegmentInfo(ushort segment)
        {
            return NetManager.instance.m_segments.m_buffer[segment].Info;
        }

        public bool IsNodeCreated(ushort node)
        {
            return NetManager.instance.m_nodes.m_buffer[node].m_flags.IsFlagSet(NetNode.Flags.Created);
        }

        public NetInfo GetNodeInfo(ushort node)
        {
            return NetManager.instance.m_nodes.m_buffer[node].Info;
        }
    }
}
