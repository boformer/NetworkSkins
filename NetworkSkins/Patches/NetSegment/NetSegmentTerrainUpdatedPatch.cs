using HarmonyLib;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetSegment
{
    /// <summary>
    /// Used by terrain surface.
    /// </summary>
    [HarmonyPatch(typeof(global::NetSegment), "TerrainUpdated")]
    public static class NetSegmentTerrainUpdatedPatch
    {
        public static void Prefix(ref global::NetSegment __instance, ushort segmentID, out TerrainSurfacePatcherState __state)
        {
            __state = TerrainSurfacePatcher.Apply(__instance.Info, NetworkSkinManager.SegmentSkins[segmentID]);
        }

        public static void Postfix(ref global::NetSegment __instance, TerrainSurfacePatcherState __state)
        {
            TerrainSurfacePatcher.Revert(__instance.Info, __state);
        }
    }
}