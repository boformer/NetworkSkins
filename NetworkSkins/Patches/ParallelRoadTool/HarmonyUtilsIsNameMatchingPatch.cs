using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace NetworkSkins.Patches.ParallelRoadTool {
    /// <summary>
    /// Support for ParallelRoadTool
    /// </summary>
    [HarmonyPatch]
    public static class HarmonyUtilsIsNameMatchingPatch {
        public static bool Prepare() {
            return TargetMethod() != null;
        }

        public static MethodBase TargetMethod() {
            return Type.GetType("ParallelRoadTool.Utils.HarmonyUtils, ParallelRoadTool")?
                .GetMethod("IsNameMatching", BindingFlags.Static | BindingFlags.Public);
        }

        public static bool Prefix(string methodName, string name, ref bool __result) {
            __result = IsNameMatching(methodName, name);
            return false;
        }

        public static bool IsNameMatching(string methodName, string name) {
            int dotIndex = methodName.LastIndexOf(".", StringComparison.InvariantCulture);
            if (dotIndex != -1) methodName = methodName.Substring(dotIndex + 1);
            return methodName == name
                || methodName.StartsWith($"{name}_Patch")
                || methodName.StartsWith($"DMD<DMD<{name}_Patch");
        }
    }
}
