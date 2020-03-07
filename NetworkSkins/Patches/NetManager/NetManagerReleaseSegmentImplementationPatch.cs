using System;
using System.Reflection;
using HarmonyLib;
using NetworkSkins.Skins;
using NetworkSkins.Patches.NetTool;
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
            if (!NS2HelpersExtensions.InSimulationThread())
            {
                return;
            }

            if (NetToolMoveMiddleNodePatch.Called) {
                NS2HelpersExtensions.Assert(NetToolMoveMiddleNodePatch.SegmentID == 0, "expected NetToolMoveMiddleNodePatch.SegmentID == 0");
                NetToolMoveMiddleNodePatch.SegmentID = segment; // do not release skin here as it is necessary later.
            } else if (NetToolSplitSegmentPatch.Called) {
                NS2HelpersExtensions.Assert(NetToolSplitSegmentPatch.SegmentID == 0, "expected NetToolSplitSegmentPatch.SegmentID == 0");
                NetToolSplitSegmentPatch.SegmentID = segment; // do not release skin here as it is necessary later.
            } else {
                NetworkSkinManager.instance.OnSegmentRelease(segment);
            }
        }
    }
}
