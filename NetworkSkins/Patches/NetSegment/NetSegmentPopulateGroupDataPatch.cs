using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace NetworkSkins.Patches.NetSegment
{
    /// <summary>
    /// Used by lane props, wires (LODs)
    /// </summary>
    [HarmonyPatch]
    public static class NetSegmentPopulateGroupDataPatch
    {
        public static MethodBase TargetMethod()
        {
            // PopulateGroupData(ushort segmentID, int groupX, int groupZ, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool requireSurfaceMaps)
            return typeof(global::NetSegment).GetMethod("PopulateGroupData", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[] {
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
            var netSegmentInfoGetter = typeof(global::NetSegment).GetProperty("Info")?.GetGetMethod();
            if (netSegmentInfoGetter == null)
            {
                Debug.LogError("Necessary field not found. Cancelling transpiler!");
                return instructions;
            }

            var originalCodes = new List<CodeInstruction>(instructions);
            var codes = new List<CodeInstruction>(originalCodes);

            var index = 0;

            CodeInstruction infoLocalVarLdloc = null;
            for (; index < codes.Count; index++)
            {
                // IL_0003: call instance class NetInfo NetSegment::get_Info()
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netSegmentInfoGetter && TranspilerUtils.IsStLoc(codes[index + 1]))
                {
                    infoLocalVarLdloc = TranspilerUtils.BuildLdLocFromStLoc(codes[index + 1]);
                    index += 2;
                    break;
                }
            }

            if (infoLocalVarLdloc == null)
            {
                Debug.LogError("info local variable not found. Cancelling transpiler!");
                return originalCodes;
            }

            var segmentIdLdInstruction = new CodeInstruction(OpCodes.Ldarg_1); // segmentID is first argument

            if (!NetSegmentRenderPatch.PatchLanesAndSegments(il, codes, infoLocalVarLdloc, segmentIdLdInstruction, ref index))
            {
                Debug.LogError("Could not apply NetSegmentRenderPatch. Cancelling transpiler!");
                return originalCodes;
            }

            return codes;
        }
    }
}
