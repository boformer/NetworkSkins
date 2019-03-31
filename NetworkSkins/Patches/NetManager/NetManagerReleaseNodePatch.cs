using Harmony;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch(typeof(global::NetManager), "ReleaseNode")]
    public static class NetManagerReleaseNodePatch
    {
        public static void Prefix(ushort node)
        {
            NetworkSkinManager.instance.OnNodeRelease(node);
        }
    }
}
