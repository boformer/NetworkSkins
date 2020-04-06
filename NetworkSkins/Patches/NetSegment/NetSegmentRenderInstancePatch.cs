using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace NetworkSkins.Patches.NetSegment
{
    /// <summary>
    /// Used by lane props, wires
    /// </summary>
    [HarmonyPatch]
    public static class NetSegmentRenderInstancePatch
    {
        private const byte InfoArgIndex = 4;

        public static MethodBase TargetMethod()
        {
            // RenderInstance(RenderManager.CameraInfo cameraInfo, ushort segmentID, int layerMask, NetInfo info, ref RenderManager.Instance data)
            return typeof(global::NetSegment).GetMethod("RenderInstance", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[] {
                typeof(RenderManager.CameraInfo),
                typeof(ushort),
                typeof(int),
                typeof(NetInfo),
                typeof(RenderManager.Instance).MakeByRefType()
            }, null);
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            var originalCodes = new List<CodeInstruction>(instructions);
            var codes = new List<CodeInstruction>(originalCodes);

            var index = 0;

            var infoLdInstruction = new CodeInstruction(OpCodes.Ldarg_S, InfoArgIndex);
            var segmentIdLdInstruction = new CodeInstruction(OpCodes.Ldarg_2); // segmentID is second argument

            if (!NetSegmentRenderPatch.PatchLanesAndSegments(il, codes, infoLdInstruction, segmentIdLdInstruction, ref index))
            {
                Debug.LogError("Could not apply NetSegmentRenderPatch. Cancelling transpiler!");
                return originalCodes;
            }

            return codes;
        }
    }
}
