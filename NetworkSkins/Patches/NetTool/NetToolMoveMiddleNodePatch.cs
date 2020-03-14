using Harmony;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetTool
{
    [HarmonyPatch(typeof(global::NetTool), "MoveMiddleNode")]
    public static class NetToolMoveMiddleNodePatch
    {
        public static NetworkSkin Skin = null;
        
        public static void Prefix(ref ushort node) // TODO remove ref when in lates harmony.
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                ushort segment = node.ToNode().GetFirstSegment();
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
