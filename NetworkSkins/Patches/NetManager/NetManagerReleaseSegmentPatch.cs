using Harmony;

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch(typeof(global::NetManager), "ReleaseSegment")]
    public static class NetManagerReleaseSegmentPatch
    {
        public static void Prefix(ushort segment)
        {
            // 0 is this method
            // 1 is ReleaseSegment_Patch<n> (The original method that was patched by Harmony)
            // (even when multiple patches are applied, harmony does not cause additional stack frames)
            var caller1 = new System.Diagnostics.StackFrame(2).GetMethod();

            switch (caller1.Name)
            {
                case "MoveMiddleNode": // segment that was modified because user added network, keep data until replacement segments were created

                    // Delete data of last moved segment
                    if (NetManagerHooks.MoveMiddleNode_releasedSegment > 0)
                    {
                        NetManagerHooks.OnSegmentRelease(NetManagerHooks.MoveMiddleNode_releasedSegment);
                    }

                    // Save segment id
                    NetManagerHooks.MoveMiddleNode_releasedSegment = segment;
                    break;

                case "SplitSegment": // segment that was split by new node, keep data until replacement segments were created

                    // Delete data of last splitted segment
                    if (NetManagerHooks.SplitSegment_releasedSegment > 0)
                    {
                        NetManagerHooks.OnSegmentRelease(NetManagerHooks.SplitSegment_releasedSegment);
                    }

                    // Save segment id
                    NetManagerHooks.SplitSegment_releasedSegment = segment;
                    break;

                case "DeleteSegmentImpl": // segment deleted with bulldozer by user, delete data
                case "ReleasePaths": // segment deleted because user bulldozed building with integrated networks, delete data
                default: // unknown caller, delete data

                    NetManagerHooks.OnSegmentRelease(segment);
                    break;
            }
        }
    }
}
