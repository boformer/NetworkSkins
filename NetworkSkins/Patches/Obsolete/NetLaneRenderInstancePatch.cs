// TODO remove, now handled by NetSegmentRenderInstancePatch
/*
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using NetworkSkins.Segments;
using UnityEngine;

namespace NetworkSkins.Patches
{
    [HarmonyPatch(typeof(NetLane), "RenderInstance")]
    public class NetLaneRenderInstancePatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var parameters = original.GetParameters();
            if (original.IsStatic || parameters.Length < 4 || parameters[1].ParameterType != typeof(ushort) || parameters[3].ParameterType != typeof(NetInfo.Lane))
            {
                Debug.LogError("NetLane#RenderInstance signature has changed. Cancelling transpiler!");
                return instructions;
            }

            var netLanePropsMpropsFieldInfo = typeof(NetInfo.Lane).GetField("m_laneProps");

            var codes = new List<CodeInstruction>(instructions);
            
            for (var i = 0; i < Math.Min(codes.Count, 10); i++)
            {
                if (codes[i].opcode == OpCodes.Ldarg_S && (byte)codes[i].operand == 4)
                {
                    if (codes[i + 1].opcode == OpCodes.Ldfld && codes[i + 1].operand == netLanePropsMpropsFieldInfo)
                    {
                        if (codes[i + 2].opcode == OpCodes.Stloc_0)
                        {
                            var lanePropsLocalSetLabel = il.DefineLabel();
                            codes[i + 2].labels.Add(lanePropsLocalSetLabel);

                            codes.InsertRange(i, GetCodeInstructions(lanePropsLocalSetLabel));
                            break;
                        }
                    }
                }
            }

            return codes;
        }

        static IEnumerable<CodeInstruction> GetCodeInstructions(Label lanePropsLocalSetLabel)
        {
            var segmentLanePropsFieldInfo = typeof(SegmentSkinManager).GetField("SegmentLaneProps", BindingFlags.Static | BindingFlags.Public);
            if (segmentLanePropsFieldInfo == null)
            {
                Debug.LogError("segmentLanePropsFieldInfo not found. Cancelling transpiler!");
                yield break;
            }

            yield return new CodeInstruction(OpCodes.Ldsfld, segmentLanePropsFieldInfo);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Ldelem_Ref);
            yield return new CodeInstruction(OpCodes.Dup);
            yield return new CodeInstruction(OpCodes.Brtrue, lanePropsLocalSetLabel);
            yield return new CodeInstruction(OpCodes.Pop);
        }
    }
}
*/