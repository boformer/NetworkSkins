using NetworkSkins.API;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace NetworkSkins.GUI.Custom {
    public class CustomPanelController : FeaturePanelControllerBase {
        public readonly NSImplementationWrapper ImplementationWrapper;

        public CustomPanelController(NSImplementationWrapper implementationWrapper) {
            ImplementationWrapper = implementationWrapper;
        }

        public override bool Enabled => throw new NotImplementedException("api call"); // base.Enabled && ;


        public Dictionary<NetInfo, CustomDataCollectionModifier> CustomDatas { get; private set; }
        protected override void OnChanged() {
            if(Enabled && Prefab != null) {
                CustomDatas = BuildCustomData().ToDictionary(
                    keySelector: pair => pair.Key,
                    elementSelector: pair => DataToCollection(pair.Value));
            } else {
                CustomDatas = new Dictionary<NetInfo, CustomDataCollectionModifier>();
            }

            base.OnChanged();
        }

        private CustomDataCollectionModifier DataToCollection(ICloneable data) {
            var ret = new CustomDataCollection();
            ret[ImplementationWrapper.ID] = data;
            return new CustomDataCollectionModifier(ret);
        }


        protected Dictionary<NetInfo, ICloneable> BuildCustomData() {
            throw new NotImplementedException("API call");
        }

        protected override void BuildWithModifiers(List<NetworkSkinModifier> modifiers) {
            ICloneable data = modifiers
                ?.OfType<CustomDataCollectionModifier>()
                ?.Select(item => item.Data[ImplementationWrapper.ID])
                ?.FirstOrDefault();
            throw new NotImplementedException("API call");

        }

        #region Active Selection Data

        public override void Reset() {
            if(!Enabled) return;

            throw new NotImplementedException("API call");

            OnChanged();
        }

        protected override void Build() {
            throw new NotImplementedException("API call");
        }
        #endregion
    }
}