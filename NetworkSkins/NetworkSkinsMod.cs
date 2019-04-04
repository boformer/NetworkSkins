using System;
using ColossalFramework.UI;
using Harmony;
using ICities;
using NetworkSkins.GUI;
using NetworkSkins.Locale;
using NetworkSkins.Patches;
using NetworkSkins.Skins;
using NetworkSkins.TranslationFramework;
using UnityEngine;
using static UnityEngine.Object;

namespace NetworkSkins
{
    // TODO ListBase.cs -> IsSelected, IsFavourite, OnSelectedChanged, OnFavouriteChanged, and SearchBox events implementation
    // TODO NetToolMonitor.cs -> NetInfoDefaultEquals
    public class NetworkSkinsMod : LoadingExtensionBase, IUserMod
    {
        public string Name => "Network Skins";
        public string Description => Translation.Instance.GetTranslation(TranslationID.MOD_DESCRIPTION);
        private const string HarmonyId = "boformer.NetworkSkins";
        private HarmonyInstance _harmony;
        private bool InGame => LoadingManager.exists && LoadingManager.instance.m_loadingComplete;
        private MainPanel panel;
        private GameObject monitorGameObject;

        public void OnEnabled() {
            if (InGame) {
                Install();
            }
            //InstallHarmony();
        }

        public void OnDisabled() {
            //UninstallHarmony();
            Uninstall();
        }

        public override void OnLevelLoaded(LoadMode mode) {
            base.OnLevelLoaded(mode);
            while (!InGame) { }
            Install();
        }

        public override void OnReleased() {
            base.OnReleased();
            Uninstall();
        }

        private void Install() {
            monitorGameObject = new GameObject(nameof(NetToolMonitor));
            NetToolMonitor.Instance = monitorGameObject.AddComponent<NetToolMonitor>();
            NetToolMonitor.Instance.EventToolStateChanged += OnNetToolStateChanged;
            
        }

        private void Uninstall() {
            if (NetworkSkinManager.exists && NetworkSkinManager.instance.gameObject != null) {
                Destroy(NetworkSkinManager.instance.gameObject);
            }
            if (NetToolMonitor.Instance != null) {
                NetToolMonitor.Instance.EventToolStateChanged -= OnNetToolStateChanged;
                if (monitorGameObject != null) {
                    Destroy(monitorGameObject);
                    monitorGameObject = null;
                }
            }
            if (panel != null && panel.gameObject != null) {
                Destroy(panel.gameObject);
                panel = null;
            }
        }

        private void InstallHarmony() {
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

        private void UninstallHarmony() {
            if (_harmony != null) {
                _harmony.UnpatchAll(HarmonyId);
                _harmony = null;
            }

            Debug.Log("NetworkSkins Reverted...");

            Destroy(NetworkSkinManager.instance.gameObject);
        }

        private void OnNetToolStateChanged(bool isToolEnabled) {
            if (isToolEnabled) {
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
