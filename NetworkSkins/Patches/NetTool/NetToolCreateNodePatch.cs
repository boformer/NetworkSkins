using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using NetworkSkins.Net;
using NetworkSkins.Skins;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.NetTool
{
    /// <summary>
    /// Changes:
    /// * Display selected pillar while placing road, 
    /// * Enable upgrading to the same kind of road (but with a different skin)
    /// </summary>
    [HarmonyPatch]
    public static class NetToolCreateNodePatch
    {
        public static MethodBase TargetMethod()
        {
            // CreateNode(NetInfo info, ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint, FastList<NodePosition> nodeBuffer, int maxSegments,
            // bool test, bool testEnds, bool visualize, bool autoFix, bool needMoney, bool invert, bool switchDir, ushort relocateBuildingID,
            // out ushort firstNode, out ushort lastNode, out ushort segment, out int cost, out int productionRate)
            return typeof(global::NetTool).GetMethod("CreateNode", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[]
            {
                typeof(NetInfo),
                typeof(global::NetTool.ControlPoint),
                typeof(global::NetTool.ControlPoint),
                typeof(global::NetTool.ControlPoint),
                typeof(FastList<global::NetTool.NodePosition>),
                typeof(int),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(bool),
                typeof(ushort),
                typeof(ushort).MakeByRefType(),
                typeof(ushort).MakeByRefType(),
                typeof(ushort).MakeByRefType(),
                typeof(int).MakeByRefType(),
                typeof(int).MakeByRefType(),
            }, null);
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            var originalInstructions = new List<CodeInstruction>(instructions);

            var netAiGetNodeBuildingMethod = typeof(NetAI).GetMethod("GetNodeBuilding");
            var netAiCheckBuildPositionMethod = typeof(NetAI).GetMethod("CheckBuildPosition");

            var pillarPatcherGetActiveNodeBuildingMethod = typeof(ActivePillarPatcher).GetMethod("GetActiveNodeBuilding");
            var patchCheckBuildPositionMethod = typeof(NetToolCreateNodePatch).GetMethod("CheckBuildPosition");

            if (netAiGetNodeBuildingMethod == null || pillarPatcherGetActiveNodeBuildingMethod == null)
            {
                Debug.LogError("Necessary methods not found. Cancelling transpiler!");
                return originalInstructions;
            }

            var codes = new List<CodeInstruction>(originalInstructions);

            // Replace all GetNodeBuilding calls with GetActiveNodeBuilding
            for (var index = 0; index < codes.Count; index++)
            {
                if (codes[index].opcode == OpCodes.Callvirt)
                {
                    if (codes[index].operand == netAiGetNodeBuildingMethod)
                    {
                        TranspilerUtils.LogDebug("Found GetNodeBuilding(ushort nodeID, ref NetNode data, out BuildingInfo building, out float heightOffset)");
                        codes[index] = new CodeInstruction(codes[index])
                        {
                            opcode = OpCodes.Call,
                            operand = pillarPatcherGetActiveNodeBuildingMethod
                        };
                    }
                }
            }

            // Replace all CheckBuildPosition calls with Patcher.CheckBuildPosition
            for (var index = 0; index < codes.Count; index++)
            {
                if (codes[index].opcode == OpCodes.Callvirt)
                {
                    if (codes[index].operand == netAiCheckBuildPositionMethod)
                    {
                        TranspilerUtils.LogDebug("Found CheckBuildPosition(bool test, bool visualize, bool overlay, bool autofix, ref NetTool.ControlPoint startPoint, ref NetTool.ControlPoint middlePoint, ref NetTool.ControlPoint endPoint, out BuildingInfo ownerBuilding, out Vector3 ownerPosition, out Vector3 ownerDirection, out int productionRate)");
                        codes[index] = new CodeInstruction(codes[index])
                        {
                            opcode = OpCodes.Call,
                            operand = patchCheckBuildPositionMethod
                        };
                    }
                }
            }

            return codes;
        }

        // Patch for NetAI.CheckBuildPosition that makes it possible to upgrade a segment to a NetInfo of the same type if the skin is different
        public static ToolBase.ToolErrors CheckBuildPosition(NetAI netAi, bool test, bool visualize, bool overlay, bool autofix, ref global::NetTool.ControlPoint startPoint, ref global::NetTool.ControlPoint middlePoint, ref global::NetTool.ControlPoint endPoint, out BuildingInfo ownerBuilding, out Vector3 ownerPosition, out Vector3 ownerDirection, out int productionRate)
        {
            var toolErrors = netAi.CheckBuildPosition(test, visualize, overlay, autofix, ref startPoint, ref middlePoint, ref endPoint, out ownerBuilding, out ownerPosition, out ownerDirection, out productionRate);

            ushort upgradedSegment = middlePoint.m_segment;
            if (startPoint.m_segment == upgradedSegment || endPoint.m_segment == upgradedSegment)
            {
                upgradedSegment = 0;
            }

            if(upgradedSegment != 0 && (toolErrors & ToolBase.ToolErrors.CannotUpgrade) != 0)
            {
                NetInfo upgradedSegmentInfo = global::NetManager.instance.m_segments.m_buffer[upgradedSegment].Info;
                if (!(upgradedSegmentInfo.m_netAI.IsUnderground() && !netAi.SupportUnderground()))
                {
                    // Check if skin is different. If so, allow upgrade!
                    var activeSkin = NetworkSkinManager.instance.GetActiveSkin(upgradedSegmentInfo);
                    var upgradedSegmentSkin = NetworkSkinManager.SegmentSkins[upgradedSegment];
                    if(!Equals(activeSkin, upgradedSegmentSkin))
                    {
                        toolErrors &= ~ToolBase.ToolErrors.CannotUpgrade;
                    }
                }
            }

            return toolErrors;
        }
    }
}
