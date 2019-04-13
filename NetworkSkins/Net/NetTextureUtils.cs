using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkSkins.Net
{
    public static class NetTextureUtils
    {
        private static readonly Dictionary<string, bool> PavementTextureDict = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> RoadTextureDict = new Dictionary<string, bool>();

        public static bool HasPavementTexture(NetInfo prefab)
        {
            if (PavementTextureDict.TryGetValue(prefab.name, out var result)) return result;

            // The P channel is inverted in APR map!
            return PavementTextureDict[prefab.name] = CalculateHasMatchingPixels(prefab, color => color.g < 0.95);
        }

        public static bool HasRoadTexture(NetInfo prefab)
        {
            if (RoadTextureDict.TryGetValue(prefab.name, out var result)) return result;

            return RoadTextureDict[prefab.name] = CalculateHasMatchingPixels(prefab, color => color.b > 0.05);
        }

        private static bool CalculateHasMatchingPixels(NetInfo prefab, Func<Color, bool> predicate)
        {
            if (prefab != null && prefab.m_segments != null)
            {
                foreach (var segment in prefab.m_segments)
                {
                    if(segment.m_material == null) continue;

                    Texture2D texture = segment.m_material.GetTexture("_APRMap") as Texture2D;
                    if (texture != null)
                    {
                        try
                        {
                            return HasMatchingPixels(texture.GetPixels(), predicate);
                        }
                        catch (UnityException) // texture not readable
                        {
                            Texture2D readableTexture = texture.MakeReadable();
                            bool hasBluePixels = HasMatchingPixels(readableTexture.GetPixels(), predicate);
                            Object.Destroy(readableTexture);
                            return hasBluePixels;
                        }
                    }
                }

            }
            return false;
        }

        private static bool HasMatchingPixels(Color[] pixels, Func<Color, bool> predicate)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if (predicate(pixels[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
