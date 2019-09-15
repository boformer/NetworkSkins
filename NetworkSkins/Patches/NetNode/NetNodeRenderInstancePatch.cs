using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using NetworkSkins.Skins;
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

            if (!PatchSegmentsAndNodes(il, codes))
            {
                Debug.LogError("NetNodeRenderInstancePatch: Patch segments/nodes failed!");
                return originalCodes;
            }

            return codes;
        }

        private static bool PatchSegmentsAndNodes(ILGenerator il, List<CodeInstruction> codes)
        {
            var netInfoSegmentsField = typeof(NetInfo).GetField("m_segments");
            var netInfoNodesField = typeof(NetInfo).GetField("m_nodes");
            var nodeSkinsField = typeof(NetworkSkinManager).GetField("NodeSkins", BindingFlags.Static | BindingFlags.Public);
            var networkSkinSegmentsField = typeof(NetworkSkin).GetField("m_segments");
            var networkSkinNodesField = typeof(NetworkSkin).GetField("m_nodes");
            if (netInfoSegmentsField == null || netInfoNodesField == null || nodeSkinsField == null || networkSkinSegmentsField == null || networkSkinNodesField == null)
            {
                Debug.LogError("Necessary field not found. Cancelling transpiler!");
                return false;
            }

            var index = 0;

            var infoLdInstruction = new CodeInstruction(OpCodes.Ldarg_3); // info is third argument
            var nodeIdLdInstruction = new CodeInstruction(OpCodes.Ldarg_2); // nodeID is second argument

            var beginLabel = il.DefineLabel();
            codes[index].labels.Add(beginLabel);

            var customSegmentsLocalVar = il.DeclareLocal(typeof(NetInfo.Segment[]));
            customSegmentsLocalVar.SetLocalSymInfo("customSegments");

            var customNodesLocalVar = il.DeclareLocal(typeof(NetInfo.Lane[]));
            customNodesLocalVar.SetLocalSymInfo("customNodes");
            
            var customSegmentsNodesInstructions = new[]
            {
                // NetInfo.Segment[] customSegments = info.m_segments;
                new CodeInstruction(infoLdInstruction), // info
                new CodeInstruction(OpCodes.Ldfld, netInfoSegmentsField),
                new CodeInstruction(OpCodes.Stloc, customSegmentsLocalVar),

                // NetInfo.Node[] customNodes = info.m_nodes;
                new CodeInstruction(infoLdInstruction), // info
                new CodeInstruction(OpCodes.Ldfld, netInfoNodesField),
                new CodeInstruction(OpCodes.Stloc, customNodesLocalVar),

                // if (SegmentSkinManager.NodeSkins[nodeID] != null) {
                new CodeInstruction(OpCodes.Ldsfld, nodeSkinsField),
                new CodeInstruction(nodeIdLdInstruction), // nodeID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Brfalse_S, beginLabel),

                // customSegments = SegmentSkinManager.NodeSkins[nodeID].m_segments;
                new CodeInstruction(OpCodes.Ldsfld, nodeSkinsField),
                new CodeInstruction(nodeIdLdInstruction), // nodeID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Ldfld, networkSkinSegmentsField),
                new CodeInstruction(OpCodes.Stloc, customSegmentsLocalVar),

                // customNodes = SegmentSkinManager.NodeSkins[nodeID].m_nodes;
                new CodeInstruction(OpCodes.Ldsfld, nodeSkinsField),
                new CodeInstruction(nodeIdLdInstruction), // nodeID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Ldfld, networkSkinNodesField),
                new CodeInstruction(OpCodes.Stloc, customNodesLocalVar),
                // }
            };
            codes.InsertRange(index, customSegmentsNodesInstructions);

            index += customSegmentsNodesInstructions.Length;

            // Replace all occurences of:
            //
            // info.m_nodes
            // -- with --
            // customNodes
            for (; index < codes.Count; index++)
            {
                if (TranspilerUtils.IsSameInstruction(codes[index], infoLdInstruction) && codes[index + 1].opcode == OpCodes.Ldfld)
                {
                    if (codes[index + 1].operand == netInfoSegmentsField)
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
                    else if (codes[index + 1].operand == netInfoNodesField)
                    {
                        // It is important that we copy the labels from the existing instruction!
                        // Otherwise "Label not marked" exception
                        codes[index] = new CodeInstruction(codes[index])
                        {
                            opcode = OpCodes.Ldloc,
                            operand = customNodesLocalVar
                        };
                        codes.RemoveAt(index + 1);
                    }
                }
            }

            return true;
        }
    }
}
