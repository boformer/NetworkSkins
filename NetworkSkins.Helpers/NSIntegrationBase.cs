﻿namespace NetworkSkins.Helpers {
    using ColossalFramework.Plugins;
    using ColossalFramework.UI;
    using ICities;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class NSIntegrationBase<TIntegration> : INSIntegration 
        where TIntegration : class, INSIntegration, new()
    {
        #region life cycle
        public static TIntegration Instance { get; private set; }

        private static void Create() {
            Instance ??= new TIntegration();
            Instance.Register();
        }

        /// <summary>Call when your mod is enabled</summary>
        public static void Install() => NSHelpers.DoOnNSEnabled(Create);

        /// <summary>Call when your mod is disabled</summary>
        public static void Uninstall() {
            NSHelpers.EventNSInstalled -= Create;
            Instance?.Remove();
            Instance = null;
        }

        public virtual void OnNSDisabled() => Install(); // install again if/when NS is enabled again.

        public virtual void OnBeforeNSLoaded() { }
        
        public virtual void OnAfterNSLoaded() { }
        #endregion life cycle

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