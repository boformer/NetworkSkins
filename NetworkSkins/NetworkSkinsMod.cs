using System;
using Harmony;
using ICities;
using NetworkSkins.Skins;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NetworkSkins
{
    public class NetworkSkinsMod : IUserMod
    {
        private const string HarmonyId = "boformer.NetworkSkins";
        private HarmonyInstance _harmony;
        
        public string Name => "Network Skins";
        public string Description => "Change the visual appearance of roads, train tracks and other networks";

        public void OnEnabled()
        {
            NetworkSkinManager.Ensure();

            if (_harmony == null)
            {
                Debug.Log("NetworkSkins Patching...");
                //HarmonyInstance.SELF_PATCHING = false;
                //HarmonyInstance.DEBUG = true;
                _harmony = HarmonyInstance.Create(HarmonyId);
                try // TODO maybe remove try catch to make clear something went wrong
                {
                    _harmony.PatchAll(GetType().Assembly);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while applying patches!");
                    Debug.LogException(e);
                }
            }
        }

        public void OnDisabled()
        {
            _harmony.UnpatchAll(HarmonyId);
            _harmony = null;

            Debug.Log("NetworkSkins Reverted...");

            Object.Destroy(NetworkSkinManager.instance);
        }
    }
}
