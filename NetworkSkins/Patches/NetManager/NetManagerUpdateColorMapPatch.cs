using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace NetworkSkins.Patches.NetManager
{
    /// <summary>
    /// Used by pavement color
    /// </summary>
    [HarmonyPatch(typeof(global::NetManager), "UpdateColorMap")]
    public static class NetManagerUpdateColorMapPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            var originalInstructions = new List<CodeInstruction>(instructions);

            var netAiGetSegmentColorMethod = typeof(NetAI).GetMethod("GetColor", new[] { typeof(ushort), typeof(NetSegment).MakeByRefType(), typeof(InfoManager.InfoMode) });
            var netAiGetNodeColorMethod = typeof(NetAI).GetMethod("GetColor", new Type[] { typeof(ushort), typeof(NetNode).MakeByRefType(), typeof(InfoManager.InfoMode) });

            var colorPatcherGetSegmentColorMethod = typeof(ColorPatcher).GetMethod("GetSegmentColor");
            var colorPatcherGetNodeColorMethod = typeof(ColorPatcher).GetMethod("GetNodeColor");

            if (netAiGetSegmentColorMethod == null || netAiGetNodeColorMethod == null ||  colorPatcherGetSegmentColorMethod == null || colorPatcherGetNodeColorMethod == null)
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
                    if (codes[index].operand == netAiGetSegmentColorMethod)
                    {
                        Debug.Log("Found GetColor(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode)");
                        codes[index] = new CodeInstruction(codes[index])
                        {
                            opcode = OpCodes.Call,
                            operand = colorPatcherGetSegmentColorMethod
                        };
                    }
                    else if(codes[index].operand == netAiGetNodeColorMethod)
                    {
                        Debug.Log("Found GetColor(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode)");
                        codes[index] = new CodeInstruction(codes[index])
                        {
                            opcode = OpCodes.Call,
                            operand = colorPatcherGetNodeColorMethod
                        };
                    }
                }
            }

            return codes;
        }
    }
}
