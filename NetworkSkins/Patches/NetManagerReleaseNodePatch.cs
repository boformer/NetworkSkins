using Harmony;

namespace NetworkSkins.Patches
{
    [HarmonyPatch(typeof(NetManager), "ReleaseNode")]
    public class NetManagerReleaseNodePatch
    {
        static void Prefix(ushort node)
        {
            NetManagerHooks.OnNodeRelease(node);
        }
    }
}
