using Harmony;
using NetworkSkins.Patches.NetTool;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetManager
{
    // TODO check compat with ParallelRoadTool
    [HarmonyPatch(typeof(global::NetManager), "CreateSegment")]
    public static class NetManagerCreateSegmentPatch
    {
        public static void Postfix(ref ushort segment, NetInfo info, bool __result)
        {
            if (!__result || !NS2HelpersExtensions.InSimulationThread())
            {
                return;
            }

            if (NetToolMoveMiddleNodePatch.SegmentID > 0) {
                NetworkSkinManager.instance.OnSegmentTransferData(NetToolMoveMiddleNodePatch.SegmentID, segment);
            } else if (NetToolSplitSegmentPatch.SegmentID > 0) {
                NetworkSkinManager.instance.OnSegmentTransferData(NetToolSplitSegmentPatch.SegmentID, segment);
            }
            else if(NetToolCreateNode0Patch.Called) {
                // only when it is called from nettool.
                NetworkSkinManager.instance.OnSegmentPlaced(segment);
            }

        }
    }
}
