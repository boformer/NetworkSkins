using System;
using Harmony;
using ICities;
using UnityEngine;

namespace NetworkSkins
{
    // TODO alle debug logs entfernen
    // TODO ped crossings
    // TODO serialization
    // TODO backup serialization outside of the savegame (or some other ways to prevent data loss!)
    // TODO rail track LODs!
    // TODO rail track nodes! (catenaries)
    // TODO use PatchUtils.GetLdLocForStLoc in all patches for more safety!
    // TODO für alle patches eine txt-datei mit den modifizierten code einfügen
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
