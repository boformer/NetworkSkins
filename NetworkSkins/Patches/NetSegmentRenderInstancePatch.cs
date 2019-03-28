using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using NetworkSkins.Skins;
using UnityEngine;

namespace NetworkSkins.Patches
{
    [HarmonyPatch]
    public class NetSegmentRenderInstancePatch
    {
        private const byte InfoArgIndex = 4;

        static MethodBase TargetMethod()
        {
            // RenderInstance(RenderManager.CameraInfo cameraInfo, ushort segmentID, int layerMask, NetInfo info, ref RenderManager.Instance data)
            return typeof(NetSegment).GetMethod("RenderInstance", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder,
                new Type[] { typeof(RenderManager.CameraInfo), typeof(ushort), typeof(int), typeof(NetInfo), typeof(RenderManager.Instance).MakeByRefType() }, null);
        }

        static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            var netInfoLanesField = typeof(NetInfo).GetField("m_lanes");
            var segmentSkinsField = typeof(NetworkSkinManager).GetField("SegmentSkins", BindingFlags.Static | BindingFlags.Public);
            var segmentSkinLanesField = typeof(NetworkSkin).GetField("m_lanes");
            if (netInfoLanesField == null || segmentSkinsField == null || segmentSkinLanesField == null)
            {
                Debug.LogError("Necessary field not found. Cancelling transpiler!");
                return instructions;
            }

            var codes = new List<CodeInstruction>(instructions);

            var customLanesLocalVar = il.DeclareLocal(typeof(NetInfo.Lane[]));
            customLanesLocalVar.SetLocalSymInfo("customLanes");

            var beginLabel = il.DefineLabel();
            codes[0].labels.Add(beginLabel);

            var customLanesInstructions = new[]
            {
                // NetInfo.Lane[] customLanes = info.m_lanes;
                new CodeInstruction(OpCodes.Ldarg_S, InfoArgIndex), // info
                new CodeInstruction(OpCodes.Ldfld, netInfoLanesField),
                new CodeInstruction(OpCodes.Stloc, customLanesLocalVar),

                // if (SegmentSkinManager.SegmentSkins[segmentID] != null)
                new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                new CodeInstruction(OpCodes.Ldarg_2), // segmentID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Brfalse_S, beginLabel),

                // lanes = SegmentSkinManager.SegmentSkins[segmentID].m_lanes;
                new CodeInstruction(OpCodes.Ldsfld, segmentSkinsField),
                new CodeInstruction(OpCodes.Ldarg_2), // segmentID
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Ldfld, segmentSkinLanesField),
                new CodeInstruction(OpCodes.Stloc, customLanesLocalVar),
            };
            codes.InsertRange(0, customLanesInstructions);

            // Replace all occurences of:
            // ldarg.s info
            // ldfld class NetInfo/Lane[] NetInfo::m_lanes
            // -- with --
            // IL_018b: ldloc.s <customLanesLocalVar>
            for (var i = customLanesInstructions.Length; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldarg_S && (byte)codes[i].operand == InfoArgIndex)
                {
                    if (codes[i + 1].opcode == OpCodes.Ldfld && codes[i + 1].operand == netInfoLanesField)
                    {
                        // It is important that we copy the labels from the existing instruction!
                        // Otherwise "Label not marked" exception
                        codes[i] = new CodeInstruction(codes[i])
                        {
                            opcode = OpCodes.Ldloc, operand = customLanesLocalVar
                        };
                        codes.RemoveAt(i + 1);
                    }
                }
            }

            return codes;
        }
    }
}
