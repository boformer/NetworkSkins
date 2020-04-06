using HarmonyLib;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.LoadingWrapper
{
    [HarmonyPatch(typeof(global::LoadingWrapper), "OnLevelLoaded")]
    public static class NATLOnLevelLoadedPatch
    {
        public static void Postfix()
        {
            NetworkSkinManager.instance.OnPostLevelLoaded();
        }
    }
}
