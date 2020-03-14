using Harmony;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetTool
{
    [HarmonyPatch(typeof(global::NetTool), "SplitSegment")]
    public class NetToolSplitSegmentPatch
    {
        internal static NetworkSkin Skin;

        public static void Prefix(ushort segment)
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                Skin = NetworkSkinManager.instance.CopySegmentSkin(segment);
            }
        }

        public static void Postfix()
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                if (Skin != null)
                {
                    NetworkSkinManager.instance.UsageRemoved(Skin);
                    Skin = null;
                }
            }
        }
    }
}
