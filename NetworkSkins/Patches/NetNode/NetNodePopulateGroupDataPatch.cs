﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace NetworkSkins.Patches._NetNode {
    /// <summary>
    /// Used by wires (LODs)
    /// </summary>
    [HarmonyPatch2(typeof(NetNode), typeof(PopulateGroupData))]
    public static class NetNodePopulateGroupDataPatch {
        delegate void PopulateGroupData(ushort nodeID, int groupX, int groupZ, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool requireSurfaceMaps); public static MethodBase TargetMethod() => typeof(global::NetNode).

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions, MethodBase original) {
            var codes = instructions.ToList();
            NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 1, counterGetSegment: 2); //DC
            NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 2, counterGetSegment: 2); //DC
            NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 5, counterGetSegment: 0); //DC Bend
            return codes;
        }
    }
}
