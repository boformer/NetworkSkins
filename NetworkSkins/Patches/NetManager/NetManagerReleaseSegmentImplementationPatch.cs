using System;
using System.Reflection;
using HarmonyLib;
using NetworkSkins.Skins;
// ReSharper disable InconsistentNaming


namespace NetworkSkins.Patches.NetManager {
    [HarmonyPatch]
    public static class NetManagerReleaseSegmentImplementationPatch {
        public delegate void ReleaseSegmentImplementation(ushort segment, ref global::NetSegment data, bool keepNodes);
        
        public static MethodBase TargetMethod() =>
            typeof(global::NetManager).GetMethod<ReleaseSegmentImplementation>();

        public static void Prefix(ushort segment) {
            if(NSUtil.InSimulationThread()) {
                NetworkSkinManager.instance.OnSegmentRelease(segment);
            }
        }
    }
}
