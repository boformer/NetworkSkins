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
    }
}
