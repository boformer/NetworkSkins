using Harmony;
using NetworkSkins.Skins;
using System;
using System.Reflection;
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
