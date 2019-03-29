using Harmony;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches
{
    [HarmonyPatch(typeof(NetSegment), "TerrainUpdated")]
    public static class NetSegmentTerrainUpdatedPatch
    {
        public static void Prefix(ref NetSegment __instance, ushort segmentID, out TerrainSurfacePatcherState __state)
        {
            __state = TerrainSurfacePatcher.Apply(__instance.Info, NetworkSkinManager.SegmentSkins[segmentID]);
        }

        public static void Postfix(ref NetSegment __instance, TerrainSurfacePatcherState __state)
        {
            TerrainSurfacePatcher.Revert(__instance.Info, __state);
        }
    }
}