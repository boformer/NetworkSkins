using ColossalFramework.IO;
using UnityEngine;

namespace NetworkSkins.Skins.Modifiers
{
    public class ColorModifier : NetworkSkinModifier
    {
        public readonly Color32 Color;

        public ColorModifier(Color color) : base(NetworkSkinModifierType.Color)
        {
            Color = color;
        }

        public override void Apply(NetworkSkin skin)
        {
            skin.m_color = Color;
        }

        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteUInt8(Color.r);
            s.WriteUInt8(Color.g);
            s.WriteUInt8(Color.b);
            s.WriteUInt8(Color.a);
        }

        public static ColorModifier DeserializeImpl(DataSerializer s)
        {
            var color = new Color32((byte)s.ReadUInt8(), (byte)s.ReadUInt8(), (byte)s.ReadUInt8(), (byte)s.ReadUInt8());
            return new ColorModifier(color);
        }
        #endregion

        #region Equality
        protected bool Equals(ColorModifier other)
        {
            return Color.Equals(other.Color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ColorModifier)obj);
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }
        #endregion
    }
}
