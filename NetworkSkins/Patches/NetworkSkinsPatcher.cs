using UnityEngine;
using HarmonyLib;

namespace NetworkSkins.Patches {
    public static class NetworkSkinsPatcher {
        private const string HarmonyId = "boformer.NetworkSkins";

        private static bool _patched = false;

        public static void Install() {
            if (_patched) return;

            Debug.Log("NetworkSkins Patching...");

            _patched = true;

            #if DEBUG
            Harmony.DEBUG = true;
            #endif

            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(typeof(NetworkSkinsMod).Assembly);
        }

        public static void Uninstall() {
            if (!_patched) return;

            _patched = false;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);

            Debug.Log("NetworkSkins Reverted...");
        }
    }
}
