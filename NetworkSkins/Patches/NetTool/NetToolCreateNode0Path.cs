using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NetworkSkins.Patches.NetTool
{
    [Harmony] 
    public static class NetToolCreateNode0Patch
    {
        public static MethodBase TargetMethod()
        {
            /*public static ToolErrors CreateNode(NetInfo info, ControlPoint startPoint, ControlPoint middlePoint, ControlPoint endPoint, 
             * FastList<NodePosition> nodeBuffer, int maxSegments,
             * bool test, bool visualize, bool autoFix, bool needMoney, bool invert, bool switchDir, 
             * ushort relocateBuildingID, out ushort node, out ushort segment, out int cost, out int productionRate)     
             */
            var ret = typeof(global::NetTool).GetMethod("CreateNode", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new[]
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

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            //Manually patching because struct references and prefix/postfix are used.
            //TODO transpiler is not necessary in harmony 2.0.0.8. Use prefix and postfix instead.
            yield return new CodeInstruction(OpCodes.Call, typeof(NetToolCreateNode0Patch).GetMethod("CallPrefix"));
            foreach (var item in instructions)
                yield return item;
            yield return new CodeInstruction(OpCodes.Call, typeof(NetToolCreateNode0Patch).GetMethod("CallPostfix"));
        }

        public static bool Called = false;
        public static void CallPrefix()
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                Called = true;
            }
        }

        public static void CallPostfix()
        {
            if (NS2HelpersExtensions.InSimulationThread())
            {
                Called = false;
            }
        }
    }
}
