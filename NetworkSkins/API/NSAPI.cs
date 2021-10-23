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

        public NSImplementationWrapper GetImplementationWrapper(string id) =>
            ImplementationWrappers.FirstOrDefault(item => item.ID == id);
        

        public void AddImplementation(object impl) {
            var wrapper = new NSImplementationWrapper(impl);
            ImplementationWrappers.Add(wrapper);
            int index = ImplementationWrappers.Count - 1;
            wrapper.Index = index;
        }

        public bool RemoveImplementation(object impl) {
            var wrapper = ImplementationWrappers.FirstOrDefault(item => item.Implemenation == impl);
            if(wrapper != null) {
                ImplementationWrappers[wrapper.Index] = null;
                return true;
            } else {
                return false;
            }
        }

        public object GetSegmentSkinData(string implID, ushort segmentID) =>
            NetworkSkinManager.SegmentSkins[segmentID].m_CustomDatas?[implID];
        

        public object GetSegmentSkinData(int implIndex, ushort segmentID) =>
            NetworkSkinManager.SegmentSkins[segmentID].m_CustomDatas[implIndex];
        
    }
}
