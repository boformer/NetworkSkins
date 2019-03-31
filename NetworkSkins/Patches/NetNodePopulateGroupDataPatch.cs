using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

// TODO copy from CalculateGroupData

namespace NetworkSkins.Patches
{
    [HarmonyPatch]
    public static class NetNodePopulateGroupDataPatch
    {
        public static MethodBase TargetMethod()
        {
            // PopulateGroupData(ushort nodeID, int groupX, int groupZ, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool requireSurfaceMaps)
            return typeof(NetNode).GetMethod("PopulateGroupData", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[]
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
            var netNodeFlagsField = typeof(NetNode).GetField("m_flags");

            var netNodeGetSegmentMethod = typeof(NetNode).GetMethod("GetSegment");

            var netInfoNodesField = typeof(NetInfo).GetField("m_nodes");

            var netInfoNodeDirectConnectField = typeof(NetInfo.Node).GetField("m_directConnect");

            var netNodeRenderPatcherShouldRenderJunctionNodeMethod =
                typeof(NetNodeRenderPatcher).GetMethod("ShouldRenderJunctionNode");

            var netNodeRenderPatcherShouldRenderBendNodeLodMethod =
                typeof(NetNodeRenderPatcher).GetMethod("ShouldRenderBendNodeLod");

            if (netNodeFlagsField == null || netNodeGetSegmentMethod == null || netInfoNodesField == null || netInfoNodeDirectConnectField == null || netNodeRenderPatcherShouldRenderJunctionNodeMethod == null || netNodeRenderPatcherShouldRenderBendNodeLodMethod == null)
            {
                Debug.LogError("Necessary methods and field not found. Cancelling transpiler!");
                return instructions;
            }

            var index = 0;

            var originalCodes = new List<CodeInstruction>(instructions);
            var codes = new List<CodeInstruction>(originalCodes);

            var junctionFlagCheckFound = false;
            for (; index < codes.Count; index++)
            {
                // if ((m_flags & Flags.Junction) != 0)
                // IL_004b: ldfld valuetype NetNode/Flags NetNode::m_flags
                // IL_0079: ldc.i4 128
                if (codes[index].opcode == OpCodes.Ldfld && codes[index].operand == netNodeFlagsField
                    && codes[index + 1].opcode == OpCodes.Ldc_I4 && (int)codes[index + 1].operand == (int)NetNode.Flags.Junction)
                {
                    junctionFlagCheckFound = true;
                    break;
                }
            }

            if (!junctionFlagCheckFound)
            {
                Debug.LogError("junctionFlagCheck not found. Cancelling transpiler!");
                return originalCodes;
            }

            CodeInstruction segmentLocalVarLdloc = null;
            CodeInstruction segment2LocalVarLdloc = null;
            CodeInstruction nodeLocalVarLdLoc = null;

            // ushort segment = GetSegment(i);
            for (; index < codes.Count; index++)
            {
                // IL_0073: call instance uint16 NetNode::GetSegment(int32)
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netNodeGetSegmentMethod)
                {
                    // IL_0078: stloc.s 5
                    segmentLocalVarLdloc = TranspilerUtils.GetLdLocForStLoc(codes[index + 1]);
                    index += 2;
                    break;
                }
            }

            // ushort segment2 = GetSegment(j);
            for (; index < codes.Count; index++)
            {
                // IL_0107: call instance uint16 NetNode::GetSegment(int32)
                if (codes[index].opcode == OpCodes.Call && codes[index].operand == netNodeGetSegmentMethod)
                {
                    // IL_010c: stloc.s 11
                    segment2LocalVarLdloc = TranspilerUtils.GetLdLocForStLoc(codes[index + 1]);
                    index += 2;
                    break;
                }
            }

            // NetInfo.Node node = info2.m_nodes[k];
            for (; index < codes.Count; index++)
            {
                // IL_0359: ldfld class NetInfo/Node[] NetInfo::m_nodes
                // IL_035e: ldloc.s 21
                // IL_0360: ldelem.ref
                // IL_0361: stloc.s 22
                if (codes[index].opcode == OpCodes.Ldfld && codes[index].operand == netInfoNodesField
                    && codes[index + 1].opcode == OpCodes.Ldloc_S
                    && codes[index + 2].opcode == OpCodes.Ldelem_Ref
                    && TranspilerUtils.GetLdLocForStLoc(codes[index + 3]) != null)
                {
                    // IL_0361: stloc.s 22
                    nodeLocalVarLdLoc = TranspilerUtils.GetLdLocForStLoc(codes[index + 3]);
                    index += 4;
                    break;
                }
            }

            if (segmentLocalVarLdloc == null || segment2LocalVarLdloc == null || nodeLocalVarLdLoc == null)
            {
                Debug.LogError("NetNodePopulateGroupDataPatch: Necessary field for junction not found. Cancelling transpiler!");
                Debug.Log($"{segmentLocalVarLdloc}, {segment2LocalVarLdloc}, {nodeLocalVarLdLoc}");
                return originalCodes;
            }

            var junctionRenderCheckInserted = false;
            for (; index < codes.Count; index++)
            {
                // IL_017c: ldloc.s 22
                // IL_0181: ldfld bool NetInfo/Node::m_directConnect
                // IL_0182: brfalse IL_04c3
                if (TranspilerUtils.IsSameInstruction(codes[index], nodeLocalVarLdLoc, true) 
                    && codes[index + 1].opcode == OpCodes.Ldfld && codes[index + 1].operand == netInfoNodeDirectConnectField
                    && codes[index + 2].opcode == OpCodes.Brfalse)
                {
                    var labelIfFalse = codes[index + 2].operand;
                    var insertionPosition = index + 3;

                    // IL_01FD: ldloc.s 22
                    // IL_01FF: ldloc.s 11
                    // IL_0200: ldloc.s 22
                    // IL_0201: call bool[NetworkSkins] NetworkSkins.Patches.NetNodeRenderPatcher::ShouldRenderJunctionNode(class NetInfo/Node, uint16, uint16)
                    // IL_0206: brfalse.s IL_04c3
                    var renderCheckInstructions = new[]
                    {
                        new CodeInstruction(nodeLocalVarLdLoc),
                        new CodeInstruction(segmentLocalVarLdloc),
                        new CodeInstruction(segment2LocalVarLdloc),
                        new CodeInstruction(OpCodes.Call, netNodeRenderPatcherShouldRenderJunctionNodeMethod),
                        new CodeInstruction(OpCodes.Brfalse, labelIfFalse),
                    };

                    renderCheckInstructions[0].labels.AddRange(codes[insertionPosition].labels);
                    codes[insertionPosition].labels.Clear();

                    codes.InsertRange(insertionPosition, renderCheckInstructions);
                    Debug.Log("Junction render check inserted");

                    index = insertionPosition + renderCheckInstructions.Length;
                    junctionRenderCheckInserted = true;
                    break;
                }
            }

            if (!junctionRenderCheckInserted)
            {
                Debug.LogError("NetNodePopulateGroupDataPatch: Junction render check not inserted. Cancelling transpiler!");
                return originalCodes;
            }

            var bendFlagCheckFound = false;
            for (; index < codes.Count; index++)
            {
                // else if ((m_flags & Flags.Bend) != 0)
                // IL_0e28: ldarg.s 'flags'
                // IL_0e2a: ldc.i4.s 64
                if (codes[index].opcode == OpCodes.Ldfld && codes[index].operand == netNodeFlagsField
                    && codes[index + 1].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[index + 1].operand == (sbyte)NetNode.Flags.Bend)
                {
                    bendFlagCheckFound = true;
                    break;
                }
            }

            if (!bendFlagCheckFound)
            {
                Debug.LogError("bendFlagCheck not found. Cancelling transpiler!");
                return originalCodes;
            }

            CodeInstruction node5LocalVarLdLoc = null;

            for (; index < codes.Count; index++)
            {
                // IL_0359: ldfld class NetInfo/Node[] NetInfo::m_nodes
                // IL_05ed: ldloc.s 44
                // IL_05ef: ldelem.ref
                // IL_05f0: stloc.s 45
                if (codes[index].opcode == OpCodes.Ldfld && codes[index].operand == netInfoNodesField
                   && codes[index + 1].opcode == OpCodes.Ldloc_S
                   && codes[index + 2].opcode == OpCodes.Ldelem_Ref
                   && TranspilerUtils.GetLdLocForStLoc(codes[index + 3]) != null)
                {
                    // IL_0361: stloc.s 22
                    node5LocalVarLdLoc = TranspilerUtils.GetLdLocForStLoc(codes[index + 3]);
                    index += 4;
                    break;
                }
            }

            if (node5LocalVarLdLoc == null)
            {
                Debug.LogError("NetNodePopulateGroupDataPatch: node5 local var for bend not found. Cancelling transpiler!");
                return originalCodes;
            }

            var bendRenderCheckInserted = false;
            for (; index < codes.Count; index++)
            {
                // IL_0990: ldloc.s 40
                // IL_0992: ldfld bool NetInfo/Node::m_directConnect
                // IL_0997: brfalse IL_0b19
                if (codes[index].opcode == OpCodes.Ldc_I4 && (int)codes[index].operand == (int)NetInfo.ConnectGroup.AllGroups
                    && codes[index + 1].opcode == OpCodes.And && codes[index + 2].opcode == OpCodes.Brfalse)
                {
                    var labelIfFalse = codes[index + 2].operand;
                    var insertionPosition = index + 3;

                    var renderCheckInstructions = new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_1), // nodeID
                        new CodeInstruction(node5LocalVarLdLoc),
                        new CodeInstruction(OpCodes.Call, netNodeRenderPatcherShouldRenderBendNodeLodMethod),
                        new CodeInstruction(OpCodes.Brfalse, labelIfFalse),
                    };

                    renderCheckInstructions[0].labels.AddRange(codes[insertionPosition].labels);
                    codes[insertionPosition].labels.Clear();

                    codes.InsertRange(insertionPosition, renderCheckInstructions);
                    Debug.Log("Bend render check inserted");

                    index = insertionPosition + renderCheckInstructions.Length;
                    bendRenderCheckInserted = true;
                    break;
                }
            }

            if (!bendRenderCheckInserted)
            {
                Debug.LogError("NetNodePopulateGroupDataPatch: Bend render check not inserted. Cancelling transpiler!");
                return originalCodes;
            }

            return codes;
        }
    }
}
