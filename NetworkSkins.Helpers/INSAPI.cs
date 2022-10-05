namespace NetworkSkins.Helpers {
    public interface INSAPI {
        void AddImplementation(object impl);
        bool RemoveImplementation(object impl);
        object GetSegmentSkinData(int implIndex, ushort segmentID);
        object GetNodeSkinData(int implIndex, ushort nodeID);
        void OnControllerChanged(string implID);
    }
}
