using HarmonyLib;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetTool {
    [HarmonyPatch(typeof(global::NetTool), "SplitSegment")]
    public class NetToolSplitSegmentPatch {
        internal static NetworkSkin Skin { get; private set; }
        internal static bool CopySkin { get; private set; }

        public static void Prefix(ushort segment) {
            if(NSUtil.InSimulationThread()) {
                Skin = NetworkSkinManager.instance.CopySegmentSkin(segment);
                CopySkin = true;
            }
        }

        public static void Postfix() {
            if(NSUtil.InSimulationThread()) {
                if(CopySkin) {
                    NetworkSkinManager.instance.UsageRemoved(Skin);
                    Skin = null;
                    CopySkin = false;
                }
            }
        }
    }
}