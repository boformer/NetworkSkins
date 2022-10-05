namespace NetworkSkins.Helpers {
    using ColossalFramework.UI;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class NSAPIWrapper : INSAPI {
        public object NSAPI { get; private set; }
        private static class Delegates {
            public delegate void AddImplementation(object impl);
            public delegate bool RemoveImplementation(object impl);
            public delegate object GetSegmentSkinData(int implIndex, ushort segmentID);
            public delegate object GetNodeSkinData(int implIndex, ushort nodeID);
            public delegate void OnControllerChanged(string implID);
        }

        private readonly Delegates.AddImplementation addImplementation_;
        private readonly Delegates.RemoveImplementation removeImplementation_;
        private readonly Delegates.GetSegmentSkinData getSegmentSkinData_;
        private readonly Delegates.GetNodeSkinData getNodeSkinData_;
        private readonly Delegates.OnControllerChanged onControllerChanged_;

        public NSAPIWrapper(object nsapi) {
            NSAPI = nsapi;
            addImplementation_ = DelegateUtil.CreateClosedDelegate<Delegates.AddImplementation>(nsapi);
            removeImplementation_ = DelegateUtil.CreateClosedDelegate<Delegates.RemoveImplementation>(nsapi);
            getSegmentSkinData_ = DelegateUtil.CreateClosedDelegate<Delegates.GetSegmentSkinData>(nsapi);
            getNodeSkinData_ = DelegateUtil.CreateClosedDelegate<Delegates.GetNodeSkinData>(nsapi);
            onControllerChanged_ = DelegateUtil.CreateClosedDelegate<Delegates.OnControllerChanged>(nsapi);
        }

        public void AddImplementation(object impl) => addImplementation_(impl);
        public bool RemoveImplementation(object impl) => removeImplementation_(impl);
        public object GetSegmentSkinData(int implIndex, ushort segmentID) => getSegmentSkinData_(implIndex, segmentID);
        public object GetNodeSkinData(int implIndex, ushort nodeID) => getNodeSkinData_(implIndex, nodeID);
        public void OnControllerChanged(string implID) => onControllerChanged_(implID);
    }
}
