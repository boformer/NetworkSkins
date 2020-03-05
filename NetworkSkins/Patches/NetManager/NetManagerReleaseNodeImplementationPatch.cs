using System;
using System.Reflection;
using HarmonyLib;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming

// TODO maybe use ManualActivation, ManualDeactivation, AfterSplitOrMove instead?

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch]
    public static class NetManagerReleaseNodeImplementationPatch
    {
        public static MethodBase TargetMethod()
        {
            // ReleaseNodeImplementation(ushort node, ref NetNode data)
            return typeof(global::NetManager).GetMethod("ReleaseNodeImplementation", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[]
            {
                typeof(ushort), typeof(global::NetNode).MakeByRefType(),
            }, null);
        }

        public static void Prefix(ushort node)
        {
            NetworkSkinManager.instance.OnNodeRelease(node);
        }
    }
}