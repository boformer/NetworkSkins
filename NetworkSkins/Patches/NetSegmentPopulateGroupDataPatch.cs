using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace NetworkSkins.Patches
{
    [HarmonyPatch]
    public static class NetSegmentPopulateGroupDataPatch
    {
        public static MethodBase TargetMethod()
        {
            // PopulateGroupData(ushort segmentID, int groupX, int groupZ, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool requireSurfaceMaps)
            return typeof(NetSegment).GetMethod("PopulateGroupData", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[] {
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
            var netSegmentInfoGetter = typeof(NetSegment).GetProperty("Info")?.GetGetMethod();
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
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netSegmentInfoGetter)
                {
                    infoLocalVarLdloc = PatchUtils.GetLdLocForStLoc(codes[index + 1]);
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

            /*
            var beginLabel = il.DefineLabel();
            codes[index].labels.Add(beginLabel);

            // TODO at this point it is exactly the same code as in NetSegmentRenderInstancePatch!
            // only difference is infoLocalVarLdloc, beginLabel and index

            var customLanesInstructions = new[]
            {
                // NetInfo.Lane[] customLanes = info.m_lanes;
                infoLocalVarLdloc, // info
                new CodeInstruction(OpCodes.Ldfld, netInfoLanesField),
                new CodeInstruction(OpCodes.Stloc, customLanesLocalVar),

                // NetInfo.Segment[] customSegments = info.m_segments;
                infoLocalVarLdloc, // info
                new CodeInstruction(OpCodes.Ldfld, netInfoSegmentsField),
                new CodeInstruction(OpCodes.Stloc, customSegmentsLocalVar),

                // if (SegmentSkinManager.SegmentSkins[segmentID] != null)
                new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                new CodeInstruction(OpCodes.Ldarg_2), // segmentID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Brfalse_S, beginLabel),

                // customLanes = SegmentSkinManager.SegmentSkins[segmentID].m_lanes;
                new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                new CodeInstruction(OpCodes.Ldarg_2), // segmentID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Ldfld, networkSkinLanesField),
                new CodeInstruction(OpCodes.Stloc, customLanesLocalVar),

                // customSegments = SegmentSkinManager.SegmentSkins[segmentID].m:segments;
                new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                new CodeInstruction(OpCodes.Ldarg_2), // segmentID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Ldfld, networkSkinSegmentsField),
                new CodeInstruction(OpCodes.Stloc, customSegmentsLocalVar),
            };
            codes.InsertRange(index, customLanesInstructions);

            index += customLanesInstructions.Length;

            // Replace all occurences of:
            // ldloc.1
            // ldfld class NetInfo/Lane[] NetInfo::m_lanes
            // -- with --
            // ldloc.s <customLanesLocalVar>
            // and all occurences of:
            // ldloc.1
            // ldfld class NetInfo/Segment[] NetInfo::m_segments
            // -- with --
            // ldloc.s <customSegmentsLocalVar>
            for (;index < codes.Count; index++)
            {
                if (codes[index].opcode == infoLocalVarLdloc.opcode && codes[index].operand == infoLocalVarLdloc.operand && codes[index + 1].opcode == OpCodes.Ldfld)
                {
                    if (codes[index + 1].operand == netInfoLanesField)
                    {
                        // It is important that we copy the labels from the existing instruction!
                        // Otherwise "Label not marked" exception
                        codes[index] = new CodeInstruction(codes[index])
                        {
                            opcode = OpCodes.Ldloc,
                            operand = customLanesLocalVar
                        };
                        codes.RemoveAt(index + 1);
                    }
                    else if (codes[index + 1].operand == netInfoSegmentsField)
                    {
                        // It is important that we copy the labels from the existing instruction!
                        // Otherwise "Label not marked" exception
                        codes[index] = new CodeInstruction(codes[index])
                        {
                            opcode = OpCodes.Ldloc,
                            operand = customSegmentsLocalVar
                        };
                        codes.RemoveAt(index + 1);
                    }
                }
            }

            return codes;
            */
        }
    }
}
