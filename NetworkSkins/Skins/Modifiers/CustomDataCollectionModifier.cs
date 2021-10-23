using ColossalFramework.IO;
using System;

namespace NetworkSkins.Skins.Modifiers {
    public class CustomDataCollectionModifier : NetworkSkinModifier {
        public readonly CustomDataCollection Data;

        public CustomDataCollectionModifier(CustomDataCollection data) : base(NetworkSkinModifierType.Custom) {
            Data = data.Clone();
        }


        public override void Apply(NetworkSkin skin) {
            skin.m_CustomDatas = Data.Clone();
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s) {
            s.WriteUniqueString(Data.Encode64());
        }

        public static CustomDataCollectionModifier DeserializeImpl(DataSerializer s) {
            var data = CustomDataCollection.Decode64(s.ReadUniqueString());
            return new CustomDataCollectionModifier(data);
        }
        #endregion

        #region Equality

        public override bool Equals(object obj) => Data.Equals(obj);

        public override int GetHashCode() => Data.GetHashCode();
        #endregion
    }
}
