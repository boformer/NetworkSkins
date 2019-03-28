using System;
using Harmony;
using ICities;
using NetworkSkins.Patches;
using UnityEngine;

namespace NetworkSkins
{
    // Ideas:
    // Road colors
    // pavement/gravel/ruined
    // ped crossíngs
    public class NetworkSkinsMod : IUserMod
    {
        private const string HarmonyId = "boformer.NetworkSkins";
        private HarmonyInstance _harmony;
        
        public string Name => "Network Skins";
        public string Description => "Change the visual appearance of roads, train tracks and other networks";

        public void OnEnabled()
        {
            if (_harmony != null) return;

            Debug.Log("NetworkSkins Patching...");
            HarmonyInstance.SELF_PATCHING = false;
            HarmonyInstance.DEBUG = true; // TODO remove
            // TODO compile release version of harmony dll
            _harmony = HarmonyInstance.Create(HarmonyId);
            try
            {
                _harmony.PatchAll(GetType().Assembly);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while applying patches!");
                Debug.LogException(e);
            }
        }

        public void OnDisabled()
        {
            _harmony.UnpatchAll(HarmonyId);
            _harmony = null;

            Debug.Log("NetworkSkins Reverted...");
        }
    }
}
