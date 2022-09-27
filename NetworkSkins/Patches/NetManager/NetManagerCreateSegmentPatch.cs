using System;
using System.Reflection;
using ColossalFramework.Math;
using HarmonyLib;
using NetworkSkins.Patches.NetTool;
using NetworkSkins.Skins;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.NetManager
{
    [HarmonyPatch]
    public static class NetManagerCreateSegmentPatch
    {
        delegate bool CreateSegment(out ushort segment, ref Randomizer randomizer, NetInfo info, TreeInfo treeInfo, ushort startNode, ushort endNode, Vector3 startDirection, Vector3 endDirection, uint buildIndex, uint modifiedIndex, bool invert);

        public static MethodBase TargetMethod() => 
            DelegateUtil.GetMethod<CreateSegment>(typeof(global::NetManager));

        public static void Postfix(ref ushort segment, bool __result) {
            if(!__result || !Net.NetUtils.InSimulationThread()) {
                return;
            }

            if(NetToolMoveMiddleNodePatch.CopySkin) {
                NetworkSkinManager.instance.PasteSegmentSkin(segment, NetToolMoveMiddleNodePatch.Skin);
            } else if(NetToolSplitSegmentPatch.CopySkin) {
                NetworkSkinManager.instance.PasteSegmentSkin(segment, NetToolSplitSegmentPatch.Skin);
            } else if(NetToolCreateNode0Patch.Called) {
                // only when it is called from nettool.
                NetworkSkinManager.instance.OnSegmentPlaced(segment);
            }

        }
    }
}
