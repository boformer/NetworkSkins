using System;
using HarmonyLib;
using NetworkSkins.Patches.NetTool;
using NetworkSkins.Skins;

// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch(typeof(global::NetManager), "CreateSegment")]
    public static class NetManagerCreateSegmentPatch
    {
        public static void Postfix(ref ushort segment, bool __result) {
            if(!__result || !NSUtil.InSimulationThread()) {
                return;
            }

            if(NetToolMoveMiddleNodePatch.CopySkin) {
                NetworkSkinManager.instance.PasteSegmentSkin(segment, NetToolMoveMiddleNodePatch.Skin);
            } else if(NetToolSplitSegmentPatch.CopySkin) {
                NetworkSkinManager.instance.PasteSegmentSkin(segment, NetToolSplitSegmentPatch.Skin);
            } else if(NetToolCreateNode0Patch.Called) {
                // only when it is called from nettool.
                NetworkSkinManager.instance.OnSegmentPlaced(segment);
            }

        }
    }
}
