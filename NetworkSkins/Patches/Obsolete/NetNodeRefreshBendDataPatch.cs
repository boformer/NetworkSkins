// TODO remove
/*
using Harmony;
using NetworkSkins.Skins;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches
{
    [HarmonyPatch(typeof(NetNode), "RefreshBendData")]
    public class NetNodeRefreshBendDataPatch
    {
        public static void Prefix(ushort nodeID, NetInfo info, out Color? __state)
        {
            __state = ColorPatcher.Apply(info, NetworkSkinManager.NodeSkins[nodeID]);
        }

        public static void Postfix(NetInfo info, Color? __state)
        {
            ColorPatcher.Revert(info, __state);
        }
    }
}
*/