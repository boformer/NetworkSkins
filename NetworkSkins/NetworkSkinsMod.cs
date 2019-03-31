using System;
using Harmony;
using ICities;
using UnityEngine;

namespace NetworkSkins
{
    // TODO remove all debug logs
    // TODO ped crossings
    // TODO serialization
    // TODO backup serialization outside of the savegame (or some other ways to prevent data loss!)
    // TODO proof-read harmony.log before release
    // TODO backup current assembly
    // TODO delete obsolete patches
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
