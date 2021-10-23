namespace NetworkSkins.API {
    using NetworkSkins.Skins;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NSAPI {
        public static NSAPI Instance;

        public List<NSImplementationWrapper> ImplementationWrappers = new List<NSImplementationWrapper>();

        public static void Enable() {
            Instance = new NSAPI();
            LoadingManager.instance.m_levelPreLoaded += Instance.OnLevelPreloaded;
        }

        public void Disable() {
            foreach(var impl in ImplementationWrappers) {
                impl.OnNSDisabled();
            }
            LoadingManager.instance.m_levelPreLoaded -= Instance.OnLevelPreloaded;
            Instance = null;
        }

        internal void OnLevelPreloaded() {
            foreach(var impl in ImplementationWrappers) {
                impl.OnBeforeNSLoaded();
            }
        }

        internal void OnSkinApplied(CustomDataCollection skinCustomData, InstanceID instanceID) {
            foreach(var impl in ImplementationWrappers) {
                var data = skinCustomData[impl.Index];
                impl.OnSkinApplied(data, instanceID);
            }
        }

        public int GetImplementationIndex(string implID) =>
            ImplementationWrappers.FindIndex(item => item.ID == implID);

        public NSImplementationWrapper GetImplementationWrapper(string id) =>
            ImplementationWrappers.FirstOrDefault(item => item.ID == id);


        public void AddImplementation(object impl) {
            if(!NetworkSkinsMod.InStartupMenu) {
                throw new Exception("Implementations should be registered before loading game");
            }
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
