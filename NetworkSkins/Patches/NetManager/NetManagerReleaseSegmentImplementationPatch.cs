using System;
using System.Reflection;
using HarmonyLib;
using NetworkSkins.Skins;

// ReSharper disable InconsistentNaming


namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch]
    public static class NetManagerReleaseSegmentImplementationPatch
    {
        public static ushort MoveMiddleNode_releasedSegment;
        public static ushort SplitSegment_releasedSegment;

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
            // 0 is this method
            // 1 is ReleaseSegmentImplementation_Patch<n> (The original method that was patched by Harmony)
            // 2 is ReleaseSegment or ReleaseSegmentImplementation 
            // (even when multiple patches are applied, harmony does not cause additional stack frames)
            var caller1 = new System.Diagnostics.StackFrame(2).GetMethod();
            var caller2 = new System.Diagnostics.StackFrame(3).GetMethod();

            if (caller1.Name == "ReleaseSegment" || caller1.Name.StartsWith("ReleaseSegment_Patch"))
            {
                // segment that was modified because user added network, keep data until replacement segments were created
                if (caller2.Name == "MoveMiddleNode" || caller2.Name.StartsWith("MoveMiddleNode_Patch"))
                {
                    // Delete data of last moved segment
                    if (MoveMiddleNode_releasedSegment > 0)
                    {
                        NetworkSkinManager.instance.OnSegmentRelease(MoveMiddleNode_releasedSegment);
                    }

                    // Save segment id
                    MoveMiddleNode_releasedSegment = segment;
                }
                // segment that was split by new node, keep data until replacement segments were created
                else if (caller2.Name == "SplitSegment" || caller2.Name.StartsWith("SplitSegment_Patch"))
                {
                    // Delete data of last splitted segment
                    if (SplitSegment_releasedSegment > 0)
                    {
                        NetworkSkinManager.instance.OnSegmentRelease(SplitSegment_releasedSegment);
                    }

                    // Save segment id
                    SplitSegment_releasedSegment = segment;
                }
                // DeleteSegmentImpl: segment deleted with bulldozer by user, delete data
                // ReleasePaths: segment deleted because user bulldozed building with integrated networks, delete data
                else
                {
                    NetworkSkinManager.instance.OnSegmentRelease(segment);
                }
            }
            else
            {
                NetworkSkinManager.instance.OnSegmentRelease(segment);
            }
        }
    }
}
