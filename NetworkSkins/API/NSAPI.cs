namespace NetworkSkins.API {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetworkSkins.Skins;

    public class NSAPI {
        public static NSAPI Instance;

        public List<NSImplementationWrapper> ImplementationWrappers = new List<NSImplementationWrapper>();

        public int GetImplementationIndex(string implID) => 
            ImplementationWrappers.FindIndex(item => item.ID == implID);

        public void AddImplementation(object impl) {
            ImplementationWrappers.Add(new NSImplementationWrapper(impl));
        }
        public bool RemoveImplementation(object impl) {
            var item = ImplementationWrappers.FirstOrDefault(item => item.Implemenation == impl);
            return ImplementationWrappers.Remove(item);
        }
        public NSImplementationWrapper GetImplementationWrapper(string id) {
            return Instance.ImplementationWrappers.FirstOrDefault(item => item.ID == id);
        }

        public object GetSegmentSkinData(string implID, ushort segmentID) {
            return NetworkSkinManager.SegmentSkins[segmentID].m_CustomDatas?[implID];
        }

        public object GetSegmentSkinData(int implIndex, ushort segmentID) {
            string implID = ImplementationWrappers[implIndex].ID;
            return NetworkSkinManager.SegmentSkins[segmentID].m_CustomDatas?[implID];
        }
    }
}
