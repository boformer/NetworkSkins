using HarmonyLib;
using NetworkSkins.Skins;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using UnityEngine;

namespace NetworkSkins.Patches._NetNode.Wires
{
    /// <summary>
    /// Common patch logic for NetNode.RenderInstance, NetNode.CalculateGroupData and NetNode.PopulateGroupData
    /// Special check for overhead wires on junctions and bends.
    /// Wires are only render when both connected segments have wires.
    /// </summary>
    public static class NetNodeRenderPatch
    {
        private static Shader _electricityNetShader;
        static MethodInfo mCheckFlags => typeof(NetInfo.Node).Method(nameof(NetInfo.Node.CheckFlags));
        static MethodInfo mGetSegment => typeof(NetNode).Method(nameof(NetNode.GetSegment));

        /// <param name="counterGetSegment">
        /// will use the n-th previous call to GetSegment() to determine segmentID.
        /// </param>
        public static void PatchCheckFlags(
            List<CodeInstruction> codes, MethodBase method, int occuranceCheckFlags, int counterGetSegment)
        {
            try
            {
                var iCheckFlags = codes.Search(c => c.Calls(mCheckFlags), count: occuranceCheckFlags);
                int iLdNodeInfo = codes.Search(
                    _c => _c.IsLdLoc(typeof(NetInfo.Node), method),
                    startIndex: iCheckFlags, count: -1);

                CodeInstruction ldNodeInfo = codes[iLdNodeInfo].Clone();
                CodeInstruction ldSegmentID = BuildSegmentLDLocFromPrevSTLoc(codes, iCheckFlags, counterGetSegment);
                CodeInstruction ldSegmentID2 = BuildSegmentLDLocFromPrevSTLoc(codes, iCheckFlags, counterGetSegment - 1);
                var callShouldRender = typeof(NetNodeRenderPatch).Method(nameof(ShouldRender));

                var newCodes = new[]{
                    ldNodeInfo,
                    ldSegmentID,
                    ldSegmentID2,
                    new CodeInstruction(OpCodes.Call, callShouldRender),
                    new CodeInstruction(OpCodes.And),
                };
                codes.InsertInstructions(iCheckFlags + 1, newCodes, moveLabels: true);// insert our check after checkflags
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw ex;
            }
        }

        public static CodeInstruction BuildSegmentLDLocFromPrevSTLoc(
            List<CodeInstruction> codes, int index, int counter = 1)
        {
            index = codes.Search(c => c.Calls(mGetSegment), startIndex: index, count: counter * -1);
            index = codes.Search(c => c.IsStloc(), startIndex: index);
            return codes[index].BuildLdLocFromStLoc();
        }

        public static bool ShouldRender(NetInfo.Node node, ushort segment1, ushort segment2)
        {
            if (_electricityNetShader == null)
            {
                _electricityNetShader = Shader.Find("Custom/Net/Electricity");
            }

            if (node.m_material?.shader != _electricityNetShader)
            {
                return true;
            }

            var segmentSkin1 = NetworkSkinManager.SegmentSkins[segment1];
            var segmentSkin2 = NetworkSkinManager.SegmentSkins[segment2];
            return (segmentSkin1 == null || segmentSkin1.m_hasWires) && (segmentSkin2 == null || segmentSkin2.m_hasWires);
        }
    }
}
