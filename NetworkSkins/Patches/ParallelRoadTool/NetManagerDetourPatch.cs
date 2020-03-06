using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace NetworkSkins.Patches.ParallelRoadTool
{
    /// <summary>
    /// Support for ParallelRoadTool
    /// </summary>
    [HarmonyPatch]
    public static class NetManagerDetourPatch
    {
        public static bool Prepare()
        {
            return TargetMethod() != null;
        }

        public static MethodBase TargetMethod()
        {
            return Type.GetType("ParallelRoadTool.Detours.NetManagerDetour, ParallelRoadTool")?
                .GetMethod("CreateSegment", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator il, MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            var originalInstructions = new List<CodeInstruction>(instructions);

            var allowedCallersField = original.DeclaringType?.GetField("AllowedCallers", BindingFlags.Static | BindingFlags.NonPublic);

            var isAllowedCallerMethod = typeof(NetManagerDetourPatch).GetMethod("IsAllowedCaller", BindingFlags.Static | BindingFlags.Public);

            if (allowedCallersField == null || isAllowedCallerMethod == null)
            {
                Debug.LogError("Necessary members not found. Cancelling transpiler!");
                return originalInstructions;
            }

            var codes = new List<CodeInstruction>(originalInstructions);

            for (var index = 0; index < codes.Count; index++)
            {
                if (codes[index].opcode == OpCodes.Ldsfld && codes[index].operand == allowedCallersField)
                {
                    if (codes[index + 1].IsLdloc())
                    {
                        if (codes[index + 2].opcode == OpCodes.Call && (codes[index + 2].operand as MethodBase).Name == "Contains")
                        {
                            codes[index + 2] = new CodeInstruction(codes[index + 2])
                            {
                                opcode = OpCodes.Call,
                                operand = isAllowedCallerMethod
                            };
                            Debug.Log("Patched ParallelRoadTool to work with NS2!");
                            break;
                        }
                    }
                }   
            }

            return codes;
        }

        public static bool IsAllowedCaller(string[] allowedCallers, string caller)
        {
            if (allowedCallers.Contains(caller)) return true;

            var split = caller.Split('.');
            return split.Length == 3
                && split[0] == "NetTool" 
                && IsNameMatching(split[1], "CreateNode") 
                && IsNameMatching(split[2], "CreateNode");
        }

        public static bool IsNameMatching(string methodName, string name)
        {
            return methodName == name
                || methodName.StartsWith($"{name}_Patch")
                || methodName.StartsWith($"DMD<DMD<{name}_Patch");
        }
    }
}
