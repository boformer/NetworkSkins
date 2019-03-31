using Harmony;

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch(typeof(global::NetManager), "ReleaseNode")]
    public static class NetManagerReleaseNodePatch
    {
        public static void Prefix(ushort node)
        {
            NetManagerHooks.OnNodeRelease(node);
        }
    }
}
