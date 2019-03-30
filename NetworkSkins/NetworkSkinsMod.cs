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
    // TODO ped crossings
    // TODO serialization
    // TODO backup serialization outside of the savegame (or some other ways to prevent data loss!)
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
        private const string HarmonyId = "boformer.NetworkSkins";
        private HarmonyInstance _harmony;
        private bool InGame => LoadingManager.exists && LoadingManager.instance.m_loadingComplete;
        private MainPanel panel;
        private GameObject monitorGameObject;

        public string Name => "Network Skins";
        public string Description => "Change the visual appearance of roads, train tracks and other networks";

        public void OnEnabled() {
            if (InGame) {
                Install();
            }
        }

        public void OnDisabled() {
            Uninstall();
        }

        public override void OnLevelLoaded(LoadMode mode) {
            base.OnLevelLoaded(mode);
            while (!InGame) { }
            OnEnabled();
        }

        public override void OnReleased() {
            base.OnReleased();
            OnDisabled();
        }

        private void Install() {
            Uninstall();
            monitorGameObject = new GameObject(nameof(NetToolMonitor));
            NetToolMonitor.Instance = monitorGameObject.AddComponent<NetToolMonitor>();
            NetToolMonitor.Instance.EventToolStateChanged += OnNetToolStateChanged;
            //InstallHarmony();
        }

        private void Uninstall() {
            if (monitorGameObject != null) {
                NetToolMonitor.Instance.EventToolStateChanged -= OnNetToolStateChanged;
                Destroy(monitorGameObject);
            }
            if (NetworkSkinManager.exists) {
                Destroy(NetworkSkinManager.instance.gameObject);
            }
            //UninstallHarmony();
        }

        private void InstallHarmony() {
            if (_harmony != null) return;

            Debug.Log("NetworkSkins Patching...");
            HarmonyInstance.SELF_PATCHING = false;
            HarmonyInstance.DEBUG = true; // TODO remove
            // TODO compile release version of harmony dll
            _harmony = HarmonyInstance.Create(HarmonyId);
            try {
                _harmony.PatchAll(GetType().Assembly);
            } catch (Exception e) {
                Debug.LogError("Error while applying patches!");
                Debug.LogException(e);
            }
        }
        private void UninstallHarmony() {
            _harmony.UnpatchAll(HarmonyId);
            _harmony = null;

            Debug.Log("NetworkSkins Reverted...");
        }

        private void OnNetToolStateChanged(bool state) {
            if (state) {
                panel = UIView.GetAView().AddUIComponent(typeof(MainPanel)) as MainPanel;
            } else {
                if (panel.gameObject != null) {
                    Destroy(panel.gameObject);
                    panel = null;
                }
            }
        }
    }
}
