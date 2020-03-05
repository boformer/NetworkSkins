using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace NetworkSkins.Patches.NetNode
{
    /// <summary>
    /// Used by wires
    /// </summary>
    [HarmonyPatch]
    public static class NetNodeRenderInstancePatch
    {
        private const byte FlagsArgIndex = 5;

        public static MethodBase TargetMethod()
        {
            // RenderInstance(RenderManager.CameraInfo cameraInfo, ushort nodeID, NetInfo info, int iter, Flags flags, ref uint instanceIndex, ref RenderManager.Instance data)
            return typeof(global::NetNode).GetMethod("RenderInstance", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[]
            {
                typeof(RenderManager.CameraInfo),
                typeof(ushort),
                typeof(NetInfo),
                typeof(int),
                typeof(global::NetNode.Flags),
                typeof(uint).MakeByRefType(),
                typeof(RenderManager.Instance).MakeByRefType()
            }, null);
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            var netNodeRefreshEndDataMethod =
                typeof(global::NetNode).GetMethod("RefreshEndData", BindingFlags.NonPublic | BindingFlags.Instance);
            var netNodeGetSegmentMethod = typeof(global::NetNode).GetMethod("GetSegment");
            var netInfoNodesField = typeof(NetInfo).GetField("m_nodes");

            var netNodeRenderPatchShouldRenderJunctionNodeMethod =
                typeof(NetNodeRenderPatch).GetMethod("ShouldRenderJunctionNode");

            var netNodeRenderPatchShouldRenderBendNodeMethod =
                typeof(NetNodeRenderPatch).GetMethod("ShouldRenderBendNode");

            if (netNodeRefreshEndDataMethod == null|| netNodeGetSegmentMethod == null || netInfoNodesField == null || netNodeRenderPatchShouldRenderJunctionNodeMethod == null || netNodeRenderPatchShouldRenderBendNodeMethod == null)
            {
                Debug.LogError("NetNodeRenderInstancePatch: Necessary methods and field not found. Cancelling transpiler!");
                return instructions;
            }

            var index = 0;

            var originalCodes = new List<CodeInstruction>(instructions);
            var codes = new List<CodeInstruction>(originalCodes);

            var refreshEndDataCallFound = false;
            for (; index < codes.Count; index++)
            {
                // IL_0066: call instance void NetNode::RefreshEndData(uint16, class NetInfo, uint32, valuetype RenderManager/Instance&)
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netNodeRefreshEndDataMethod)
                {
                    refreshEndDataCallFound = true;
                    break;
                }
            }

            if (!refreshEndDataCallFound)
            {
                Debug.LogError("NetNodeRenderInstancePatch: RefreshEndData call not found. Cancelling transpiler!");
                return originalCodes;
            }

            var junctionFlagCheckFound = false;
            for (; index < codes.Count; index++)
            {
                // if ((flags & Flags.Junction) != 0)
                // IL_0077: ldarg.s 'flags'
                // IL_0079: ldc.i4 128
                if (codes[index].opcode == OpCodes.Ldarg_S && (byte)codes[index].operand == FlagsArgIndex
                    && codes[index + 1].opcode == OpCodes.Ldc_I4 && (int)codes[index + 1].operand == (int)global::NetNode.Flags.Junction)
                {
                    junctionFlagCheckFound = true;
                    break;
                }
            }

            if (!junctionFlagCheckFound)
            {
                Debug.LogError("NetNodeRenderInstancePatch: junctionFlagCheck not found. Cancelling transpiler!");
                return originalCodes;
            }

            CodeInstruction segmentLocalVarLdloc = null;
            CodeInstruction segment2LocalVarLdloc = null;
            CodeInstruction nodeLocalVarLdLoc = null;

            for (;index < codes.Count; index++)
            {
                // IL_009c: call instance uint16 NetNode::GetSegment(int32)
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netNodeGetSegmentMethod && TranspilerUtils.IsStLoc(codes[index + 1]))
                {
                    // IL_00a1: stloc.0
                    segmentLocalVarLdloc = TranspilerUtils.BuildLdLocFromStLoc(codes[index + 1]);
                    index += 2;
                    break;
                }
            }

            for (; index < codes.Count; index++)
            {
                // IL_00ac: call instance uint16 NetNode::GetSegment(int32)
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netNodeGetSegmentMethod && TranspilerUtils.IsStLoc(codes[index + 1]))
                {
                    // IL_00b1: stloc.1
                    segment2LocalVarLdloc = TranspilerUtils.BuildLdLocFromStLoc(codes[index + 1]);
                    index += 2;
                    break;
                }
            }

            for (; index < codes.Count; index++)
            {
                // IL_013e: ldfld class NetInfo/Node[] NetInfo::m_nodes
                if (codes[index].opcode == OpCodes.Ldfld && codes[index].operand == netInfoNodesField && TranspilerUtils.IsStLoc(codes[index + 3]))
                {
                    // IL_0146: stloc.s 6
                    nodeLocalVarLdLoc = TranspilerUtils.BuildLdLocFromStLoc(codes[index + 3]);
                    index += 4;
                    break;
                }
            }

            if (segmentLocalVarLdloc == null || segment2LocalVarLdloc == null || nodeLocalVarLdLoc == null)
            {
                Debug.LogError("NetNodeRenderInstancePatch: Necessary field for junction not found. Cancelling transpiler!");
                Debug.LogError($"{segmentLocalVarLdloc}, {segment2LocalVarLdloc}, {nodeLocalVarLdLoc}");
                return originalCodes;
            }

            var junctionRenderCheckInserted = false;
            for (; index < codes.Count; index++)
            {
                // IL_017c: ldc.i4 987135
                // IL_0181: and
                // IL_0182: brfalse IL_0570
                if (codes[index].opcode == OpCodes.Ldc_I4 && (int)codes[index].operand == (int)NetInfo.ConnectGroup.AllGroups
                    && codes[index + 1].opcode == OpCodes.And && codes[index + 2].opcode == OpCodes.Brfalse)
                {
                    var labelIfFalse = codes[index + 2].operand;
                    var insertionPosition = index + 3;

                    // && NetNodeRenderPatch.NetNodeRenderPatch(node, segment, segment2)
                    // IL_01FD: ldloc.s V_6
                    // IL_01FF: ldloc.0
                    // IL_0200: ldloc.1
                    // IL_0201: call bool[NetworkSkins] NetworkSkins.Patches.NetNodeRenderPatch::ShouldRenderJunctionNode(class NetInfo/Node, uint16, uint16)
                    // IL_0206: brfalse.s IL_024A
                    var renderCheckInstructions = new[]
                    {
                        new CodeInstruction(nodeLocalVarLdLoc), 
                        new CodeInstruction(segmentLocalVarLdloc), 
                        new CodeInstruction(segment2LocalVarLdloc), 
                        new CodeInstruction(OpCodes.Call, netNodeRenderPatchShouldRenderJunctionNodeMethod), 
                        new CodeInstruction(OpCodes.Brfalse, labelIfFalse),
                    };

                    renderCheckInstructions[0].labels.AddRange(codes[insertionPosition].labels);
                    codes[insertionPosition].labels.Clear();

                    codes.InsertRange(insertionPosition, renderCheckInstructions);
                    TranspilerUtils.LogDebug("Junction render check inserted");

                    index = insertionPosition + renderCheckInstructions.Length;
                    junctionRenderCheckInserted = true;
                    break;
                }
            }

            if (!junctionRenderCheckInserted)
            {
                Debug.LogError("NetNodeRenderInstancePatch: Render check 1 not inserted. Cancelling transpiler!");
                return originalCodes;
            }

            var bendFlagCheckFound = false;
            for (; index < codes.Count; index++)
            {
                // else if ((flags & Flags.Bend) != 0)
                // IL_0e28: ldarg.s 'flags'
                // IL_0e2a: ldc.i4.s 64
                if (codes[index].opcode == OpCodes.Ldarg_S && (byte)codes[index].operand == FlagsArgIndex
                    && codes[index + 1].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[index + 1].operand == (sbyte)global::NetNode.Flags.Bend)
                {
                    bendFlagCheckFound = true;
                    break;
                }
            }

            if (!bendFlagCheckFound)
            {
                Debug.LogError("NetNodeRenderInstancePatch: bendFlagCheck not found. Cancelling transpiler!");
                return originalCodes;
            }

            CodeInstruction segment5LocalVarLdloc = null;
            CodeInstruction segment6LocalVarLdloc = null;
            CodeInstruction node4LocalVarLdLoc = null;

            for (; index < codes.Count; index++)
            {
                // IL_11ae: call instance uint16 NetNode::GetSegment(int32)
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netNodeGetSegmentMethod && TranspilerUtils.IsStLoc(codes[index + 1]))
                {
                    // IL_00a1: stloc.s 32
                    segment5LocalVarLdloc = TranspilerUtils.BuildLdLocFromStLoc(codes[index + 1]);
                    index += 2;
                    break;
                }
            }

            for (; index < codes.Count; index++)
            {
                // IL_11bf: call instance uint16 NetNode::GetSegment(int32)
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netNodeGetSegmentMethod && TranspilerUtils.IsStLoc(codes[index + 1]))
                {
                    // IL_11c4: stloc.s 33
                    segment6LocalVarLdloc = TranspilerUtils.BuildLdLocFromStLoc(codes[index + 1]);
                    index += 2;
                    break;
                }
            }

            for (; index < codes.Count; index++)
            {
                // IL_120d: ldfld class NetInfo/Node[] NetInfo::m_nodes
                if (codes[index].opcode == OpCodes.Ldfld && codes[index].operand == netInfoNodesField && TranspilerUtils.IsStLoc(codes[index + 3]))
                {
                    // IL_1215: stloc.s 35
                    node4LocalVarLdLoc = TranspilerUtils.BuildLdLocFromStLoc(codes[index + 3]);
                    index += 4;
                    break;
                }
            }

            if (segment5LocalVarLdloc == null || segment6LocalVarLdloc == null || node4LocalVarLdLoc == null)
            {
                Debug.LogError("NetNodeRenderInstancePatch: Necessary field for bend not found. Cancelling transpiler!");
                Debug.LogError($"{segment5LocalVarLdloc} {segment6LocalVarLdloc} {node4LocalVarLdLoc}");
                return originalCodes;
            }

            var bendRenderCheckInserted = false;
            for (; index < codes.Count; index++)
            {
                // IL_124b: ldc.i4 987135
                // IL_1250: and
                // IL_1251: brfalse IL_1637
                if (codes[index].opcode == OpCodes.Ldc_I4 && (int)codes[index].operand == (int)NetInfo.ConnectGroup.AllGroups
                    && codes[index + 1].opcode == OpCodes.And && codes[index + 2].opcode == OpCodes.Brfalse)
                {
                    var labelIfFalse = codes[index + 2].operand;
                    var insertionPosition = index + 3;

                    // ldloc.s 35
                    // ldloc.s 32
                    // ldloc.s 33
                    // call bool[NetworkSkins] NetworkSkins.Patches.NetNodeRenderPatch::ShouldRenderJunctionNode(class NetInfo/Node, uint16, uint16)
                    // brfalse.s IL_1637
                    var renderCheckInstructions = new[]
                    {
                        new CodeInstruction(node4LocalVarLdLoc), 
                        new CodeInstruction(segment5LocalVarLdloc), 
                        new CodeInstruction(segment6LocalVarLdloc), 
                        new CodeInstruction(OpCodes.Call, netNodeRenderPatchShouldRenderBendNodeMethod),
                        new CodeInstruction(OpCodes.Brfalse, labelIfFalse),
                    };

                    renderCheckInstructions[0].labels.AddRange(codes[insertionPosition].labels);
                    codes[insertionPosition].labels.Clear();

                    codes.InsertRange(insertionPosition, renderCheckInstructions);
                    TranspilerUtils.LogDebug("Bend render check inserted");

                    index = insertionPosition + renderCheckInstructions.Length;
                    bendRenderCheckInserted = true;
                    break;
                }
            }

            if (!bendRenderCheckInserted)
            {
                Debug.LogError("NetNodeRenderInstancePatch: Bend render check not inserted. Cancelling transpiler!");
                return originalCodes;
            }

            return codes;
        }
    }
}
