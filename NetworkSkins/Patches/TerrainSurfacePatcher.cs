using System;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches
{
    /// <summary>
    /// Temporarily changes m_createPavement, m_createGravel, m_createRuining and m_clipTerrain
    /// of a NetInfo to the values defined in the skin of the node/segment
    /// </summary>
    public static class TerrainSurfacePatcher
    {
        public static TerrainSurfacePatcherState Apply(NetInfo info, NetworkSkin skin)
        {
            if (info == null || skin == null)
            {
                return TerrainSurfacePatcherState.None;
            }

            var state = TerrainSurfacePatcherState.Modified;
            if (info.m_createPavement) state |= TerrainSurfacePatcherState.Pavement;
            if (info.m_createGravel) state |= TerrainSurfacePatcherState.Gravel;
            if (info.m_createRuining) state |= TerrainSurfacePatcherState.Ruined;
            if (info.m_clipTerrain) state |= TerrainSurfacePatcherState.ClipTerrain;

            info.m_createPavement = skin.m_createPavement;
            info.m_createGravel = skin.m_createGravel;
            info.m_createRuining = skin.m_createRuining;
            info.m_clipTerrain = skin.m_clipTerrain;

            return state;
        }

        public static void Revert(NetInfo info, TerrainSurfacePatcherState state)
        {
            if (info == null || (state & TerrainSurfacePatcherState.Modified) == TerrainSurfacePatcherState.None)
            {
                return;
            }
           
            info.m_createPavement = (state & TerrainSurfacePatcherState.Pavement) != TerrainSurfacePatcherState.None;
            info.m_createGravel = (state & TerrainSurfacePatcherState.Gravel) != TerrainSurfacePatcherState.None;
            info.m_createRuining = (state & TerrainSurfacePatcherState.Ruined) != TerrainSurfacePatcherState.None;
            info.m_clipTerrain = (state & TerrainSurfacePatcherState.ClipTerrain) != TerrainSurfacePatcherState.None;
        }
    }

    [Flags]
    public enum TerrainSurfacePatcherState
    {
        None = 0,
        Modified = 1,
        Pavement = 1 << 1,
        Gravel = 1 << 2,
        Ruined = 1 << 3,
        ClipTerrain = 1 << 4
    }
}
