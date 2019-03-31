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
            // 0 is this method
            // 1 is ReleaseNode_Patch<n> (The original method that was patched by Harmony)
            // (even when multiple patches are applied, harmony does not cause additional stack frames)
            // var caller1 = new System.Diagnostics.StackFrame(2).GetMethod();
            // Debug.Log($"ReleaseNode: {caller1.Name}");

            NetworkSkinManager.instance.OnNodeRelease(node);
        }
    }
}