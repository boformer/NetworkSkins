using NetworkSkins.Skins.Modifiers;

namespace NetworkSkins.Net
{
    // TODO rename to NetTerrainSurfaceUtil
    public class SurfaceUtil
    {
        public static bool SupportsSurfaces(NetInfo prefab)
        {
            return GetDefaultSurface(prefab) != Surface.None
                   && (prefab.m_segments == null || prefab.m_segments.Length == 0)
                   || NetTextureUtils.HasPavementTexture(prefab);
        }

        public static bool SupportsNoneSurface(NetInfo prefab)
        {
            return (prefab.m_segments == null || prefab.m_segments.Length == 0);
        }

        public static Surface GetDefaultSurface(NetInfo prefab)
        {
            if (prefab.m_createPavement)
            {
                return Surface.Pavement;
            }
            else if (prefab.m_createGravel)
            {
                return Surface.Gravel;
            }
            else if (prefab.m_createRuining)
            {
                return Surface.Ruined;
            }
            else
            {
                return Surface.None;
            }
        }

        public static Surface StringToSurface(string name) {
            switch (name) {
                case "None": return Surface.None;
                case "Pavement": return Surface.Pavement;
                case "Gravel": return Surface.Gravel;
                case "Ruined": return Surface.Ruined;
                default: return Surface.Unchanged;
            }
        }
    }
}
