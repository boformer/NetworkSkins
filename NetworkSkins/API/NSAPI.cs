namespace NetworkSkins.API {
    using ColossalFramework.UI;
    using NetworkSkins.Skins;
    using NetworkSkins.GUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class NSAPI {
        public static NSAPI Instance;

        public List<NSImplementationWrapper> ImplementationWrappers = new List<NSImplementationWrapper>();
        public IEnumerable<NSImplementationWrapper> ActiveImplementationWrappers =>
            ImplementationWrappers.OfType<NSImplementationWrapper>(); // get rid of nulls

        public static void Enable() {
            Instance = new NSAPI();
            LoadingManager.instance.m_levelPreLoaded += Instance.OnLevelPreloaded;
        }

        public void Disable() {
            foreach(var impl in ActiveImplementationWrappers) {
                impl.OnNSDisabled();
            }
            LoadingManager.instance.m_levelPreLoaded -= Instance.OnLevelPreloaded;
            Instance = null;
        }

        internal void OnLevelPreloaded() {
            foreach(var impl in ActiveImplementationWrappers) {
                impl.OnBeforeNSLoaded();
            }
        }

        internal void OnSkinApplied(CustomDataCollection skinCustomData, InstanceID instanceID) {
            foreach(var impl in ActiveImplementationWrappers) {
                var data = skinCustomData?[impl.Index];
                impl.OnSkinApplied(data, instanceID);
            }
        }

        public int GetImplementationIndex(string implID) =>
            ImplementationWrappers.FindIndex(item => item?.ID == implID);

        public NSImplementationWrapper GetImplementationWrapper(string id) =>
            ActiveImplementationWrappers.FirstOrDefault(item => item.ID == id);


        public void AddImplementation(object impl) {
            var wrapper = new NSImplementationWrapper(impl);
            if(wrapper.ID == null || wrapper.ID == "" || ActiveImplementationWrappers.Any(wrapper2 => wrapper2.ID == wrapper.ID)) {
                var ex = new Exception("Implementation ID must be a unique string. got " + wrapper.ID);
                Debug.LogException(ex);
                UIView.ForwardException(ex);
                throw ex;
            }

            ImplementationWrappers.Add(wrapper);
            int index = ImplementationWrappers.Count - 1;
            wrapper.Index = index;
        }

        public bool RemoveImplementation(object impl) {
            var wrapper = ActiveImplementationWrappers.FirstOrDefault(item => item.Implemenation == impl);
            if(wrapper != null) {
                ImplementationWrappers[wrapper.Index] = null;
                return true;
            } else {
                return false;
            }
        }

        public object GetSegmentSkinData(int implIndex, ushort segmentID) =>
            NetworkSkinManager.SegmentSkins[segmentID].m_CustomDatas[implIndex];

        public object GetNodeSkinData(int implIndex, ushort nodeID) =>
            NetworkSkinManager.NodeSkins[nodeID].m_CustomDatas[implIndex];

        public void OnControllerChanged(string implID) {
            var panel = NetworkSkinPanelController.Instance.CustomPanelControllers[implID];
            panel.Changed();
        }
    }
}
