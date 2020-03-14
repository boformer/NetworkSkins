using System;
using System.Reflection;
using Harmony;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming


namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch]
    public static class NetManagerReleaseSegmentImplementationPatch
    {
        public static MethodBase TargetMethod()
        {
            // ReleaseSegmentImplementation(ushort segment, ref NetSegment data, bool keepNodes)
            return typeof(global::NetManager).GetMethod("ReleaseSegmentImplementation", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[]
            {
                typeof(ushort), typeof(global::NetSegment).MakeByRefType(), typeof(bool)
            }, null);
        }

        public static void Prefix(ushort segment)
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                NetworkSkinManager.instance.OnSegmentRelease(segment);
            }
        }
    }
}
