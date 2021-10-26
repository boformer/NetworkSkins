using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static NetTool;

namespace NetworkSkins.Patches.NetTool {
    [Harmony]
    public static class NetToolCreateNode0Patch {
        public delegate ToolBase.ToolErrors CreateNode(NetInfo info, ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint,
             FastList<NodePosition> nodeBuffer, int maxSegments,
             bool test, bool visualize, bool autoFix, bool needMoney, bool invert, bool switchDir,
             ushort relocateBuildingID, out ushort node, out ushort segment, out int cost, out int productionRate);

        static MethodBase TargetMethod() => typeof(global::NetTool).GetMethod<CreateNode>();

        public static bool Called = false;
        static void Prefix() {
            if(NSUtil.InSimulationThread())
                Called = true;
        }

        static void Postfix() {
            if(NSUtil.InSimulationThread())
                Called = false;
        }
    }
}