using HarmonyLib;
using System;
using System.Reflection;

namespace NetworkSkins.Patches.NetTool
{
    [HarmonyPatch]
    public static class NetToolCreateNode0Patch
    {
        public static bool Called = false;
        public static void Prefix()
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                Called = true;
            }
        }
        public static void Postfix()
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                Called = false;
            }
        }

        public static MethodBase TargetMethod()
        {
            /*public static ToolErrors CreateNode(NetInfo info, ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint, 
             * FastList<NodePosition> nodeBuffer, int maxSegments,
             * bool test, bool visualize, bool autoFix, bool needMoney, bool invert, bool switchDir, 
             * ushort relocateBuildingID, out ushort node, out ushort segment, out int cost, out int productionRate)     
             * 
             * bool:6
             * ushort:1
             * out ushort : 2
             * out int : 2
             */
            var ret =  typeof(global::NetTool).GetMethod("CreateNode", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[]
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
                typeof(ushort),
                typeof(ushort).MakeByRefType(),
                typeof(ushort).MakeByRefType(),
                typeof(int).MakeByRefType(),
                typeof(int).MakeByRefType(),
            }, null);
            NS2HelpersExtensions.Assert(ret != null, "expected ret!=null");
            return ret;
        }
    }
}
