using Harmony;
using NetworkSkins.Skins;
using UnityEngine;
// ReSharper disable InconsistentNaming

// TODO maybe use ManualActivation, ManualDeactivation, AfterSplitOrMove instead?

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch(typeof(global::NetManager), "ReleaseNode")]
    public static class NetManagerReleaseNodePatch
    {
        //public static ushort MoveMiddleNode_releasedNode;

        public static void Prefix(ushort node)
        {
            const int firstStackFrameIndex = 2;

            // 0 is this method
            // 1 is ReleaseNode_Patch<n> (The original method that was patched by Harmony)
            // (even when multiple patches are applied, harmony does not cause additional stack frames)
            var caller1 = new System.Diagnostics.StackFrame(firstStackFrameIndex).GetMethod();

            Debug.Log($"ReleaseNode: {caller1.Name}");

            NetworkSkinManager.instance.OnNodeRelease(node);

            /*
            // node that was modified because user added network, keep data until replacement node is created
            if (caller1.Name == "MoveMiddleNode" || caller1.Name.StartsWith("MoveMiddleNode_Patch"))
            {
                // Delete data of last moved segment
                if (MoveMiddleNode_releasedNode > 0)
                {
                    Debug.Log("MoveMiddleNode_releasedNode overflow!");
                    NetworkSkinManager.instance.OnNodeRelease(MoveMiddleNode_releasedNode);
                }

                // Save segment id
                MoveMiddleNode_releasedNode = node;
            }
            // segment that was split by new node, keep data until replacement segments were created
            else if (caller1.Name == "SplitSegment" || caller1.Name.StartsWith("SplitSegment_Patch"))
            {
                if (NetManagerCreateNodePatch.SplitSegment_createdNode > 0)
                {
                    NetworkSkinManager.instance.OnNodeTransferData(node, NetManagerCreateNodePatch.SplitSegment_createdNode);
                }
                NetManagerCreateNodePatch.SplitSegment_createdNode = 0;

                NetworkSkinManager.instance.OnNodeRelease(node);
            }
            else
            {
                NetworkSkinManager.instance.OnNodeRelease(node);
            }
            */
        }
    }
}