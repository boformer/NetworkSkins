namespace NetworkSkins.API {
    using ColossalFramework.UI;
    using System;
    using NetworkSkins.Helpers;
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Resources = NetworkSkins.Resources;

    public class NSImplementationWrapper : INSImplementation{
        private static class Delegates {
            public delegate int get_Index(object impl);
            public delegate void set_Index(object impl, int value);
            public delegate string get_ID(object impl);
            public delegate void OnBeforeNSLoaded(object impl);
            public delegate void OnAfterNSLoaded(object impl);
            public delegate void OnSkinApplied(object impl, ICloneable data, InstanceID instanceID);
            public delegate void OnNSDisabled(object impl);

            public delegate Version get_DataVersion(object impl);
            public delegate string Encode64(object impl, ICloneable data);
            public delegate ICloneable Decode64(object impl, string base64Data, Version dataVersion);

            public delegate Texture2D get_Icon(object impl);
            public delegate string get_Tooltip(object impl);

            public delegate void BuildPanel(object impl, UIPanel panel);
            public delegate void RefreshUI(object impl);

            public delegate bool get_Enabled(object impl);
            public delegate Dictionary<NetInfo, ICloneable> LoadCustomData(object impl);
            public delegate void LoadWithData(object impl, ICloneable data);
            public delegate void Reset(object impl);
            public delegate void LoadActiveSelection(object impl);
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
        private Delegates.LoadCustomData loadCustomData_;
        private Delegates.LoadWithData loadWithData_;
        private Delegates.Reset reset_;
        private Delegates.LoadActiveSelection loadActiveSelection_;


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

            get_Icon_ = DelegateUtil.CreateDelegate<Delegates.get_Icon>(type, true);
            get_Tooltip_ = DelegateUtil.CreateDelegate<Delegates.get_Tooltip>(type, true);
            buildPanel_ = DelegateUtil.CreateDelegate < Delegates.BuildPanel> (type, true);
            refreshUI_ = DelegateUtil.CreateDelegate<Delegates.RefreshUI>(type, true);

            get_Enabled_  = DelegateUtil.CreateDelegate<Delegates.get_Enabled>(type, true);
            loadCustomData_ = DelegateUtil.CreateDelegate < Delegates.LoadCustomData> (type, true);
            loadWithData_ = DelegateUtil.CreateDelegate < Delegates.LoadWithData> (type, true);
            reset_ = DelegateUtil.CreateDelegate < Delegates.Reset> (type, true);
            loadActiveSelection_ = DelegateUtil.CreateDelegate < Delegates.LoadActiveSelection> (type, true);
        }

        public int Index { 
            get => get_Index_(Implemenation);
            set => set_Index_(Implemenation, value);
        }

        public string ID => get_ID_(Implemenation);
        public void OnBeforeNSLoaded() => onBeforeNSLoaded_(Implemenation);
        public void OnAfterNSLoaded() => onAfterNSLoaded_(Implemenation);
        public void OnSkinApplied(ICloneable data, InstanceID instanceID) => onSkinApplied_(Implemenation, data, instanceID);
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
        public Texture2D Icon => get_Icon_(Implemenation);

        public const string ForegroundIconName = "Icon";

        private UITextureAtlas CreateAtlas() {
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
            } catch (Exception ex) {
                Debug.LogException(ex);
                return null;
            }
        }

        private UITextureAtlas atlas_;
        public UITextureAtlas Atlas => atlas_ ??= CreateAtlas();

        public string Tooltip => get_Tooltip_(Implemenation);

        public void BuildPanel(UIPanel panel) => buildPanel_(Implemenation,panel);
        public void RefreshUI() => refreshUI_(Implemenation);
        #endregion

        #region Controller
        public bool Enabled => get_Enabled_(Implemenation);
        public Dictionary<NetInfo, ICloneable> BuildCustomData() => loadCustomData_(Implemenation);
        public void LoadWithData(ICloneable data) => loadWithData_(Implemenation, data);
        public void Reset() => reset_(Implemenation);
        public void LoadActiveSelection() => loadActiveSelection_(Implemenation);
        #endregion
    }
}
