using System;
using ColossalFramework.UI;
using Harmony;
using ICities;
using NetworkSkins.GUI;
using NetworkSkins.Patches;
using NetworkSkins.Skins;
using UnityEngine;
using static UnityEngine.Object;

namespace NetworkSkins
{
    // Ideas:
    // Road colors
    // pavement/gravel/ruined
    // ped crossíngs
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
        private const string HarmonyId = "boformer.NetworkSkins";
        private HarmonyInstance _harmony;
        private bool InGame => LoadingManager.exists && LoadingManager.instance.m_loadingComplete;
        private MainPanel panel;

        public string Name => "Network Skins";
        public string Description => "Change the visual appearance of roads, train tracks and other networks";

        public void OnEnabled()
        {
            if(InGame) {
                panel = UIView.GetAView().AddUIComponent(typeof(MainPanel)) as MainPanel;
            }
            return;//remove later
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
        public override void OnLevelLoaded(LoadMode mode) {
            base.OnLevelLoaded(mode);
            while (!InGame) { }
            OnEnabled();
        }

        public void OnDisabled()
        {
            if (panel != null) {
                Destroy(panel.gameObject);
                panel = null;
            }
            if (NetworkSkinManager.exists) {
                Destroy(NetworkSkinManager.instance.gameObject);
            }
            return;
            _harmony.UnpatchAll(HarmonyId);
            _harmony = null;

            Debug.Log("NetworkSkins Reverted...");
        }

        public override void OnReleased() {
            base.OnReleased();
            OnDisabled();
        }
    }
}
