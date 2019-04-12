using System;
using ColossalFramework.IO;
using NetworkSkins.Skins.Serialization;
using UnityEngine;

namespace NetworkSkins.Skins.Modifiers
{
    public class ThemeTextureModifier : NetworkSkinModifier
    {
        public readonly ThemeTextureType TextureType;
        public readonly string TextureName;

        public ThemeTextureModifier(ThemeTextureType textureType, string textureName) : base(NetworkSkinModifierType.ThemeTexture)
        {
            TextureType = textureType;
            TextureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
        }

        public override void Apply(NetworkSkin skin)
        {
            var themeTexture = ThemeTextureManager.instance.GetTexture(TextureType, TextureName);
            if (themeTexture == null) return;

            if (skin.m_segments != null)
            {
                for (var s = 0; s < skin.m_segments.Length; s++)
                {
                    if (skin.m_segments[s].m_segmentMaterial != null)
                    {
                        skin.UpdateSegmentMaterial(s, material => UpdateMaterial(material, themeTexture));
                    }
                }
            }

            if (skin.m_nodes != null)
            {
                for (var n = 0; n < skin.m_nodes.Length; n++)
                {
                    if (skin.m_nodes[n].m_nodeMaterial != null)
                    {
                        skin.UpdateNodeMaterial(n, material => UpdateMaterial(material, themeTexture));
                    }
                }
            }
        }

        private void UpdateMaterial(Material material, ThemeTexture themeTexture)
        {
            if (TextureType == ThemeTextureType.Pavement)
            {
                material.SetTexture("_TerrainPavementDiffuse", themeTexture.Diffuse);
                var tiling = GetVectorShaderParameter(material, "_TerrainTextureTiling1");
                tiling.x = themeTexture.Tiling;
                material.SetVector("_TerrainTextureTiling1", tiling);
            }
            else if (TextureType == ThemeTextureType.Gravel)
            {
                material.SetTexture("_TerrainGravelDiffuse", themeTexture.Diffuse);
                var tiling = GetVectorShaderParameter(material, "_TerrainTextureTiling2");
                tiling.y = themeTexture.Tiling;
                material.SetVector("_TerrainTextureTiling2", tiling);
            }
            else if (TextureType == ThemeTextureType.Ruined)
            {
                material.SetTexture("_TerrainRuinedDiffuse", themeTexture.Diffuse);
                var tiling = GetVectorShaderParameter(material, "_TerrainTextureTiling1");
                tiling.y = themeTexture.Tiling;
                material.SetVector("_TerrainTextureTiling1", tiling);
            }
            else if (TextureType == ThemeTextureType.Road)
            {
                material.SetTexture("_RoadUpwardDiffuse", themeTexture.Diffuse);
                // no tiling factor possible for road upwards diffuse. it is fixed to 16.4m!
            }
        }

        private static Vector4 GetVectorShaderParameter(Material material, string name)
        {
            var value = material.GetVector(name);
            return value != default(Vector4) ? value : Shader.GetGlobalVector(name);
        }


        #region Serialization
        protected override void SerializeImpl(DataSerializer s)
        {
            s.WriteUInt8((uint)TextureType);
            s.WriteUniqueString(TextureName);
        }

        public static ThemeTextureModifier DeserializeImpl(DataSerializer s, NetworkSkinLoadErrors errors)
        {
            var textureType = (ThemeTextureType)s.ReadUInt8();
            var textureName = s.ReadUniqueString();

            return new ThemeTextureModifier(textureType, textureName);
        }
        #endregion

        #region Equality
        protected bool Equals(ThemeTextureModifier other)
        {
            return TextureType == other.TextureType && string.Equals(TextureName, other.TextureName);
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

            return Equals((ThemeTextureModifier) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) TextureType * 397) ^ TextureName.GetHashCode();
            }
        }
        #endregion
    }
}
