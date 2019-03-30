using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    public static class NetSegmentRenderPatch
    {
        public static bool PatchLanesAndSegments(ILGenerator il, List<CodeInstruction> codes, CodeInstruction infoLdInstruction, CodeInstruction segmentIdLdInstruction, ref int index)
        {
            var netInfoLanesField = typeof(NetInfo).GetField("m_lanes");
            var netInfoSegmentsField = typeof(NetInfo).GetField("m_segments");
            var segmentSkinsField = typeof(NetworkSkinManager).GetField("SegmentSkins", BindingFlags.Static | BindingFlags.Public);
            var networkSkinLanesField = typeof(NetworkSkin).GetField("m_lanes");
            var networkSkinSegmentsField = typeof(NetworkSkin).GetField("m_segments");
            if (netInfoLanesField == null || netInfoSegmentsField == null || segmentSkinsField == null || networkSkinLanesField == null || networkSkinSegmentsField == null)
            {
                Debug.LogError("Necessary field not found. Cancelling transpiler!");
                return false;
            }

            var beginLabel = il.DefineLabel();
            codes[index].labels.Add(beginLabel);

            var customLanesLocalVar = il.DeclareLocal(typeof(NetInfo.Lane[]));
            customLanesLocalVar.SetLocalSymInfo("customLanes");

            var customSegmentsLocalVar = il.DeclareLocal(typeof(NetInfo.Segment[]));
            customSegmentsLocalVar.SetLocalSymInfo("customSegments");

            // TODO at this point it is exactly the same code as in NetSegmentRenderInstancePatch!
            // only difference is infoLocalVarLdloc, beginLabel and index

            var customLanesInstructions = new[]
            {
                // NetInfo.Lane[] customLanes = info.m_lanes;
                new CodeInstruction(infoLdInstruction), // info
                new CodeInstruction(OpCodes.Ldfld, netInfoLanesField),
                new CodeInstruction(OpCodes.Stloc, customLanesLocalVar),

                // NetInfo.Segment[] customSegments = info.m_segments;
                new CodeInstruction(infoLdInstruction), // info
                new CodeInstruction(OpCodes.Ldfld, netInfoSegmentsField),
                new CodeInstruction(OpCodes.Stloc, customSegmentsLocalVar),

                // if (SegmentSkinManager.SegmentSkins[segmentID] != null)
                new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                new CodeInstruction(segmentIdLdInstruction), // segmentID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Brfalse_S, beginLabel),

                // customLanes = SegmentSkinManager.SegmentSkins[segmentID].m_lanes;
                new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                new CodeInstruction(segmentIdLdInstruction), // segmentID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Ldfld, networkSkinLanesField),
                new CodeInstruction(OpCodes.Stloc, customLanesLocalVar),

                // customSegments = SegmentSkinManager.SegmentSkins[segmentID].m:segments;
                new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                new CodeInstruction(segmentIdLdInstruction), // segmentID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Ldfld, networkSkinSegmentsField),
                new CodeInstruction(OpCodes.Stloc, customSegmentsLocalVar),
            };
            codes.InsertRange(index, customLanesInstructions);

            index += customLanesInstructions.Length;

            // Replace all occurences of:
            // ldloc.1 or ldarg.s info
            // ldfld class NetInfo/Lane[] NetInfo::m_lanes
            // -- with --
            // ldloc.s <customLanesLocalVar>
            // and all occurences of:
            // ldloc.1 or ldarg.s info
            // ldfld class NetInfo/Segment[] NetInfo::m_segments
            // -- with --
            // ldloc.s <customSegmentsLocalVar>
            for (; index < codes.Count; index++)
            {
                var code = codes[index];
                if (code.opcode == infoLdInstruction.opcode)
                {
                    Debug.Log($"Found infoLdInstruction opcode {code.opcode} with operand {code.operand} {code.operand?.GetType()} should be {infoLdInstruction.operand} {infoLdInstruction.operand?.GetType()}");
                }

                if (code.opcode == infoLdInstruction.opcode && codes[index + 1].opcode == OpCodes.Ldfld)
                {
                    if (code.operand != infoLdInstruction.operand)
                    {
                        Debug.Log($"sbyte1: {infoLdInstruction.operand is byte}, sbyte2: {code.operand is byte}");
                        if (!(infoLdInstruction.operand is byte sByte1 && code.operand is byte sByte2 &&
                              sByte1 == sByte2))
                        {
                            continue;
                        }
                    }

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

            return true;
        }
    }
}
