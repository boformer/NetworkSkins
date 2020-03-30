using Harmony;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetTool
{
    [HarmonyPatch(typeof(global::NetTool), "MoveMiddleNode")]
    public static class NetToolMoveMiddleNodePatch
    {
        internal static NetworkSkin Skin { get; private set; }
        internal static bool CopySkin { get; private set; }

        public static void Prefix(ref ushort node) // TODO remove ref when in lates harmony.
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                ushort segment = NS2HelpersExtensions.GetFirstSegment(node);
                Skin = NetworkSkinManager.instance.CopySegmentSkin(segment);
                CopySkin = true;
            }
        }

        public static void Postfix()
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                if (CopySkin)
                {
                    NetworkSkinManager.instance.UsageRemoved(Skin);
                    Skin = null;
                    CopySkin = false;
                }
            }
        }
    }
}
