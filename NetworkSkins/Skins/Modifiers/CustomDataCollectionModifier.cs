using ColossalFramework.IO;
using System;

namespace NetworkSkins.Skins.Modifiers {
    public interface ICustomData : ICloneable {
        void Serialize(DataSerializer s);

        //public static ICustomData Deserialize(DataSerializer s);
    }

    public class CustomDataCollectionModifier : NetworkSkinModifier {
        public readonly CustomDataColloction Data;

        public CustomDataCollectionModifier(CustomDataColloction data) : base(NetworkSkinModifierType.Custom) {
            Data = data.Clone();
        }


        public override void Apply(NetworkSkin skin) {
            skin.m_CustomDatas = Data.Clone();
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s) => Data.Serialize(s);

        public static CustomDataCollectionModifier DeserializeImpl(DataSerializer s) =>
            new CustomDataCollectionModifier(CustomDataColloction.Deserialize(s));
        #endregion

        #region Equality

        public override bool Equals(object obj) => Data.Equals(obj);

        public override int GetHashCode() => Data.GetHashCode();
        #endregion
    }
}
