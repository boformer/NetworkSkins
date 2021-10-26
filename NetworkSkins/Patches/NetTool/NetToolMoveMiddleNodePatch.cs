using HarmonyLib;
using NetworkSkins.Skins;
using NetworkSkins.Net;

namespace NetworkSkins.Patches.NetTool {
    [HarmonyPatch(typeof(global::NetTool), "MoveMiddleNode")]
    public static class NetToolMoveMiddleNodePatch {
        internal static NetworkSkin Skin { get; private set; }
        internal static bool CopySkin { get; private set; }

        public static void Prefix(ref ushort node) // TODO remove ref when in lates harmony.
        {
            if(NetUtils.InSimulationThread()) {
                ushort segment = node.ToNode().GetFirstSegment();
                Skin = NetworkSkinManager.instance.CopySegmentSkin(segment);
                CopySkin = true;
            }
        }

        public static void Postfix() {
            if(NetUtils.InSimulationThread()) {
                if(CopySkin) {
                    NetworkSkinManager.instance.UsageRemoved(Skin);
                    Skin = null;
                    CopySkin = false;
                }
            }
        }
    }
}