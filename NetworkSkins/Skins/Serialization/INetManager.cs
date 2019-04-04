namespace NetworkSkins.Skins.Serialization
{
    public interface INetManager
    {
        bool IsSegmentCreated(ushort segment);
        NetInfo GetSegmentInfo(ushort segment);

        bool IsNodeCreated(ushort node);
        NetInfo GetNodeInfo(ushort node);
    }
}
