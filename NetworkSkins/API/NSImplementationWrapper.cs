namespace NetworkSkins.API {
    using ColossalFramework.UI;
    using System;
    using NetworkSkins.Helpers;
    using System.Collections.Generic;

    public class NSImplementationWrapper : INSImplementation{
        private static class Delegates {
            public delegate int get_Index(object impl);
            public delegate int set_Index(object impl, int value);
            public delegate string get_ID(object impl);
            public delegate void OnBeforeNSLoaded(object impl);
            public delegate void OnAfterNSLoaded(object impl);
            public delegate void OnSkinApplied(object impl, object data, InstanceID instanceID);
            public delegate void OnNSDisabled(object impl);

            public delegate Version get_DataVersion(object impl);
            public delegate string Encode64(object impl, ICloneable data);
            public delegate ICloneable Decode64(object impl, string base64Data, Version dataVersion);

            public delegate UITextureAtlas get_Atlas(object impl);
            public delegate string get_BackGroundSprite(object impl);
            public delegate void BuildPanel(object impl, UIPanel panel);
            public delegate void RefreshUI(object impl, NetInfo netInfo);

            public delegate UITextureAtlas get_Enabled(object impl);
            public delegate Dictionary<NetInfo, ICloneable> BuildCustomData(object impl);
            public delegate void BuildWithData(object impl, ICloneable data);
            public delegate void Reset(object impl);
            public delegate void BuildActiveSelection(object impl);
        }

        public object Implemenation;

        private Delegates.get_Index get_Index_;
        private Delegates.set_Index set_Index_;
        private Delegates.get_ID get_ID_;
        private Delegates.OnBeforeNSLoaded onBeforeNSLoaded_;
        private Delegates.OnAfterNSLoaded onAfterNSLoaded_;
        private Delegates.OnSkinApplied onSkinApplied_;
        private Delegates.OnNSDisabled onNSDisabled_;

        private Delegates.get_DataVersion get_DataVersion_;
        private Delegates.Encode64 encode64_;
        private Delegates.Decode64 decode64_;

        private Delegates.get_Atlas get_Atlas_;
        private Delegates.get_BackGroundSprite get_BackGroundSprite_;
        private Delegates.BuildPanel buildPanel_;
        private Delegates.RefreshUI refreshUI_;

        private Delegates.get_Enabled get_Enabled_;
        private Delegates.BuildCustomData buildCustomData_;
        private Delegates.BuildWithData buildWithData_;
        private Delegates.Reset reset_;
        private Delegates.BuildActiveSelection buildActiveSelection_;

        public NSImplementationWrapper(object impl) {
            Implemenation = impl;

            Type type = impl.GetType();
            get_Index_ = DelegateUtil.CreateDelegate<Delegates.get_Index>(type, true);
            set_Index_ = DelegateUtil.CreateDelegate<Delegates.set_Index>(type, true);
            get_ID_ = DelegateUtil.CreateDelegate<Delegates.get_ID>(type, true);
            onBeforeNSLoaded_ = DelegateUtil.CreateDelegate<Delegates.OnBeforeNSLoaded>(type, true);
            onAfterNSLoaded_ = DelegateUtil.CreateDelegate<Delegates.OnAfterNSLoaded>(type, true);
            onSkinApplied_ = DelegateUtil.CreateDelegate<Delegates.OnSkinApplied>(type, true);
            onNSDisabled_ = DelegateUtil.CreateDelegate<Delegates.OnNSDisabled>(type, true);

            get_DataVersion_ = DelegateUtil.CreateDelegate<Delegates.get_DataVersion>(type, true);
            encode64_ = DelegateUtil.CreateDelegate<Delegates.Encode64>(type, true);
            decode64_ = DelegateUtil.CreateDelegate<Delegates.Decode64>(type, true);

            get_Atlas_ = DelegateUtil.CreateDelegate < Delegates.get_Atlas> (type, true);
            get_BackGroundSprite_ = DelegateUtil.CreateDelegate < Delegates.get_BackGroundSprite> (type, true);
            buildPanel_ = DelegateUtil.CreateDelegate < Delegates.BuildPanel> (type, true);
            refreshUI_ = DelegateUtil.CreateDelegate<Delegates.RefreshUI>(type, true);

            get_Enabled_  = DelegateUtil.CreateDelegate<Delegates.get_Enabled>(type, true);
            buildCustomData_ = DelegateUtil.CreateDelegate < Delegates.BuildCustomData> (type, true);
            buildWithData_ = DelegateUtil.CreateDelegate < Delegates.BuildWithData> (type, true);
            reset_ = DelegateUtil.CreateDelegate < Delegates.Reset> (type, true);
            buildActiveSelection_ = DelegateUtil.CreateDelegate < Delegates.BuildActiveSelection> (type, true);
        }

        public int Index { 
            get => get_Index_(Implemenation);
            set => set_Index_(Implemenation, value);
        }

        public string ID => get_ID_(Implemenation);
        public void OnBeforeNSLoaded() => onBeforeNSLoaded_(Implemenation);
        public void OnAfterNSLoaded() => onAfterNSLoaded_(Implemenation);
        public void OnSkinApplied(object data, InstanceID instanceID) => onSkinApplied_(Implemenation, data, instanceID);
        public void OnNSDisabled() => onNSDisabled_(Implemenation);

        #region Persistency
        public Version DataVersion => get_DataVersion_(Implemenation);
        public string Encode64(ICloneable data) {
            if(data == null)
                return null;
            else
                return encode64_(Implemenation, data);
        }
        public ICloneable Decode64(string base64Data, Version dataVersion) {
            if(base64Data == null)
                return null;
            else
                return decode64_(Implemenation, base64Data, dataVersion);
        }

        #endregion

        #region panel
        public UITextureAtlas Atlas => get_Atlas_(Implemenation);
        public string BackGroundSprite => get_BackGroundSprite_(Implemenation);
        public void BuildPanel(UIPanel panel) => buildPanel_(Implemenation,panel);
        public void RefreshUI(NetInfo netInfo) => refreshUI_(Implemenation, netInfo);
        #endregion

        #region Controller
        public bool Enabled => get_Enabled_(Implemenation);

        public Dictionary<NetInfo, ICloneable> BuildCustomData() => buildCustomData_(Implemenation);
        public void BuildWithData(ICloneable data) => buildWithData_(Implemenation, data);
        public void Reset() => reset_(Implemenation);
        public void BuildActiveSelection() => buildActiveSelection_(Implemenation);
        #endregion
    }
}
