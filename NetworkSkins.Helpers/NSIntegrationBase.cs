namespace NetworkSkins.Helpers {
    using ColossalFramework.UI;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class NSIntegrationBase<TIntegration> : INSIntegration 
        where TIntegration : NSIntegrationBase<TIntegration>, INSIntegration, new()
    {
        public INSAPI api_;
        public IPersistency persistency_;

        public IPersistency Persistency => 
            persistency_ ??= new NSPersistencyWrapper(NSHelpers.GetPersistency());

        public INSAPI API => 
            api_ ??= new NSAPIWrapper(NSHelpers.GetNSAPI());


        #region life cycle
        public static TIntegration Instance;

        private static void Create() {
            Instance ??= new TIntegration();
            Instance.AddImplementation();
        }

        /// <summary>Call when your mod is enabled</summary>
        public static void Install() => NSHelpers.DoOnNSEnabled(Create);

        /// <summary>Call when your mod is disabled</summary>
        public static void Uninstall() {
            NSHelpers.EventNSInstalled -= Create;
            Instance?.API?.RemoveImplementation(Instance);
            Instance = null;
        }

        public virtual void OnNSDisabled() {
            persistency_ = null;
            api_ = null;
            Install(); // install again if/when NS is enabled again.
        }

        public virtual void OnBeforeNSLoaded() {
            persistency_ = null;
        }
        
        public virtual void OnAfterNSLoaded() { }
        #endregion life cycle

        #region API
        public void AddImplementation() => API.AddImplementation(this);
        public bool RemoveImplementation() => API.RemoveImplementation(this);
        public object GetSegmentSkinData(ushort segmentID) => API.GetSegmentSkinData(Index, segmentID);
        public object GetNodeSkinData(ushort nodeID) => API.GetNodeSkinData(Index, nodeID);
        public void OnControllerChanged() => API.OnControllerChanged(ID);
        #endregion


        public abstract string ID { get; }
        public abstract int Index { get; set; }
        public abstract void OnSkinApplied(ICloneable data, InstanceID instanceID);

        public abstract Version DataVersion { get; }
        public abstract string Encode64(ICloneable data);
        public abstract ICloneable Decode64(string base64Data, Version dataVersion);

        public abstract Texture2D Icon { get; }
        public abstract string Tooltip { get; }
        public abstract void BuildPanel(UIPanel panel);
        public abstract void RefreshUI();

        public abstract bool Enabled { get; }
        public abstract Dictionary<NetInfo, ICloneable> BuildCustomData();
        public abstract void Reset();
        public abstract void LoadWithData(ICloneable data);
        public abstract void LoadActiveSelection();
        public abstract void SaveActiveSelection();
    }
}
