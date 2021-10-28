using NetworkSkins.API;
using NetworkSkins.GUI.Abstraction;
using NetworkSkins.Skins;
using NetworkSkins.Skins.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace NetworkSkins.GUI.Custom {
    public class CustomPanelController : FeaturePanelControllerBase {
        public readonly NSImplementationWrapper Implementation;

        public CustomPanelController(NSImplementationWrapper impl) {
            Implementation = impl;
        }

        public override bool Enabled => base.Enabled && Implementation.Enabled;

        public Dictionary<NetInfo, CustomDataCollectionModifier> CustomDatas { get; private set; }
        protected override void OnChanged() {
            if(Enabled && Prefab != null) {
                CustomDatas = BuildCustomData().ToDictionary(
                    keySelector: pair => pair.Key,
                    elementSelector: pair => DataToCollection(pair.Value));
            } else {
                CustomDatas = new Dictionary<NetInfo, CustomDataCollectionModifier>();
            }

            Implementation.SaveActiveSelection();
            
            base.OnChanged();
        }
        public void Changed() => OnChanged();

        private CustomDataCollectionModifier DataToCollection(ICloneable data) {
            var ret = new CustomDataCollection();
            ret[Implementation.ID] = data;
            return new CustomDataCollectionModifier(ret);
        }

        protected Dictionary<NetInfo, ICloneable> BuildCustomData() => Implementation.BuildCustomData();

        protected override void BuildWithModifiers(List<NetworkSkinModifier> modifiers) {
            var modifer = modifiers?.OfType<CustomDataCollectionModifier>()?.FirstOrDefault();
            ICloneable data = modifer?.Data?[Implementation.ID];
            Implementation.LoadWithData(data);
            Implementation.SaveActiveSelection();
        }

        #region Active Selection Data

        public override void Reset() {
            if(!Enabled) return;

            Implementation.Reset();

            OnChanged();
        }

        protected override void Build() => Implementation.LoadActiveSelection();
        #endregion
    }
}