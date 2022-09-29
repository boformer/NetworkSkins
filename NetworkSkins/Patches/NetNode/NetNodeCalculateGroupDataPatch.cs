using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonoMod.Utils;

namespace NetworkSkins.Patches._NetNode
{
    /// <summary>
    /// Used by wires (LODs)
    /// </summary>
    [HarmonyPatch2(typeof(NetNode), typeof(CalculateGroupData))]
    public static class NetNodeCalculateGroupDataPatch
    {
        delegate bool CalculateGroupData(ushort nodeID, int layer, ref int vertexCount, ref int triangleCount, ref int objectCount, ref RenderGroup.VertexArrays vertexArrays);

        static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            var codes = instructions.ToList();
            NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 1, counterGetSegment: 2); //DC
            NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 2, counterGetSegment: 2); //DC
            NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 5, counterGetSegment: 0); //DC Bend
            return codes;
        }
    }
}
