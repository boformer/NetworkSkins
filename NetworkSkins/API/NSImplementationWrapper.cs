namespace NetworkSkins.API {
    using ColossalFramework.UI;
    using System;
    using NetworkSkins.Helpers;
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Resources = NetworkSkins.Resources;

    public class NSImplementationWrapper : INSImplementation {
        static void LogCalled() => Debug.Log("[NS LogCalled]" + Environment.StackTrace);

        private static class Delegates {
            public delegate int get_Index();
            public delegate void set_Index(int value);
            public delegate string get_ID();
            public delegate void OnBeforeNSLoaded();
            public delegate void OnAfterNSLoaded();
            public delegate void OnSkinApplied(ICloneable data, InstanceID instanceID);
            public delegate void OnNSDisabled();

            public delegate Version get_DataVersion();
            public delegate string Encode64(ICloneable data);
            public delegate ICloneable Decode64(string base64Data, Version dataVersion);

            public delegate Texture2D get_Icon();
            public delegate string get_Tooltip();

            public delegate void BuildPanel(UIPanel panel);
            public delegate void RefreshUI();

            public delegate bool get_Enabled();
            public delegate Dictionary<NetInfo, ICloneable> BuildCustomData();
            public delegate void LoadWithData(ICloneable data);
            public delegate void Reset();
            public delegate void LoadActiveSelection();
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

        private Delegates.get_Icon get_Icon_;
        private Delegates.get_Tooltip get_Tooltip_;
        private Delegates.BuildPanel buildPanel_;
        private Delegates.RefreshUI refreshUI_;

        private Delegates.get_Enabled get_Enabled_;
        private Delegates.BuildCustomData buildCustomData_;
        private Delegates.LoadWithData loadWithData_;
        private Delegates.Reset reset_;
        private Delegates.LoadActiveSelection loadActiveSelection_;


        public NSImplementationWrapper(object impl) {
            LogCalled();
            Implemenation = impl;

            get_Index_ = DelegateUtil.CreateClosedDelegate<Delegates.get_Index>(impl);
            set_Index_ = DelegateUtil.CreateClosedDelegate<Delegates.set_Index>(impl);
            get_ID_ = DelegateUtil.CreateClosedDelegate<Delegates.get_ID>(impl);
            onBeforeNSLoaded_ = DelegateUtil.CreateClosedDelegate<Delegates.OnBeforeNSLoaded>(impl);
            onAfterNSLoaded_ = DelegateUtil.CreateClosedDelegate<Delegates.OnAfterNSLoaded>(impl);
            onSkinApplied_ = DelegateUtil.CreateClosedDelegate<Delegates.OnSkinApplied>(impl);
            onNSDisabled_ = DelegateUtil.CreateClosedDelegate<Delegates.OnNSDisabled>(impl);

            get_DataVersion_ = DelegateUtil.CreateClosedDelegate<Delegates.get_DataVersion>(impl);
            encode64_ = DelegateUtil.CreateClosedDelegate<Delegates.Encode64>(impl);
            decode64_ = DelegateUtil.CreateClosedDelegate<Delegates.Decode64>(impl);

            get_Icon_ = DelegateUtil.CreateClosedDelegate<Delegates.get_Icon>(impl);
            get_Tooltip_ = DelegateUtil.CreateClosedDelegate<Delegates.get_Tooltip>(impl);
            buildPanel_ = DelegateUtil.CreateClosedDelegate<Delegates.BuildPanel>(impl);
            refreshUI_ = DelegateUtil.CreateClosedDelegate<Delegates.RefreshUI>(impl);

            get_Enabled_ = DelegateUtil.CreateClosedDelegate<Delegates.get_Enabled>(impl);
            buildCustomData_ = DelegateUtil.CreateClosedDelegate<Delegates.BuildCustomData>(impl);
            loadWithData_ = DelegateUtil.CreateClosedDelegate<Delegates.LoadWithData>(impl);
            reset_ = DelegateUtil.CreateClosedDelegate<Delegates.Reset>(impl);
            loadActiveSelection_ = DelegateUtil.CreateClosedDelegate<Delegates.LoadActiveSelection>(impl);
        }

        public int Index {
            get => get_Index_();
            set => set_Index_(value);
        }

        public string ID {
            get {
                LogCalled();
                return get_ID_();
            }
        }
        public void OnBeforeNSLoaded() => onBeforeNSLoaded_();
        public void OnAfterNSLoaded() => onAfterNSLoaded_();
        public void OnSkinApplied(ICloneable data, InstanceID instanceID) => onSkinApplied_(data, instanceID);
        public void OnNSDisabled() => onNSDisabled_();

        #region Persistency
        public Version DataVersion => get_DataVersion_();
        public string Encode64(ICloneable data) {
            if(data == null)
                return null;
            else
                return encode64_(data);
        }
        public ICloneable Decode64(string base64Data, Version dataVersion) {
            if(base64Data == null)
                return null;
            else
                return decode64_(base64Data, dataVersion);
        }

        #endregion

        #region panel
        public Texture2D Icon => get_Icon_();

        public const string ForegroundIconName = "Icon";

        private UITextureAtlas CreateAtlas() {
            LogCalled();
            try {
                var normal = Resources.GetTextureFromAssemblyManifest(Resources.ButtonSmall);
                var hovered = Resources.GetTextureFromAssemblyManifest(Resources.ButtonSmallHovered);
                var pressed = Resources.GetTextureFromAssemblyManifest(Resources.ButtonSmallPressed);
                var focused = Resources.GetTextureFromAssemblyManifest(Resources.ButtonSmallFocused);
                var spriteNames = new[] { Resources.ButtonSmall, Resources.ButtonSmallHovered, Resources.ButtonSmallPressed, Resources.ButtonSmallFocused, ForegroundIconName };
                var textures = new[] { normal, hovered, pressed, focused, Icon };

                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                Rect[] regions = texture2D.PackTextures(textures, padding: 2, maximumAtlasSize: 1024);

                Material material = Object.Instantiate(UIView.GetAView().defaultAtlas.material);
                material.mainTexture = texture2D;
                UITextureAtlas textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
                textureAtlas.material = material;
                textureAtlas.name = "NSImplementation_" + ID;

                for(int i = 0; i < spriteNames.Length; i++) {
                    UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo {
                        name = spriteNames[i],
                        texture = textures[i],
                        region = regions[i],
                    };

                    textureAtlas.AddSprite(item);
                }

                return textureAtlas;
            } catch(Exception ex) {
                Debug.LogException(ex);
                return null;
            }
        }

        private UITextureAtlas atlas_;
        public UITextureAtlas Atlas => atlas_ ??= CreateAtlas();

        public string Tooltip => get_Tooltip_();

        public void BuildPanel(UIPanel panel) => buildPanel_(panel);
        public void RefreshUI() => refreshUI_();
        #endregion

        #region Controller
        public bool Enabled => get_Enabled_();
        public Dictionary<NetInfo, ICloneable> BuildCustomData() => buildCustomData_();
        public void LoadWithData(ICloneable data) => loadWithData_(data);
        public void Reset() => reset_();
        public void LoadActiveSelection() => loadActiveSelection_();
        #endregion
    }
}
