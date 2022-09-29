using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace NetworkSkins.Patches._NetNode
{
    /// <summary>
    /// Used by wires (LODs)
    /// </summary>
    [HarmonyPatch]
    public static class NetNodePopulateGroupDataPatch
    {
        public static MethodBase TargetMethod()
        {
            // PopulateGroupData(ushort nodeID, int groupX, int groupZ, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool requireSurfaceMaps)
            return typeof(global::NetNode).GetMethod("PopulateGroupData", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[]
            {
                typeof(ushort),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(int).MakeByRefType(),
                typeof(int).MakeByRefType(),
                typeof(Vector3),
                typeof(RenderGroup.MeshData),
                typeof(Vector3).MakeByRefType(),
                typeof(Vector3).MakeByRefType(),
                typeof(float).MakeByRefType(),
                typeof(float).MakeByRefType(),
                typeof(bool).MakeByRefType(),
            }, null);
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            return NetNodeGroupDataPatch.Transpiler(il, instructions);
        }
    }
}
