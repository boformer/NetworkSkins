using Harmony;

namespace NetworkSkins.Patches
{
    [HarmonyPatch(typeof(NetManager), "ReleaseNode")]
    public static class NetManagerReleaseNodePatch
    {
        public static void Prefix(ushort node)
        {
            NetManagerHooks.OnNodeRelease(node);
        }
    }
}
