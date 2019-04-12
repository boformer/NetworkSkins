using System;
using System.Collections.Generic;
using System.IO;
using ColossalFramework.Plugins;
using JetBrains.Annotations;
using UnityEngine;

namespace NetworkSkins.Skins
{
    public class ThemeTextureManager : ModSingleton<ThemeTextureManager>
    {
        private readonly Dictionary<TextureKey, ThemeTexture> _textures = new Dictionary<TextureKey, ThemeTexture>();

        public void OnDestroy()
        {
            ClearTextures();
        }

        public void LoadTextures()
        {
            var terrainProperties = TerrainManager.instance.m_properties;
            var netProperties = NetManager.instance.m_properties;

            var averagePavementColor = CalculateAverageColor(terrainProperties.m_pavementDiffuse);
            var averageGravelColor = CalculateAverageColor(terrainProperties.m_gravelDiffuse);
            var averageRuinedColor = CalculateAverageColor(terrainProperties.m_ruinedDiffuse);
            var averageRoadColor = CalculateAverageColor(netProperties.m_upwardDiffuse as Texture2D);

            Debug.Log($"Average colors: pavement: {averagePavementColor}, gravel: {averageGravelColor}, ruined: {averageRuinedColor}, road: {averageRoadColor}");

            var modPath = PluginManager.instance.FindPluginInfo(GetType().Assembly)?.modPath;
            if (modPath == null)
            {
                Debug.LogError("Mod path not found!");
                return;
            }

            var texturePackPath = Path.Combine(modPath, "networkskins_textures.unity3d");
            Debug.Log($"texturePackPath: {texturePackPath}");

            var bundle = AssetBundle.LoadFromFile(texturePackPath);

            var marbleTilesTexture = bundle.LoadAsset<Texture2D>("Assets/marble_tiles.png");
            _textures[new TextureKey(ThemeTextureType.Pavement, "marble_tiles")] = new ThemeTexture(
                diffuse: CreateTintedTexture(marbleTilesTexture, averagePavementColor), 
                tiling: 0.25f
            );
            _textures[new TextureKey(ThemeTextureType.Gravel, "marble_tiles")] = new ThemeTexture(
                diffuse: CreateTintedTexture(marbleTilesTexture, averageGravelColor),
                tiling: 0.25f
            );
            _textures[new TextureKey(ThemeTextureType.Ruined, "marble_tiles")] = new ThemeTexture(
                diffuse: CreateTintedTexture(marbleTilesTexture, averageRuinedColor),
                tiling: 0.25f
            );

            var rectanglePaversTexture = bundle.LoadAsset<Texture2D>("Assets/rectangle_pavers.png");
            _textures[new TextureKey(ThemeTextureType.Pavement, "rectangle_pavers")] = new ThemeTexture(
                diffuse: CreateTintedTexture(rectanglePaversTexture, averagePavementColor),
                tiling: 0.25f
            );
            _textures[new TextureKey(ThemeTextureType.Gravel, "rectangle_pavers")] = new ThemeTexture(
                diffuse: CreateTintedTexture(rectanglePaversTexture, averageGravelColor),
                tiling: 0.25f
            );
            _textures[new TextureKey(ThemeTextureType.Ruined, "rectangle_pavers")] = new ThemeTexture(
                diffuse: CreateTintedTexture(rectanglePaversTexture, averageRuinedColor),
                tiling: 0.25f
            );

            var straightCobblestoneTexture = bundle.LoadAsset<Texture2D>("Assets/straight_cobblestone.png");
            _textures[new TextureKey(ThemeTextureType.Road, "straight_cobblestone")] = new ThemeTexture(
                diffuse: CreateTintedTexture(straightCobblestoneTexture, averageRoadColor),
                tiling: 2.05f // ignored, always 16.4!
            );

            bundle.Unload(true);
        }

        public void ClearTextures()
        {
            foreach (var texture in _textures.Values)
            {
                Destroy(texture.Diffuse);
            }
            _textures.Clear();
        }

        [CanBeNull]
        public ThemeTexture GetTexture(ThemeTextureType textureType, string textureName)
        {
            return _textures.TryGetValue(new TextureKey(textureType, textureName), out var texture) ? texture : null;
        }

        private static Texture2D CreateTintedTexture(Texture2D pattern, Color tint)
        {
            var patternPixels = pattern.GetPixels();

            var texture = new Texture2D(pattern.width, pattern.height)
            {
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Trilinear,
                anisoLevel = 4
            };
            var texturePixels = texture.GetPixels();
            var pixelsLength = texturePixels.Length;

            for (var i = 0; i < pixelsLength; i++)
            {
                var r = Mathf.Clamp(tint.r + patternPixels[i].r - 0.5f, 0f, 1f);
                var g = Mathf.Clamp(tint.g + patternPixels[i].g - 0.5f, 0f, 1f);
                var b = Mathf.Clamp(tint.b + patternPixels[i].b - 0.5f, 0f, 1f);
                texturePixels[i] = new Color(r, g, b, 1f);
            }

            texture.SetPixels(texturePixels);
            texture.Apply();

            return texture;
        }

        private static Color CalculateAverageColor(Texture2D texture)
        {
            if (texture == null) return Color.gray;
            try
            {
                return CalculateAverageColorImpl(texture);
            }
            catch (UnityException) // texture not readable
            {
                var readableTexture = texture.MakeReadable();
                var averageColor = CalculateAverageColorImpl(readableTexture);
                Destroy(readableTexture);
                return averageColor;
            }
        }

        private static Color CalculateAverageColorImpl(Texture2D texture)
        {
            var texColors = texture.GetPixels();
            var total = texColors.Length;
            float r = 0f, g = 0f, b = 0f;
            for (var i = 0; i < total; i++)
            {
                r += texColors[i].r;
                g += texColors[i].g;
                b += texColors[i].b;
            }
            return new Color(r / total, g / total, b / total, 1f);

        }
        
        private sealed class TextureKey
        {
            private readonly ThemeTextureType _type;
            private readonly string _name;


            public TextureKey(ThemeTextureType type, string name)
            {
                _type = type;
                _name = name ?? throw new ArgumentNullException(nameof(name));
            }

            #region Equality
            private bool Equals(TextureKey other)
            {
                return _type == other._type && string.Equals(_name, other._name);
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

                return obj is TextureKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((int) _type * 397) ^ _name.GetHashCode();
                }
            }
            #endregion
        }
    }

    public class ThemeTexture
    {
        public readonly Texture2D Diffuse;
        public readonly float Tiling;

        public ThemeTexture(Texture2D diffuse, float tiling)
        {
            Diffuse = diffuse ?? throw new ArgumentNullException(nameof(diffuse));
            Tiling = tiling;
        }
    }

    public enum ThemeTextureType
    {
        Pavement,
        Gravel,
        Ruined,
        Road
    }
}
