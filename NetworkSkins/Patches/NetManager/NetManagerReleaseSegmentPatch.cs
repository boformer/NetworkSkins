using Harmony;
using NetworkSkins.Skins;
using UnityEngine;

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

            // Debug.Log($"ReleaseSegment: {caller1.Name}");

            // segment that was modified because user added network, keep data until replacement segments were created
            if (caller1.Name == "MoveMiddleNode" || caller1.Name.StartsWith("MoveMiddleNode_Patch"))
            {
                // Delete data of last moved segment
                if (MoveMiddleNode_releasedSegment > 0)
                {
                    Debug.Log("MoveMiddleNode_releasedSegment overflow!");
                    NetworkSkinManager.instance.OnSegmentRelease(MoveMiddleNode_releasedSegment);
                }

                // Save segment id
                MoveMiddleNode_releasedSegment = segment;
            }
            // segment that was split by new node, keep data until replacement segments were created
            else if (caller1.Name == "SplitSegment" || caller1.Name.StartsWith("SplitSegment_Patch"))
            {
                // Delete data of last splitted segment
                if (SplitSegment_releasedSegment > 0)
                {
                    Debug.Log("SplitSegment_releasedSegment overflow!");
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
    }
}
