﻿using HarmonyLib;
using NetworkSkins.Skins;

namespace NetworkSkins.Patches.NetTool {
    [HarmonyPatch(typeof(global::NetTool), "SplitSegment")]
    public class NetToolSplitSegmentPatch {
        internal static NetworkSkin Skin { get; private set; }
        internal static bool CopySkin { get; private set; }

        public static void Prefix(ushort segment) {
            if(Net.NetUtils.InSimulationThread()) {
                Skin = NetworkSkinManager.instance.CopySegmentSkin(segment);
                CopySkin = true;
            }
        }

        public static void Postfix() {
            if(Net.NetUtils.InSimulationThread()) {
                if(CopySkin) {
                    NetworkSkinManager.instance.UsageRemoved(Skin);
                    Skin = null;
                    CopySkin = false;
                }
            }
        }
    }
}