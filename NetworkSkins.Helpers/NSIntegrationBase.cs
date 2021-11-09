﻿namespace NetworkSkins.Helpers {
    using ColossalFramework.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public abstract class NSIntegrationBase<T> : INSIntegration where T : class, new() {
        #region life cycle
        public static T Instace { get; private set; }
        private static void Create() => Instace = new T();
        
        /// <summary>
        /// call when your mod is enabled
        /// </summary>
        public static void Install() => NSHelpers.DoOnNSEnabled(Create);
        
        /// <summary>
        /// call when your mod is disabled
        /// </summary>
        public void Uninstall() {
            this.Remove();
            Instace = null;
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
