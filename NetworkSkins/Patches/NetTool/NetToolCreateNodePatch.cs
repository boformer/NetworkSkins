using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace NetworkSkins.Patches.NetTool
{
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

            var pillarPatcherGetActiveNodeBuildingMethod = typeof(PillarPatcher).GetMethod("GetActiveNodeBuilding");

            if (netAiGetNodeBuildingMethod == null || pillarPatcherGetActiveNodeBuildingMethod == null)
            {
                Debug.LogError("Necessary methods not found. Cancelling transpiler!");
                return originalInstructions;
            }

            var codes = new List<CodeInstruction>(originalInstructions);

            // Replace all GetColor calls with GetSegmentColor/GetNodeColor
            for (var index = 0; index < codes.Count; index++)
            {
                if (codes[index].opcode == OpCodes.Callvirt)
                {
                    if (codes[index].operand == netAiGetNodeBuildingMethod)
                    {
                        Debug.Log("Found GetNodeBuilding(ushort nodeID, ref NetNode data, out BuildingInfo building, out float heightOffset)");
                        codes[index] = new CodeInstruction(codes[index])
                        {
                            opcode = OpCodes.Call,
                            operand = pillarPatcherGetActiveNodeBuildingMethod
                        };
                    }
                }
            }

            return codes;
        }

        /*
        public static void Prefix(NetInfo info, out PillarPatcherState? __state)
        {
            var skin = NetworkSkinManager.instance.GetActiveSkin(info);
            __state = PillarPatcher.Apply(info, skin);
        }

        public static void Postfix(NetInfo info, ref PillarPatcherState? __state)
        {
            PillarPatcher.Revert(info, __state);
        }
        */
    }
}
