using ColossalFramework.IO;
using NetworkSkins.Skins.Modifiers;
using NetworkSkins.Skins.Serialization;

namespace NetworkSkins.Skins
{
    public abstract class NetworkSkinModifier
    {
        private readonly NetworkSkinModifierType _type;

        protected NetworkSkinModifier(NetworkSkinModifierType type)
        {
            _type = type;
        }

        public abstract void Apply(NetworkSkin skin);

        #region Serialization
        public void Serialize(DataSerializer s)
        {
            s.WriteUInt8((uint)_type);
            SerializeImpl(s);
        }

        protected abstract void SerializeImpl(DataSerializer s);

        // nullable
        public static NetworkSkinModifier Deserialize(DataSerializer s, IPrefabCollection prefabCollection, NetworkSkinLoadErrors errors)
        {
            var type = (NetworkSkinModifierType)s.ReadUInt8();
            switch (type)
            {
                case NetworkSkinModifierType.TerrainSurface:
                    return TerrainSurfaceModifier.DeserializeImpl(s);
                case NetworkSkinModifierType.Color:
                    return ColorModifier.DeserializeImpl(s);
                case NetworkSkinModifierType.StreetLight:
                    return StreetLightModifier.DeserializeImpl(s, prefabCollection, errors);
                case NetworkSkinModifierType.Tree:
                    return TreeModifier.DeserializeImpl(s, prefabCollection, errors);
                case NetworkSkinModifierType.Pillar:
                    return PillarModifier.DeserializeImpl(s, prefabCollection, errors);
                case NetworkSkinModifierType.Catenary:
                    return CatenaryModifier.DeserializeImpl(s, prefabCollection, errors);
                case NetworkSkinModifierType.RoadDecoration:
                    return RoadDecorationModifier.DeserializeImpl(s);
                default:
                    return null;
            }
        }
        #endregion
    }

    public enum NetworkSkinModifierType
    {
        TerrainSurface = 1,
        Color = 2,
        StreetLight = 3,
        Tree = 4,
        Pillar = 5,
        Catenary = 6,
        RoadDecoration = 7
    }
}
