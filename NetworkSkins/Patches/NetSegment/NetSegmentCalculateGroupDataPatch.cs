using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace NetworkSkins.Patches.NetSegment
{
    /// <summary>
    /// Used by lane props, wires (LODs)
    /// </summary>
    [HarmonyPatch]
    public static class NetSegmentCalculateGroupDataPatch
    {
        public static MethodBase TargetMethod()
        {
            // CalculateGroupData(ushort segmentID, int layer, ref int vertexCount, ref int triangleCount, ref int objectCount, ref RenderGroup.VertexArrays vertexArrays)
            return typeof(global::NetSegment).GetMethod("CalculateGroupData", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[] {
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
                Debug.LogError("NetSegmentCalculateGroupDataPatch: info local variable not found. Cancelling transpiler!");
                return originalCodes;
            }

            var segmentIdLdInstruction = new CodeInstruction(OpCodes.Ldarg_1); // segmentID is first argument

            if (!NetSegmentRenderPatch.PatchLanesAndSegments(il, codes, infoLocalVarLdloc, segmentIdLdInstruction, ref index))
            {
                Debug.LogError("NetSegmentCalculateGroupDataPatch: Could not apply NetSegmentRenderPatch. Cancelling transpiler!");
                return originalCodes;
            }

            return codes;
        }
    }
}
