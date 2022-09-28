using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace NetworkSkins.Patches._NetNode
{
    /// <summary>
    /// Used by wires
    /// </summary>
    [HarmonyPatch2(typeof(NetNode), typeof(RenderInstance))]
    public static class NetNodeRenderInstancePatch
    {
        delegate void RenderInstance(RenderManager.CameraInfo cameraInfo, ushort nodeID, NetInfo info, int iter, NetNode.FlagsLong flags, ref uint instanceIndex, ref RenderManager.Instance data);

        static MethodInfo mCheckFlags => typeof(NetInfo.Node).Method(nameof(NetInfo.Node.CheckFlags));
        static MethodInfo mGetSegment => typeof(NetNode).Method(nameof(NetNode.GetSegment));

        /// <param name="counterGetSegment">
        /// will use the n-th previous call to GetSegment() to determine segmentID.
        /// </param>
        public static void PatchCheckFlags(
            List<CodeInstruction> codes, MethodBase method, int occuranceCheckFlags, bool bend) {

            var iCheckFlags = codes.Search(c => c.Calls(mCheckFlags), count: occuranceCheckFlags);
            int iLdNodeInfo = codes.Search(
                _c => _c.IsLdLoc(typeof(NetInfo.Node), method),
                startIndex: iCheckFlags, count: -1);
            
            CodeInstruction ldNodeInfo = codes[iLdNodeInfo].Clone();
            CodeInstruction ldNodeID = TranspilerUtils.GetLDArg(method, "nodeID");
            CodeInstruction ldSegmentID = BuildSegmentLDLocFromPrevSTLoc(codes, iCheckFlags, 2);
            CodeInstruction ldSegmentID2 = BuildSegmentLDLocFromPrevSTLoc(codes, iCheckFlags, 1);
            var call = bend
                ? typeof(NetNodeRenderPatch).Method(nameof(NetNodeRenderPatch.ShouldRenderBendNode))
                : typeof(NetNodeRenderPatch).Method(nameof(NetNodeRenderPatch.ShouldRenderJunctionNode));

            var newCodes = new[]{
                    ldNodeInfo,
                    ldNodeID,
                    ldSegmentID,
                    ldSegmentID2,
                    new CodeInstruction(OpCodes.Call, call),
                    new CodeInstruction(OpCodes.And),
                };
            codes.InsertInstructions(iCheckFlags + 1, newCodes, moveLabels: true);// insert our check after checkflags
        }

        public static CodeInstruction BuildSegmentLDLocFromPrevSTLoc(
            List<CodeInstruction> codes, int index, int counter = 1) {
            index = codes.Search(c => c.Calls(mGetSegment), startIndex: index, count: counter * -1);
            index = codes.Search(c => c.IsStloc(), startIndex: index);
            return codes[index].BuildLdLocFromStLoc();
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            try {
                var codes = instructions.ToList();
                PatchCheckFlags(codes, original, occuranceCheckFlags: 1, false); //DC
                PatchCheckFlags(codes, original, occuranceCheckFlags: 4, true); // DC bend
                return codes;
            } catch (Exception ex) {
                Debug.LogException(ex);
                return instructions;
            }
        }
    }
}
