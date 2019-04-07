using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkSkins.Net
{
    public static class NetTextureUtils
    {
        private static readonly Dictionary<string, bool> RoadTextureDict = new Dictionary<string, bool>();

        public static bool HasRoadTexture(NetInfo prefab)
        {
            if (RoadTextureDict.TryGetValue(prefab.name, out var result))
            {
                return result;
            }
            else
            {
                return RoadTextureDict[prefab.name] = CalculateHasRoadTexture(prefab);
            }
        }

        private static bool CalculateHasRoadTexture(NetInfo prefab)
        {
            if (prefab != null && prefab.m_class.m_service == ItemClass.Service.Road
                                && prefab.m_segments != null && prefab.m_segments.Length > 0
                                && prefab.m_segments[0].m_material != null)
            {
                Texture2D texture = prefab.m_segments[0].m_material.GetTexture("_APRMap") as Texture2D;
                if (texture != null)
                {
                    try
                    {
                        return HasBluePixels(texture.GetPixels());
                    }
                    catch (UnityException) // texture not readable
                    {
                        Texture2D readableTexture = texture.MakeReadable();
                        bool hasBluePixels = HasBluePixels(readableTexture.GetPixels());
                        Object.Destroy(readableTexture);
                        return hasBluePixels;
                    }
                }
            }
            return false;
        }

        private static bool HasBluePixels(Color[] pixels)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].b > 0.05f)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
