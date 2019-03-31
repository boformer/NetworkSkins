using Harmony;
using NetworkSkins.Skins;

// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch(typeof(global::NetManager), "ReleaseSegment")]
    public static class NetManagerReleaseSegmentPatch
    {
        public static ushort MoveMiddleNode_releasedSegment;
        public static ushort SplitSegment_releasedSegment;

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
                    if (MoveMiddleNode_releasedSegment > 0)
                    {
                        NetworkSkinManager.instance.OnSegmentRelease(MoveMiddleNode_releasedSegment);
                    }

                    // Save segment id
                    MoveMiddleNode_releasedSegment = segment;
                    break;

                case "SplitSegment": // segment that was split by new node, keep data until replacement segments were created

                    // Delete data of last splitted segment
                    if (SplitSegment_releasedSegment > 0)
                    {
                        NetworkSkinManager.instance.OnSegmentRelease(SplitSegment_releasedSegment);
                    }

                    // Save segment id
                    SplitSegment_releasedSegment = segment;
                    break;

                case "DeleteSegmentImpl": // segment deleted with bulldozer by user, delete data
                case "ReleasePaths": // segment deleted because user bulldozed building with integrated networks, delete data
                default: // unknown caller, delete data

                    NetworkSkinManager.instance.OnSegmentRelease(segment);
                    break;
            }
        }
    }
}
