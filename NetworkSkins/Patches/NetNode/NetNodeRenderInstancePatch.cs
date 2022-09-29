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
    [HarmonyPatch2(delcaringType: typeof(NetNode), delegateType: typeof(RenderInstance))]
    public static class NetNodeRenderInstancePatch
    {
        delegate void RenderInstance(RenderManager.CameraInfo cameraInfo, ushort nodeID, NetInfo info, int iter, NetNode.FlagsLong flags, ref uint instanceIndex, ref RenderManager.Instance data);

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            try {
                var codes = instructions.ToList();
                NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 1); //DC
                NetNodeRenderPatch.PatchCheckFlags(codes, original, occuranceCheckFlags: 4); // DC bend
                return codes;
            } catch (Exception ex) {
                Debug.LogException(ex);
                return instructions;
            }
        }
    }
}
