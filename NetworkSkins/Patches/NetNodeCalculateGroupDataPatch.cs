using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NetworkSkins.Patches
{
    /// <summary>
    /// Used by wires (LODs)
    /// </summary>
    [HarmonyPatch]
    public static class NetNodeCalculateGroupDataPatch
    {
        public static MethodBase TargetMethod()
        {
            // CalculateGroupData(ushort nodeID, int layer, ref int vertexCount, ref int triangleCount, ref int objectCount, ref RenderGroup.VertexArrays vertexArrays)
            return typeof(NetNode).GetMethod("CalculateGroupData", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[]
            {
                typeof(ushort),
                typeof(int),
                typeof(int).MakeByRefType(),
                typeof(int).MakeByRefType(),
                typeof(int).MakeByRefType(),
                typeof(RenderGroup.VertexArrays).MakeByRefType(),
            }, null);
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            return NetNodeGroupDataPatch.Transpiler(il, instructions);
        }
    }
}
