/*
using Harmony;
using NetworkSkins.Skins;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.NetManager
{
    // TODO check compat with ParallelRoadTool
    [HarmonyPatch(typeof(global::NetManager), "CreateNode")]
    public static class NetManagerCreateNodePatch
    {
        public static ushort SplitSegment_createdNode = 0;

        public static void Postfix(ref ushort node, NetInfo info, bool __result)
        {
            const int firstStackFrameIndex = 2;

            // 0 is this method
            // 1 is CreateNode_Patch<n> (The original method that was patched by Harmony)
            // (even when multiple patches are applied, harmony does not cause additional stack frames)
            var caller1 = new System.Diagnostics.StackFrame(firstStackFrameIndex).GetMethod();
            var caller2 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 1).GetMethod();
            var caller3 = new System.Diagnostics.StackFrame(firstStackFrameIndex + 2).GetMethod();

            Debug.Log($"CreateNode: caller1: {caller1.Name}, caller2: {caller2.Name}, caller3: {caller3.Name}");

            if (caller1.Name == "CreateNode" || caller1.Name.StartsWith("CreateNode_Patch"))
            {
                if (caller2.Name == "CreateNode" || caller2.Name.StartsWith("CreateNode_Patch"))
                {
                    NetworkSkinManager.instance.OnNodeCreate(node);
                }
                else if (caller2.Name == "LoadPaths" || caller2.Name.StartsWith("LoadPaths_Patch"))
                {
                    // segment created because user placed building with integrated network
                    // currently not doing anything
                }
            }
            else if (caller1.Name == "MoveMiddleNode" || caller1.Name.StartsWith("MoveMiddleNode_Patch"))
            {
                if (NetManagerReleaseNodePatch.MoveMiddleNode_releasedNode > 0)
                {
                    if (__result)
                    {
                        NetworkSkinManager.instance.OnNodeTransferData(NetManagerReleaseNodePatch.MoveMiddleNode_releasedNode, node);
                    }

                    // Delete data of previous segment
                    NetManagerReleaseSegmentPatch.MoveMiddleNode_releasedSegment = 0;
                }
            }
            else if (caller1.Name == "SplitSegment" || caller1.Name.StartsWith("SplitSegment_Patch"))
            {
                if (__result)
                {
                    if (SplitSegment_createdNode > 0)
                    {
                        Debug.Log("SplitSegment_createdNode overflow!");
                    }
                    SplitSegment_createdNode = node;
                }
            }
        }
    }
}
*/