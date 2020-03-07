using HarmonyLib;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetTool
{
    [HarmonyPatch(typeof(global::NetTool), "SplitSegment")]
    public class NetToolSplitSegmentPatch
    {
        public static bool Called = false;
        public static ushort SegmentID = 0;

        public static void Prefix()
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                Called = true;
            }
        }

        public static void Postfix()
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                // Delete data of last moved segment
                if (SegmentID > 0)
                {
                    NetworkSkinManager.instance.OnSegmentRelease(SegmentID);
                }
                SegmentID = 0;
                Called = false;
            }
        }
    }
}
