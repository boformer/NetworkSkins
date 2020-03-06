using HarmonyLib;
using NetworkSkins.Skins;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch(typeof(global::NetManager), "CreateSegment")]
    public static class NetManagerCreateSegmentPatch
    {
        public static void Postfix(ref ushort segment, NetInfo info, bool __result)
        {
            var firstStackFrameIndex = 2;

            // 0 is this method
            // 1 is CreateSegment_Patch<n> (The original method that was patched by Harmony)
            // (even when multiple patches are applied, harmony does not cause additional stack frames)
            var caller1 = new System.Diagnostics.StackFrame(firstStackFrameIndex).GetMethod();
            var caller2 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 1).GetMethod();
            var caller3 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 2).GetMethod();

            // Support for ParallelRoadTool
            if (IsNameMatching(caller1, "CreateSegmentOriginal") && IsNameMatching(caller2, "CreateSegment"))
            {
                firstStackFrameIndex += 2;
                caller1 = new System.Diagnostics.StackFrame(firstStackFrameIndex).GetMethod();
                caller2 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 1).GetMethod();
                caller3 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 2).GetMethod();
            }

            if (IsNameMatching(caller1, "CreateNode"))
            {
                if (IsNameMatching(caller2, "CreateNode"))
                {
                    // check that caller was called by NetTool
                    var caller3Type = caller3.DeclaringType;
                    if (caller3Type != null && (typeof(global::NetTool).IsAssignableFrom(caller3Type))) // new segment created by user, apply selected style

                    {
                        if (__result)
                        {
                            NetworkSkinManager.instance.OnSegmentPlaced(segment);
                        }

                        // Delete data of deleted segments
                        if (NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment > 0)
                        {
                            NetworkSkinManager.instance.OnSegmentRelease(NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment);
                        }

                        if (NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment > 0)
                        {
                            NetworkSkinManager.instance.OnSegmentRelease(NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment);
                        }

                        NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment = 0;
                        NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment = 0;
                    }
                }
                else if (IsNameMatching(caller2, "LoadPaths"))
                {
                    // segment created because user placed building with integrated network
                    // currently not doing anything
                }
            }
            // segment that was modified because user added network, apply style of previous segment
            else if (IsNameMatching(caller1, "MoveMiddleNode"))
            {
                if (NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment > 0)
                {
                    if (__result)
                    {
                        NetworkSkinManager.instance.OnSegmentTransferData(NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment, segment);
                    }

                    // Delete data of previous segment
                    NetworkSkinManager.instance.OnSegmentRelease(NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment);
                    NetManagerReleaseSegmentImplementationPatch.MoveMiddleNode_releasedSegment = 0;
                }
            }
            // segment that was split by new node, apply style of previous segment
            else if (IsNameMatching(caller1, "SplitSegment"))
            {
                if (NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment > 0)
                {
                    if (__result)
                    {
                        NetworkSkinManager.instance.OnSegmentTransferData(NetManagerReleaseSegmentImplementationPatch.SplitSegment_releasedSegment, segment);
                    }
                }
            }
        }

        public static bool IsNameMatching(MethodBase method, string name)
        {
            return method.Name == name
                || method.Name.StartsWith($"{name}_Patch")
                || method.Name.StartsWith($"DMD<DMD<{name}_Patch");
        }
    }
}
