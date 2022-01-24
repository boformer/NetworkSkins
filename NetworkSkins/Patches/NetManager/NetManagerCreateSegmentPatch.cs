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
        public static MethodBase TargetMethod() {
            // bool CreateSegment(out ushort segment, ref Randomizer randomizer, NetInfo info, TreeInfo treeInfo, ushort startNode, ushort endNode,
            // Vector3 startDirection, Vector3 endDirection, uint buildIndex, uint modifiedIndex, bool invert)
            return typeof(global::NetManager).GetMethod("CreateSegment", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[]
            {
                typeof(ushort).MakeByRefType(),
                typeof(Randomizer).MakeByRefType(),
                typeof(NetInfo),
                typeof(TreeInfo),
                typeof(ushort),
                typeof(ushort),
                typeof(Vector3),
                typeof(Vector3),
                typeof(uint),
                typeof(uint),
                typeof(bool)
            }, null);
        }

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
