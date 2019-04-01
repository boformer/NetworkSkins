using UnityEngine;

namespace NetworkSkins.Skins
{
    public class ColorModifier : NetworkSkinModifier
    {
        public readonly Color Color;

        public ColorModifier(Color color)
        {
            Color = color;
        }

        public override void Apply(NetworkSkin skin)
        {
            skin.m_color = Color;
        }

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

            if (obj.GetType() != this.GetType())
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
