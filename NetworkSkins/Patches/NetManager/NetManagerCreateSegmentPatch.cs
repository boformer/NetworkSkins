using Harmony;
using NetworkSkins.Patches.NetTool;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetManager
{
    // TODO check compat with ParallelRoadTool
    [HarmonyPatch(typeof(global::NetManager), "CreateSegment")]
    public static class NetManagerCreateSegmentPatch
    {
        public static void Postfix(ref ushort segment, bool __result)
        {
            if (!__result || !NS2HelpersExtensions.InSimulationThread())
            {
                return;
            }

            if (NetToolMoveMiddleNodePatch.CopySkin) {
                NetworkSkinManager.instance.PasteSegmentSkin(segment, NetToolMoveMiddleNodePatch.Skin);
            } else if (NetToolSplitSegmentPatch.CopySkin) {
                NetworkSkinManager.instance.PasteSegmentSkin(segment, NetToolSplitSegmentPatch.Skin);
            } else if(NetToolCreateNode0Patch.Called) {
                // only when it is called from nettool.
                NetworkSkinManager.instance.OnSegmentPlaced(segment);
            }

        }
    }
}
